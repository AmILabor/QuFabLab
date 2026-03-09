using System;
using Microsoft.MixedReality.Toolkit;
using QuantenKoffer.Bricks;
using QuantenKoffer.Laser;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.Localization;
using Random = UnityEngine.Random;

namespace GhostImaging.Bricks
{
    /*! Der SPDC-Brick ist in der Realtit�t verantwortlich f�r das Konvertieren eines einhergehenden Photon in zwei weniger stark geladene verschr�nkte Photonen.
     * In unseren Projekt wird die Energieladung des Strahles �ber dessen Farbgebung visualisiert. Der einhergehende blaue Strahl hat ein h�heres Energieniveu.
     * Die ausgehenden roten Strahlen haben ein kleineres Energieniveau. Weiterhin w�rden im echten Experiment die ausgehenden Photonen zuf�llig in dem Raum schie�en.
     * Wir umgehen das, indem wir den Zufall auf die Detektorfl�che beschr�nken. Das spart Rechenleistung und Zeit. In der Realit�t soll das Abtasten eines Bild �ber
     * Ghost-Imaging wohl so 5 Stunden dauern. Diese Zeit haben wir bei Demos nicht.
     */
    public class SPDCBrick : Brick
    {
        [SerializeField] public LocalizedString Titel;
        [SerializeField] public LocalizedString Descr;
        [SerializeField] public Pump PreviosBrick;
        [SerializeField] public Vector2Int targetPosition;
        [SerializeField] public GameObject ArrayDetectorTarget;

        private Brick currentTarget;
        private Vector3 bucketHitPoint;
        private Vector3 centerPosition;
        [SerializeField] public LaserBeam OutBeamPrefab;

        private void Start()
        {
            targetPosition = new Vector2Int(-1, -1);
        }

        //! Diese Funktion w�rde ich gerne �ber ein Interface definieren und hier nur implementieren. CA
        public void ShowExplanation()
        {
            AMI.Util.Console.Log("SHOWEXPLANTitle", Titel);
            AMI.Util.Console.Log("SHOWEXPLANDescr", Descr);
        }

        public override LaserBeam[] HandleLaser(LaserBeam beam)
        {
            LaserBeam newBeam = Instantiate(OutBeamPrefab);
            Vector3[] outVectors = getOutVectors(beam.direction);
            LaserBeam[] beams = HandleLaserBase(newBeam, outVectors);
            foreach (var next_beam in beams)
            {
                if (next_beam.to != currentTarget.getCenterTransform())
                {
                    next_beam.RedirectBeamToPosition(bucketHitPoint);
                }

                next_beam.Draw();
            }

            newBeam.DestroyWhenDone();
            return beams;
        }

        private Brick getRandomTarget()
        {
            Brick[] ArrayDetectorTargets = ArrayDetectorTarget.GetComponentsInChildren<Brick>();
            int target = Random.Range(0, ArrayDetectorTargets.Length);
            if (targetPosition.x != -1 && targetPosition.y != -1)
            {
                double index = (double)targetPosition.x +
                               ((double)targetPosition.y * Math.Sqrt(ArrayDetectorTargets.Length));
                return ArrayDetectorTargets[(int)Math.Round(index)];
            }

            return ArrayDetectorTargets[target];
        }

        private void Update()
        {
            //Needs to be set in update because the object gets repositioned by qr code scanning.
            centerPosition = gameObject.GetNamedChild("BrickModel").GetComponent<BoxCollider>().bounds.center;
        }

        protected override Vector3[] getOutVectors(Vector3 inVector)
        {
            currentTarget = getRandomTarget();
            Vector3 targetCenterPosition = currentTarget.getCenterTransform().position;
            float distance = (targetCenterPosition - centerPosition).magnitude;

            Vector3 targetVector = targetCenterPosition - centerPosition;
            Ray r = new Ray(targetCenterPosition, centerPosition - targetCenterPosition);
            Ray computedR = new Ray(centerPosition, Vector3.Reflect(r.direction, transform.forward));
            RaycastHit hit;
            LayerMask OccludingObjectMask = LayerMask.GetMask("DisplayedObjectGI");
            if (Physics.Raycast(computedR.origin, computedR.direction, out hit, distance * 2, OccludingObjectMask))
            {
                bucketHitPoint = computedR.GetPoint(hit.distance);
            }
            else
            {
                bucketHitPoint = computedR.GetPoint(distance);
            }


            return new Vector3[] { targetVector, computedR.direction };
        }
    }
}