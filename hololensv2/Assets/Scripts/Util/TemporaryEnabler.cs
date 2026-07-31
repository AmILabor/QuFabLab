/// <summary>
/// Aktiviert oder deaktiviert einen Renderer für eine festgelegte Zeitdauer.
/// Schaltet nach Ablauf der Dauer automatisch zurück.
/// </summary>
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AMI.Util{   
    /// <summary>
    /// Enables or disables a Renderer for a set duration
    /// </summary>
    public class TemporaryEnabler : MonoBehaviour{
        /// <summary>
        /// Component to switch enabled state for
        /// </summary>
        [SerializeField] Renderer meshRenderer;
        /// <summary>
        /// Duration after which the active State is switched back
        /// </summary>
        [SerializeField] float duration = 1f;
        /// <summary>
        /// The activeState to temporarily change to
        /// </summary>
        [SerializeField] bool desiredState = true;
        Coroutine coroutine;
        /// <summary>
        /// Sets the state of the meshRenderer Component to desiredState, then calls ChangeStateAfterWait to switch it back after waiting for duration
        /// </summary>
        public void Do(){
            if(gameObject.activeSelf){
                meshRenderer.enabled = desiredState;
                if(coroutine != null){
                    StopCoroutine(coroutine);
                }
                coroutine = StartCoroutine(ChangeStateAfterWait(!desiredState));
            }
        }
        /// <summary>
        /// Coroutine which sets the activeState of the Renderer Component to a given value after a wait
        /// </summary>
        /// /// <param name="stateToChangeTo">State to set to after wait</param>
        IEnumerator ChangeStateAfterWait(bool stateToChangeTo){
            yield return new WaitForSeconds(duration);
            meshRenderer.enabled = stateToChangeTo;
            coroutine = null;
        }
    }
}
