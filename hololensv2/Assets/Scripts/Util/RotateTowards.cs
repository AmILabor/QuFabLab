/// <summary>
/// Dreht ein GameObject in Richtung eines Ziel-Transforms.
/// </summary>
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AMI.Util{
    /// <summary>
    /// Rotates the gameObject towards a target transform
    /// </summary>
    public class RotateTowards : MonoBehaviour{
        /// <summary>
        /// Rotates the gameObject towards a target transform
        /// </summary>
        /// <param name="target">target to rotate towards</param>
        public void LookAt(Transform target){
            transform.LookAt(target);
        }
    }
}
