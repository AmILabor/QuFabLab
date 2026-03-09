using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AMI.Util{
    /// <summary>
    /// Returns a random point within a configurable rectangle
    /// </summary>
    public class QuadBounds : MonoBehaviour{
        /// <summary>
        /// Extends of the rectangle
        /// </summary>
        [Tooltip("Extends of the rectangle")]
        [SerializeField] Vector3 size;

        private void OnDrawGizmos() {
            Gizmos.color = Color.blue;
            Gizmos.DrawCube(transform.position,size);
        }
        /// <summary>
        /// Returns a random Vector3 point within the rectangle
        /// </summary>
        /// <returns>the random point</returns>
        public Vector3 RandomPoint(){
            return new Vector3(Random.Range(transform.position.x - size.x,transform.position.x + size.x),Random.Range(transform.position.y - size.y,transform.position.y + size.y),Random.Range(transform.position.z - size.z,transform.position.z + size.z));
        }
    }
}
