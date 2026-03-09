using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AMI.Util{
    /// <summary>
    /// Makes a transform always face the Camera
    /// </summary>
    public class FaceCamera : MonoBehaviour
    {
        /// <summary>
        /// Upwards direction of the world
        /// </summary>
        [Tooltip("Upwards direction of the world")]
        [SerializeField] Vector3 worldUp = Vector3.up;
        private void FixedUpdate() {
            transform.LookAt(Camera.main.transform.position,worldUp);
            transform.Rotate(0,180,0);

        }
    } 
}
