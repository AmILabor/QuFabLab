using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AMI.Util{
    /// <summary>
    /// Switches a gameObjects active state to the opposite state
    /// </summary> 
    public class SwitchActive : MonoBehaviour
    {
        /// <summary>
        /// Switches this gameObjects active state
        /// </summary> 
        public void Switch(){
            gameObject.SetActive(!gameObject.activeSelf);
        }
        /// <summary>
        /// Switches the given gameObjects active state
        /// </summary> 
        /// <param name="go">gameObject to switch</param>
        public void DefaultSwitch(GameObject go)
        {
            if (gameObject.activeSelf)
            {
                gameObject.SetActive(false);
                go.gameObject.SetActive(false);
                return;
            }
            gameObject.SetActive(true);
            go.gameObject.SetActive(true);
        }
    }
}
