using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace AMI.Util{
    /// <summary>
    /// Invokes a UnityEvent regulary
    /// </summary>
    public class InvokeAtInterval : MonoBehaviour{
        /// <summary>
        /// Event to be invoked
        /// </summary>
        [Tooltip("Event to be invoked")]
        [SerializeField] UnityEvent eventToInvoke;
        /// <summary>
        /// Intervall between invokations
        /// </summary>
        [Tooltip("Intervall between invokations")]
        [SerializeField] float interval = 1f;

        void OnEnable(){
            StartCoroutine(Interval());
        }
        private void OnDisable() {
            StopAllCoroutines();
        }
        /// <summary>
        /// Coroutine invoking the event regulary
        /// </summary>
        IEnumerator Interval(){
            while(true){
                yield return new WaitForSeconds(interval);
                eventToInvoke.Invoke();
            }
        }
        /// <summary>
        /// Restart the Invoke Coroutine
        /// </summary>
        public void Restart(){
            StopAllCoroutines();
            StartCoroutine(Interval());
        }
        /// <summary>
        /// Invoke the Event just once
        /// </summary>
        public void InvokeOnce(){
            eventToInvoke.Invoke();
        }
    }
}
