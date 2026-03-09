using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace AMI.Util{

/// <summary>
/// Display Log messages within a TMPro text element
/// </summary>
    public class DebugMessageEvent : MonoBehaviour{

        /// <summary>
        /// Which type of Log do you want to display
        /// </summary>
        [Tooltip("Which type of Log do you want to display")]
        [SerializeField] LogType logType;

        /// <summary>
        /// Also log stackTrace ?
        /// </summary>
        [Tooltip("Also log stackTrace ?")]
        [SerializeField] bool logStackTrace = false;

        /// <summary>
        /// Separator for separating Log message & stackTrace
        /// </summary>
        [Tooltip("Separator for separating Log message & stackTrace")]
        [SerializeField] string stackTraceSeparator= "_";

        /// <summary>
        /// which character/string is used for a line break
        /// </summary>
        [Tooltip("which character/string is used for a line break")]
        [SerializeField] string lineBreak = "\n";

        /// <summary>
        /// textfield for displaying the Messages
        /// </summary>
        [Tooltip("textfield for displaying the Messages")]
        [SerializeField] TMPro.TMP_Text textTMP;

        /// <summary>
        /// textfield for displaying the PageNumber
        /// </summary>
        [Tooltip("textfield for displaying the PageNumber")]
        [SerializeField] TMPro.TMP_Text pageNumberTMP;
        private void Awake() {
            Application.logMessageReceived += HandleLog;
            textTMP.text = "";
        }
        /// <summary>
        /// Callback handler for received Debug messages
        /// </summary>
        void HandleLog(string logString, string stackTrace, LogType type){
            string msg = logString;
            if(type==logType){
                if(logStackTrace){
                    msg+=stackTraceSeparator+stackTrace+lineBreak;
                }
                textTMP.text += lineBreak + msg;
                textTMP.pageToDisplay = textTMP.textInfo.pageCount +1;
                pageNumberTMP.text = textTMP.pageToDisplay.ToString();
            }
        }
        /// <summary>
        /// Toggles the next page on the TMP_Text element
        /// </summary>
        public void NextPage(){
            if(textTMP.pageToDisplay < textTMP.textInfo.pageCount){
                textTMP.pageToDisplay++;
                pageNumberTMP.text = textTMP.pageToDisplay.ToString();
            }
        }
        /// <summary>
        /// Toggles the previous page on the TMP_Text element
        /// </summary>
        public void PreviousPage(){
            if(textTMP.pageToDisplay > 0){
                textTMP.pageToDisplay--;
                pageNumberTMP.text = textTMP.pageToDisplay.ToString();
            }
        }

    }
}
