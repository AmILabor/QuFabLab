using System.Collections;
using System.Collections.Generic;
using Microsoft.MixedReality.Toolkit.UI;
using UnityEngine;

namespace AMI.Util{
    /// <summary>
    /// Rotates a transform based on a normalized float input locally on the z axis
    /// </summary>
    public class RotateAround : MonoBehaviour{
        /// <summary>
        /// additional rotation in degrees to be applied on top of the normal rotation
        /// </summary> 
        [SerializeField] float basisRotation = 0f;
        /// <summary>
        /// The maximum angle to be rotated by
        /// </summary> 
        [SerializeField] float maxAngle = 45f;
        /// <summary>
        /// value to be added to the normalized rotateAmount input
        /// </summary> 
        [SerializeField] float additionalRotateAmount = -.5f;

        /// <summary>
        /// Rotates the transform based on a normalized float input locally on the z axis
        /// </summary>
        /// <param name="rotateAmount">How much the transform is rotated in relation to maxAngle</param>
        public void RotateAroundLocal(float rotateAmount){
            var angles = transform.localEulerAngles;
            angles.z = basisRotation + (maxAngle * (rotateAmount + additionalRotateAmount));
            transform.localEulerAngles = angles;
        }
        /// <summary>
        /// Rotates the transform based on a normalized float input locally on the z axis
        /// </summary>
        /// <param name="data">Uses NewValue from SliderEventData to determine how much the transform is rotated in relation to maxAngle</param>
        public void RotateAroundLocal(SliderEventData data){
            RotateAroundLocal(data.NewValue);
        }
    }
}
