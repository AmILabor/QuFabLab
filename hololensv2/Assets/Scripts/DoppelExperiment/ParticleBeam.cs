using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements.Experimental;
using UnityEngine.XR.ARSubsystems;

namespace AMI.DoppelExperiment{
    public class ParticleBeam : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] GameObject particleStart;
        [SerializeField] GameObject wall;
        [SerializeField] InterferencePattern intpat;
        [SerializeField] TrailRenderer particlePrefab;
        [Header("Particle Attributes")]
        [SerializeField] float SpawnFrequency;
        [SerializeField] float SphereSize;
        [SerializeField] float distanceTolerance = 0.01f;
        [SerializeField] float waitOnGoalMultiplier = 5f;
        [SerializeField] float particleSpeed = 0.1f;
        [SerializeField] int numParticles;
        [SerializeField] Color particleColor = Color.red;

        Vector3 endPos;

        public Vector3 EndPos
        {
            get
            {
                return this.endPos;
            }
            set
            {
                endPos = value;
            }
        }


        Vector3 wallPos;
        MeshRenderer wallMesh;


        // Start is called before the first frame update
        void Start()
        {

            wallPos = wall.transform.position;
            wallMesh = wall.GetComponent<MeshRenderer>();

            StartCoroutine(doStuff());
        }
        [ContextMenu("Shoot Particles")]
        public void ShootParticles()
        {
            StartCoroutine(doStuff());
        }


        IEnumerator doStuff()
        {

            for (int i = 0; i < numParticles; i++)
            {
                yield return new WaitForSecondsRealtime(SpawnFrequency);

                GameObject part = generateparticles();                     
                StartCoroutine(shootParticles(part));
            }
        }


        Tuple<Vector3, Vector3> calcOffset()
        {
            Vector3 ret = new Vector3(0, 0, 0);

            int i;  //Random.Range(0, intpat.PixelPos.Count);
            

            i = UnityEngine.Random.Range(0, intpat.PixelPos.Count); // Calculate random index of PixelPos on interference curve
            ret.x = intpat.PixelPos[i].Item1.x; // get x coordinate of pixel position of chosen index of the interference pattern
            
            ret.z = 0.0f;
            if (ret.x > 0)
            {
                // right part of the wall
                ret = intpat.firstLineRenderer.transform.TransformPoint(ret); // transform coordinates, so that the particel ends up in the right spot on the wall

            }
            else
            {
                // left part of the wall
                ret = intpat.secondLineRenderer.transform.TransformPoint(ret);
            }
            ret.y = UnityEngine.Random.Range(-wallMesh.bounds.size.y / 2, wallMesh.bounds.size.y / 2);

            return new Tuple<Vector3, Vector3>(ret, intpat.PixelPos[i].Item2);

        }

        // Shoot particle until it hits the wall
        IEnumerator shootParticles(GameObject part)
        {
            Vector3 offset = new Vector3(0,0,0); // interferenceTuple.Item1;
            Vector3 pixelPosition = new Vector3(0,0,0); //interferenceTuple.Item2;
            (offset, pixelPosition) = calcOffset();
            

            Vector3 endPos = wall.transform.position;
            
            while (part != null)
            {
                part.transform.position = Vector3.MoveTowards(part.transform.position ,  offset + new Vector3(0, 0, 0.0f), particleSpeed * Time.deltaTime);
                
                if(part.transform.position.z > endPos.z - distanceTolerance)
                {
                    yield return new WaitForSeconds(waitOnGoalMultiplier * SpawnFrequency * numParticles);
                    Destroy(part);
                }
                yield return null;
            }
            
        }

        GameObject generateparticles()
        {
            TrailRenderer s = Instantiate<TrailRenderer>(particlePrefab);
            s.GetComponent<Renderer>().material.color = particleColor;
            s.transform.position = particleStart.transform.position;
            s.transform.localScale = new Vector3(SphereSize, SphereSize, SphereSize);

            return s.gameObject;
        }

    }
}
