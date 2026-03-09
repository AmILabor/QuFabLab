using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AMI.Util{
    /// <summary>
    /// Offsets this transforms position by a configurable amount
    /// </summary>
    public class OffsetPosition : MonoBehaviour{
        /// <summary>
        /// amount to offset by
        /// </summary>
        public Vector3 offset = Vector3.zero;
        /// <summary>
        /// Apply the offset
        /// </summary>
        public void Apply(){
            transform.position += offset;
        }
    }
}
