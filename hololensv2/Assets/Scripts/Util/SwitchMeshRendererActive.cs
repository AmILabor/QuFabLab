using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AMI.Util{
    /// <summary>
    /// Switches a MeshRenderer Component active state to the opposite state
    /// </summary> 
    public class SwitchMeshRendererActive : MonoBehaviour{
        /// <summary>
        /// Component to switch active/inactive
        /// </summary> 
        [Tooltip("Component to switch active/inactive")]
        [SerializeField] MeshRenderer componentToSwitch;
        bool switchOnStart = false;
        private void Start() {
            if(switchOnStart)
                Switch();
        }
        /// <summary>
        /// Switch the active state
        /// </summary>
        public void Switch(){
            componentToSwitch.enabled = !componentToSwitch.enabled;
        }
    }  
}
