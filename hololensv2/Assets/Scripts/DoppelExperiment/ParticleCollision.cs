using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AMI.DoppelExperiment{ 
    public class ParticleCollision : MonoBehaviour
    {
        [SerializeField]
        public ParticleSystem toObserve;
        public List<ParticleCollisionEvent> collisionEvents;

        private void Start()
        {
            toObserve = GetComponent<ParticleSystem>(); 
            collisionEvents = new List<ParticleCollisionEvent>();
        }

        private void OnParticleCollision(GameObject other)
        {
            if (other == null) return;
            
            int numCollisionEvents = toObserve.GetCollisionEvents(other, collisionEvents);

            Rigidbody rb = other.GetComponent<Rigidbody>();
            int i = 0;

            while(i < numCollisionEvents)
            {
                if (rb)
                {
                    Vector3 pos = collisionEvents[i].intersection;
                    Debug.Log(pos);
                }
            }
        }
    }
}
