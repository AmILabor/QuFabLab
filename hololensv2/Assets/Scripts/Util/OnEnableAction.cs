/// <summary>
/// Führt ein im Inspector konfigurierbares UnityEvent beim Aktivieren und/oder Deaktivieren des Objekts aus.
/// </summary>
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace AMI.Util{
    /// <summary>
    /// Fires an inspector configurable UnityEvent on enable and/or disable
    /// </summary>
    public class OnEnableAction : MonoBehaviour{
        /// <summary>
        /// Event called when enabled
        /// </summary>
        [SerializeField] UnityEvent enableAction;
        /// <summary>
        /// Event called when disabled
        /// </summary>
        [SerializeField] UnityEvent disableAction;

        void OnEnable(){
            enableAction.Invoke();
        }
        void OnDisable(){
            disableAction.Invoke();
        }
    }
}
