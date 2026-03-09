using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace AMI.Util{
    /// <summary>
    /// Makes a TMP_Text editable by using the AR keyboard
    /// </summary>
    [RequireComponent(typeof(Microsoft.MixedReality.Toolkit.Experimental.UI.MixedRealityKeyboard))]
    public class KeyboardEditTMP : MonoBehaviour{
        Microsoft.MixedReality.Toolkit.Experimental.UI.MixedRealityKeyboard keyboard;
        /// <summary>
        /// Pre-inserted text in the AR keyboard
        /// </summary>
        [Tooltip("Pre-inserted text in the AR keyboard")]
        [SerializeField] string previewText = "";
        /// <summary>
        /// Multiline Text editing?
        /// </summary>
        [Tooltip("Multiline Text editing?")]
        [SerializeField] bool multiline = false;
        /// <summary>
        /// Target of the AR keyboard text edit
        /// </summary>
        [Tooltip("Target of the AR keyboard text edit")]
        [SerializeField] TMPro.TMP_Text target;
        private void Awake() {
            keyboard = GetComponent<Microsoft.MixedReality.Toolkit.Experimental.UI.MixedRealityKeyboard>();
        }
        /// <summary>
        /// Show the AR Keyboard
        /// </summary>
        public void Show(){
            keyboard.ShowKeyboard(previewText,multiline);
        }
        /// <summary>
        /// Commit text from the AR keyboard to the TMP_Text element
        /// </summary>
        public void Commit(){
            target.text = keyboard.Text;
        }
    }
}
