using System.Collections;
using System.Diagnostics;
using Microsoft.MixedReality.Toolkit.Audio;
using QuantenKoffer.Bricks;
using QuantenKoffer.Case;
using UnityEngine;

namespace QuantenKoffer.Instructions
{
    /// <summary>
    /// Interface for managing Instructions (Setup,autoComplete,TTS,Highlights)
    /// </summary>
    public class InstructionDialogue : MonoBehaviour
    {
        [Header("References")]
        /// <summary>
        /// reference to the TextToSpeech Component
        /// </summary>
        [Tooltip("reference to the TextToSpeech Component")]
        [SerializeField]
        TextToSpeech textToSpeech;

        /// <summary>
        /// reference to the TMP_Text Component for displaying the name of the Instruction
        /// </summary>
        [Tooltip("reference to the TMP_Text Component for displaying the name of the Instruction")] [SerializeField]
        TMPro.TMP_Text displayName;

        /// <summary>
        /// reference to the TMP_Text Component for displaying the description of the Instruction
        /// </summary>
        [Tooltip("reference to the TMP_Text Component for displaying the description of the Instruction")]
        [SerializeField]
        TMPro.TMP_Text displayDescription;

        /// <summary>
        /// reference to the TMP_Text Component for displaying the current step of the Instruction
        /// </summary>
        [Tooltip("reference to the TMP_Text Component for displaying the current step of the Instruction")]
        [SerializeField]
        TMPro.TMP_Text displayStep;

        /// <summary>
        /// reference to the KofferGrid
        /// </summary>
        [Tooltip("reference to the KofferGrid")] [SerializeField]
        KofferGrid grid;

        /// <summary>
        /// reference to the TestCaseCreator
        /// </summary>
        [Tooltip("reference to the TestCaseCreator")] [SerializeField]
        TestCaseCreator testCaseCreator;

        /// <summary>
        /// reference to the displayed Instruction
        /// </summary>
        [Tooltip("reference to the displayed Instruction")] [SerializeField]
        Instruction instruction;

        /// <summary>
        /// reference to the AudioSource to be played when a step is completed
        /// </summary>
        [Tooltip("reference to the AudioSource to be played when a step is completed")] [SerializeField]
        AudioSource successAudio;

        /// <summary>
        /// reference to the AudioSource to be played when a step becomes uncompleted
        /// </summary>
        [Tooltip("reference to the AudioSource to be played when a step becomes uncompleted")] [SerializeField]
        AudioSource failAudio;

        [Header("Settings")]
        /// <summary>
        /// should highlights be cleared when this component becomes disabled
        /// </summary>
        [Tooltip("should highlights be cleared when this component becomes disabled")]
        [SerializeField]
        bool clearHighlightsOnDisable = true;

        private int lastCompletedStep = 0;

        /// <summary>
        /// should highlights be set when this component becomes enabled
        /// </summary>
        [Tooltip("should highlights be set when this component becomes enabled")] [SerializeField]
        bool setHighlightsOnEnable = true;

        /// <summary>
        /// is audio output enabled?
        /// </summary>
        [Tooltip("is audio output enabled?")] [SerializeField]
        bool audioEnabled = true;

        string language;


        public string CurrentLanguage
        {
            get => language;
        }

        public Instruction Instruction
        {
            get => instruction;
            set => SetInstruction(value);
        }

        public bool AudioEnabled
        {
            get => audioEnabled;
            set => audioEnabled = value;
        }

        int currentStep = -1;

        void Awake()
        {
            if (instruction)
                SetInstruction(instruction);
        }

        private void OnDisable()
        {
            if (clearHighlightsOnDisable)
            {
                ClearHighlights();
            }
        }

        private void OnEnable()
        {
            if (setHighlightsOnEnable && gameObject.activeSelf)
            {
                grid.ClearHighlights();
                var inst = instruction.getStep(currentStep);
                for (int i = 0; i < inst.Highlights.Count; i++)
                {
                    grid.HighlightField(inst.Highlights[i].Position);
                }
            }

            VerifyCompletedSteps();
        }

        /// <summary>
        /// Called through editor when language changes
        /// We need this to know when to refill referenced components with the translated strings
        /// </summary>
        /// <param name="newLanguage">name of the new language</param>
        public void LanguageChanged(string newLanguage)
        {
            language = newLanguage;
            SetStep(currentStep);
        }

        /// <summary>
        /// Clear all highlights from KofferGrid and hide all error dialogs
        /// </summary>
        private void ClearHighlights()
        {
            grid.ClearHighlights();
        }

        /// <summary>
        /// Changes the displayed Instruction or re-reads its content
        /// </summary>
        /// <param name="instruction">new Instruction</param>
        public void SetInstruction(Instruction instruction)
        {
            grid.ClearHighlights();
            this.instruction = instruction;
            currentStep = 0;
            displayName.text = instruction.InstructionName;
            displayDescription.text = instruction.getStep(currentStep).Description.GetLocalizedString();
            displayStep.text = $"{currentStep + 1} / {instruction.StepCount}";
            

            var inst = instruction.getStep(currentStep);
            for (int i = 0; i < inst.Highlights.Count; i++)
            {
                if (!(inst.Highlights[i].Position.x == -1 && inst.Highlights[i].Position.y == -1))
                    grid.HighlightField(inst.Highlights[i].Position);
            }

            ReadStep(currentStep);
        }

