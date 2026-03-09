using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AMI.HAL{
    public class Magnet : MonoBehaviour{
        [SerializeField] float influenceRadius = 1f;
        [SerializeField] Transform redPart;
        [SerializeField] Transform greenPart;

        public float InfluenceRadius { get => influenceRadius; set => influenceRadius = value; }
        public Transform GreenPart { get => greenPart; set => greenPart = value; }
        public Transform RedPart { get => redPart; set => redPart = value; }

        // OnDrawGizmos gets called in Editor only and is used for drawing helping lines e.g the influence radius of the parts
        private void OnDrawGizmos() {

            Gizmos.color = new Color(1,0,0,0.05f);
            Gizmos.DrawSphere(redPart.position,influenceRadius);
            //Gizmos.DrawWireSphere(redPart.position,influenceRadius);

            Gizmos.color = new Color(0,1,0,0.05f);
            Gizmos.DrawSphere(greenPart.position,influenceRadius);
            //Gizmos.DrawWireSphere(greenPart.position,influenceRadius);


        }
    }
}
