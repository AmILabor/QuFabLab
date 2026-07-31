/// <summary>
/// Wandelt Spracheingabe in Text um und zeigt diesen auf einem TMP_Text-Element an.
/// Steuert die Aufnahme über einen DictationHandler und schaltet den Aufnahmezustand um.
/// </summary>
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.Input;

namespace AMI.Util{
    /// <summary>
    /// Converts speech input to text and displays it on a TMP_Text element
    /// </summary>
    public class SpeechToString : MonoBehaviour{
        /// <summary>
        /// textfield for displaying the Messages
        /// </summary>
        [Tooltip("textfield for displaying the Messages")]
        [SerializeField] TMPro.TMP_Text recordStateTMP;
        /// <summary>
        /// Reference to the DictationHandler used for recording
        /// </summary>
        [Tooltip("Reference to the DictationHandler used for recording")]
        [SerializeField] DictationHandler dictationHandler;

        public bool RecordingState { get => dictationHandler.IsListening; set => SetRecordingState(value); }

        private void Awake() {
            if(dictationHandler == null){
                Debug.Log("DictationHandler not set",this);
            }
        }
        /// <summary>
        /// Start or Stop the recording & display the text if recording was stopped
        /// </summary>
        void SetRecordingState(bool value){
            if(value && !RecordingState){
                dictationHandler.StartRecording();
            }else if(!value && RecordingState){
                dictationHandler.StopRecording();
            }else{
                Debug.Log("Desired RecordingState is already set");
            }
            if(RecordingState && recordStateTMP != null){
                recordStateTMP.text = "Stop Recording";
            }else{
                recordStateTMP.text = "Start Recording";
            }
        }
        /// <summary>
        /// Switches the RecordingState
        /// </summary>
        public void SwitchRecordState(){
            SetRecordingState(!RecordingState);
        }
    }
}
