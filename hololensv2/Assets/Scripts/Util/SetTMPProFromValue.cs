/// <summary>
/// Setzt den Text eines TMP_Text-Elements basierend auf einem Zahlenwert oder SliderEventData.
/// Wird in Verbindung mit UnityEvents im Inspector verwendet.
/// </summary>
using System.Collections;
using System.Collections.Generic;
using Microsoft.MixedReality.Toolkit.UI;
using UnityEngine;

namespace AMI.Util{
    /// <summary>
    /// Sets a TMP_text text from a non-string value or SliderEventData
    /// Used in conjuncture with UnityEvent
    /// </summary>
    public class SetTMPProFromValue : MonoBehaviour{
        /// <summary>
        /// TMP_text to set
        /// </summary>
        [SerializeField] TMPro.TMP_Text setTMP;
        /// <summary>
        /// set from SliderEventData
        /// </summary>
        /// <param name="data">data to set from</param>
        public void Set(SliderEventData data){
            setTMP.text = data.NewValue.ToString();
        }
        /// <summary>
        /// set from int
        /// </summary>
        /// <param name="data">data to set from</param>
        public void Set(int data){
            setTMP.text = data.ToString();
        }
        /// <summary>
        /// set from float
        /// </summary>
        /// <param name="data">data to set from</param>
        public void Set(float data){
            setTMP.text = data.ToString();
        }
    }
    
}
