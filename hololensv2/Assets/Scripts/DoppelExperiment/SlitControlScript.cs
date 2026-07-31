/// <summary>
/// Enthält die Klasse zur Steuerung der Spaltbreite und des Spaltabstands im Doppelspalt-Experiment.
/// </summary>
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AMI.DoppelExperiment{
    public class SlitControlScript : MonoBehaviour
    {
        [SerializeField]
        Transform RLclap,RRclap,LRclap,LLclap;
        [SerializeField]
        float distance;
        private void Start()
        {
            Debug.Log("NameRLclap: "+RLclap.name);
            Debug.DrawRay(RLclap.position, -transform.right, Color.white);
            Debug.DrawRay(RRclap.position, transform.right, Color.green);
            Debug.DrawRay(LRclap.position, transform.right, Color.red);
            Debug.DrawRay(LLclap.position, -transform.right, Color.yellow);
        }
        public void WiderClick()
        {
            RRclap.position += Vector3.right * distance;
            LRclap.position += Vector3.right * distance;

            LLclap.position += Vector3.left * distance;
            RLclap.position += Vector3.left * distance;
        }
        public void SmallerClick()
        {
            RRclap.position += Vector3.left * distance;
            LRclap.position += Vector3.left * distance;

            LLclap.position += Vector3.right * distance;
            RLclap.position += Vector3.right * distance;
        }
    }
}
