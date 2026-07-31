/// <summary>
/// Enthält Klassen zur reflexionsbasierten Positionierung von Objekten.
/// </summary>
using System;
using Unity.XR.CoreUtils;
using UnityEditor;
using UnityEngine;

namespace QuantenKoffer.Bricks
{
    /// <summary>
    /// Platziert ein Objekt basierend auf der Reflexion eines Laserstrahls an einem Spiegel.
    /// </summary>
    public class ReflectivePlacer : MonoBehaviour

    {
        private bool initialized = false;
        private Vector3 mirrorPosition;
        private Vector3 center;
        [SerializeField] private GameObject target;
        [SerializeField] private GameObject mirrorObject;

        /// <summary>
        /// Setzt die Initialisierung zurück, damit die Position neu berechnet wird.
        /// </summary>
        public void SetReflectedPosition()
        {
            initialized = false;
        }

        /// <summary>
        /// Berechnet die reflektierte Position und Rotation für das Zielobjekt.
        /// </summary>
        private void Update()
        {
            if (!initialized)
            {
                center = gameObject.GetNamedChild("Quad").GetComponent<Renderer>().bounds.center;
                mirrorPosition = mirrorObject.GetComponent<Brick>().getCenterTransform().position;
                float mirrorDistance = (center - mirrorPosition).magnitude;
                Ray r = new Ray(center, mirrorPosition - center);

                Quaternion targetRotation = Quaternion.LookRotation(-r.direction);
                transform.rotation = targetRotation;
                
                RaycastHit hit;
                LayerMask mask = LayerMask.GetMask("MirrorColliderGI");
                if (Physics.Raycast(r.origin, r.direction, out hit,mirrorDistance*2, mask))
                {
                    Debug.DrawRay(r.origin,r.direction,Color.blue,30);   
                    r = new Ray(hit.point, Vector3.Reflect(r.direction, hit.normal));
                    Debug.DrawRay(r.origin,r.direction,Color.blue,30);
                    Vector3 newCenter = r.GetPoint(mirrorDistance);
                    Vector3 targetCenter = target.GetNamedChild("Quad").GetComponent<Renderer>().bounds.center;
                    Vector3 diff = targetCenter - newCenter;
                    
                    targetRotation = Quaternion.LookRotation(r.direction);
                    target.transform.rotation = targetRotation;
                    target.transform.position -= diff;
                }
                initialized = true;
            }

            
        }
    }
}