using UnityEngine;

namespace AMI.Util{  
    /// <summary>
    /// Multiplies scale and sets position of the attached gameobject if enabled
    /// </summary>
    public class ScaleMultiplier : MonoBehaviour{
        /// <summary>
        /// Multiplies scale by this value
        /// </summary>
        [Tooltip("Multiplies scale by this value")]
        [SerializeField] float multiplier;
        /// <summary>
        /// Multiply scale only in editor
        /// </summary>
        [Tooltip("Multiply scale only in editor")]
        [SerializeField] bool onlyInEditor;
        /// <summary>
        /// Also set position of the attached gameObject?
        /// </summary>
        [Tooltip("Also set position of the attached gameObject?")]
        [SerializeField] bool setPosition;
        /// <summary>
        /// position to set the attached gameObject to
        /// </summary>
        [Tooltip("position to set the attached gameObject to")]
        [SerializeField] Vector3 position;

        /// <summary>
        /// Sets scale and position of the attached gameObject if enabled
        /// </summary>
        public void SetScale(){
            if(enabled){
                if(Application.isEditor || !onlyInEditor){
                    transform.localScale *= multiplier;
                }
                if(setPosition){
                    transform.position = position;
                }
            }
        }

        public float GetScale()
        {
            return multiplier;
        }

        private void Start() {
            SetScale();
        }
        private void OnEnable() {
            
        }
    }
}
