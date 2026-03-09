using System.Collections.Generic;
using QuantenKoffer.Bricks;
using UnityEngine;
using UnityEngine.Localization;

namespace QuantenKoffer.Instructions
{
    /// <summary>
    /// A ScriptableObject Instruction containing a series of InstructionDetail steps
    /// </summary>
    [CreateAssetMenu(fileName = "Instruction", menuName = "QFAB/Quantenkoffer/Instruction")]
    public class Instruction : ScriptableObject
    {
        /// <summary>
        /// name of this instruction with translation/s
        /// </summary>
        [Tooltip("name of this instruction with translation/s")] [SerializeField]
        LocalizedString instructionName;

        public string InstructionName
        {
            get => instructionName.GetLocalizedString();
        }

        /// <summary>
        /// List of all required InstructionDetail to complete this Instruction
        /// </summary>
        [Tooltip("List of all required InstructionDetail to complete this Instruction")] [SerializeField]
        List<InstructionDetail> statement;

        public int StepCount
        {
            get => statement.Count;
        }

        /// <summary>
        /// Get the amount of QuBricks required to be placed to complete this Instruction from a given step onwards
        /// </summary>
        /// <param name="fromStep">step from which on the required QuBricks are counted</param>
        /// <returns>Total amount of bricks required to complete this Instruction from the given step onwards</returns>
        public int GetBrickAmount(int fromStep)
        {
            int amount = 0;
            for (int i = fromStep; i < statement.Count; i++)
            {
                amount += statement[i].BrickAmount;
            }

            return amount;
        }

        /// <summary>
        /// Get the InstructionDetail of a given step
        /// </summary>
        /// <param name="step">step of which the InstructionDetail is returned</param>
        /// <returns>InstructionDetail of the given step</returns>
        public InstructionDetail getStep(int step)
        {
            return statement[step];
        }

        /// <summary>
        /// Checks if a given QuBrick is part of a given step and sets errorDialog if not
        /// </summary>
        /// <param name="brick">brick to be checked</param>
        /// <param name="step">step to be checked</param>
        /// <returns>bool if the brick is correct for the given step</returns>
        public bool CheckBrick(Brick brick, int step)
        {
            if (brick is null) return false;
            brick.SetErrors(false, false, false);
            return statement[step].CheckBrick(brick);
            ;
        }
    }
}