        /// <summary>
        /// Moves to the next step of the current Instruction
        /// </summary>
        [ContextMenu("Next Step")]
        public void NextStep()
        {
            SetStep(currentStep + 1);
        }

        /// <summary>
        /// Moves to the previous step of the current Instruction
        /// </summary>
        [ContextMenu("Previous Step")]
        public void PreviousStep()
        {
            SetStep(currentStep - 1);
        }

        /// <summary>
        /// Moves to a specific step of the current Instruction
        /// </summary>
        /// <param name="step">step to move to</param>
        public void SetStep(int step)
        {
            if (gameObject.activeSelf)
            {
                int previousStep = currentStep;

                grid.ClearHighlights();
                currentStep = step;
                if (currentStep >= 0 && currentStep < instruction.StepCount)
                {
                    displayDescription.text = instruction.getStep(currentStep).Description.GetLocalizedString();
                }
                else
                {
                    currentStep = previousStep;
                }

                displayStep.text = $"{currentStep + 1} / {instruction.StepCount}";
                displayName.text = instruction.InstructionName;

                var inst = instruction.getStep(currentStep);
                for (int i = 0; i < inst.Highlights.Count; i++)
                {
                    if (inst.Highlights[i].Position.x == -1 &&
                        inst.Highlights[i].Position.y == -1)
                        continue;
                    if (grid.GetField(inst.Highlights[i].Position) == null)
                        grid.HighlightField(inst.Highlights[i].Position);
                }

                if (currentStep > previousStep && audioEnabled)
                {
                    successAudio.Play();
                }
                else if (currentStep < previousStep && audioEnabled)
                {
                    failAudio.Play();
                }

                if (previousStep != currentStep)
                    ReadStep(currentStep);
            }
        }

        /// <summary>
        /// Reads the current Instructions specified step out loud
        /// </summary>
        /// <param name="step">step to read out loud</param>
        public void ReadStep(int step)
        {
            if (audioEnabled && !Application.isEditor)
                textToSpeech.StartSpeaking(instruction.getStep(step).Description.GetLocalizedString());
        }

        /// <summary>
        /// Checks each step of the current Instruction for completion and moves currentStep to the last uncompleted one
        /// </summary>
        /// <param name="stepIndex">i'm afraid to change the function signature because quite a few events configured within
        /// the editor use it to map to this function and i don't want them to break</param>
        public bool VerifyCompletedSteps(int stepIndex = 0)
        {
            InstructionDetail step;
            if (gameObject.activeSelf && instruction)
            {
                for (int i = 0; i < instruction.StepCount; i++)
                {
                    step = instruction.getStep(i);

                    if (step.Highlights.Count > 0)
                    {
                        if (step.Highlights[0].Position.x == -1 &&
                            step.Highlights[0].Position.y == -1)
                        {
                            if (currentStep > stepIndex)
                                continue;
                        }
                    }

                    if (!step.IsCompleted(grid))
                    {
                        SetStep(i);
                        return false;
                    }
                }
                
                SetStep(instruction.StepCount - 1);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Switches the state of audioEnabled
        /// </summary>
        public void AudioEnabledSwitch()
        {
            audioEnabled = !audioEnabled;
        }

        /// <summary>
        /// Checks if all steps are completed and sets currentStep to the last uncompleted one
        /// </summary>
        [ContextMenu("Check if current step is completed")]
        public void IsCurrentStepCompeleted()
        {
            VerifyCompletedSteps(0);
        }

        /// <summary>
        /// Checks if a brick is at the right place,rotation and of the right type for the currentStep
        /// </summary>
        public void CheckBrick(Brick brick)
        {
            if (gameObject.activeSelf)
            {
                instruction?.CheckBrick(brick, currentStep);
                VerifyCompletedSteps();
            }
        }

        /// <summary>
        /// Automatically builds the current Instruction
        /// </summary>
        /// <param name="delayed">bool if there is delay between each build step or if it's instant</param>
        public void BuildInstruction(bool delayed)
        {
            if (delayed)
            {
                testCaseCreator.CreateFromInstructionWithDelay(instruction, currentStep);
                StartCoroutine(CheckIfCompletedPeriodically(testCaseCreator.BuildingDelay,
                    instruction.GetBrickAmount(currentStep) - 1));
            }
            else
            {
                testCaseCreator.CreateFromInstruction(instruction, currentStep);
                VerifyCompletedSteps();
            }
        }

        /// <summary>
        /// Periodically checks if current Instruction is completed
        /// </summary>
        /// <param name="wait">waittime between each check</param>
        /// <param name="cycles">amount of checks made in total</param>
        IEnumerator CheckIfCompletedPeriodically(float wait, int cycles)
        {
            VerifyCompletedSteps();
            for (int i = 0; i < cycles; i++)
            {
                yield return new WaitForSeconds(wait);
                VerifyCompletedSteps();
            }
        }
    }
}