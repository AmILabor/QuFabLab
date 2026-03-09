using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AMI.Util{
    /// <summary>
    /// Moves the attached transform by a choosable amount
    /// </summary>
    public class MoveTransform : MonoBehaviour{
        /// <summary>
        /// Move local or global position
        /// </summary>
        [Tooltip("Move local or global position")]
        [SerializeField] bool moveLocal = false;
        /// <summary>
        /// Amount to move by
        /// </summary>
        [Tooltip("Amount to move by")]
        [SerializeField] Vector3 moveAmount;
        /// <summary>
        /// Execute the move
        /// </summary>
        public void Move(){
            if(moveLocal){
                transform.localPosition += moveAmount;
            }else{
                transform.position += moveAmount;
            }
        }
    }
}
