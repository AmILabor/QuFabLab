/// <summary>
/// Ändert die Farbe und Höhe von Rasterelementen basierend auf dem Einfluss eines Magneten.
/// Prüft, ob ein Rasterelement innerhalb des Einflussradius des roten oder grünen Magnetteils liegt,
/// und zeigt die Richtung zum Magneten über einen Pfeil an.
/// </summary>
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Andreas:
// Habe die Klasse (ursprünglich RaycastColliderObject oä) so geändert dass keine Raycasts benutzt werden --> bessere Leistung
// durch Referenz zu den beiden Magnet.Transform können wir ohne Raycast bestimmen ob diese in Einflussreichweite sind 

namespace AMI.HAL{

    [RequireComponent(typeof(Renderer))] // wenn die Klasse einem ScenenObjekt hinzugefügt wird fügt diese selber den Required Component hinzu(falls nochnicht vorhanden)
    public class MagnetInfluenceColoring : MonoBehaviour
    {
        Renderer changling;

        /// <summary>
        /// Reference to direction Arrow Transform
        /// </summary>
        [Tooltip("Reference to direction Arrow Transform")]
        [SerializeField] Transform directionArrow;

        /// <summary>
        /// default Cube Color
        /// </summary>
        [Tooltip("default Cube Color")]
        [SerializeField] Color baseColor = Color.yellow;
        /// <summary>
        /// Color under green Influence
        /// </summary>
        [Tooltip("Color under green Influence")]
        [SerializeField] Color greenMagnetColor = Color.green;
        /// <summary>
        /// Color under red Influence
        /// </summary>
        [Tooltip("Color under red Influence")]
        [SerializeField] Color redMagnetColor = Color.red;

        /// <summary>
        /// Transform of the red Magnet
        /// </summary>
        [Tooltip("Transform of the red Magnet")]
        public Magnet magnet;

        /// <summary>
        /// Also raise the Height if Influenced?
        /// </summary>
        [Tooltip("Also raise the Height if Influenced?")]
        [SerializeField] bool raiseHeight = true;

        /// <summary>
        /// if raiseHeight is set how far will the Elements be raised
        /// </summary>
        [Tooltip("if raiseHeight is set how far will the Elements be raised")]
        [SerializeField] Vector3 raiseAmount = Vector3.one;

        bool isRaised = false;
        Vector3 oldPos;

        /// <summary>
        /// Holt die Renderer-Komponente beim Start.
        /// </summary>
        private void Awake(){
            changling = GetComponent<Renderer>();
        }
        /// <summary>
        /// Aktualisiert die Farbe und Position jedes Rasterelements basierend auf dem Magnetfeldeinfluss.
        /// </summary>
        void FixedUpdate(){
            

            bool redInfluence = false;
            bool greenInfluence = false;

            changling.material.color = baseColor;
            
            if(IsUnderInfluence(magnet.GreenPart,magnet.InfluenceRadius)) {
                greenInfluence = true; 
                changling.material.color = greenMagnetColor; 
                if(raiseHeight && !isRaised){
                    Vector3 posAdjust = raiseAmount;
                    transform.localPosition += posAdjust;
                    isRaised = true;
                }
            }
            if(IsUnderInfluence(magnet.RedPart,magnet.InfluenceRadius)){
                redInfluence = true;
                changling.material.color = redMagnetColor;
                if(raiseHeight && !isRaised){
                    Vector3 posAdjust = raiseAmount;
                    transform.localPosition += posAdjust;
                    isRaised = true;
                }
            }
            // Wenn beide beeinflussen --> welcher Magnet ist näher
            if(redInfluence && greenInfluence){
                // Distanz zu redMagnet >= Distanz zu greenMagnet?
                var distRed = Vector3.Distance(transform.position,magnet.RedPart.position);
                var distGreen = Vector3.Distance(transform.position,magnet.GreenPart.position);
                if(distRed >= distGreen){
                    changling.material.color = greenMagnetColor;
                }else{
                    changling.material.color = redMagnetColor;
                }
            }
            if(!redInfluence && !greenInfluence){
                directionArrow.gameObject.SetActive(false);
                if(raiseHeight && isRaised){
                    Vector3 posAdjust = raiseAmount;
                    transform.localPosition -= posAdjust;
                    isRaised = false;
                }
            }
            if(redInfluence){
                directionArrow.gameObject.SetActive(true);
                directionArrow.LookAt(magnet.RedPart);
            }else if(greenInfluence){
                directionArrow.gameObject.SetActive(true);
                directionArrow.LookAt(magnet.GreenPart);
            }
            
        }

        /// <summary>
        /// Check if GameObject is under Influence of a given Magnet (by Distance & Scale)
        /// </summary>
        /// <param name="magnet">Transform of the Magnet </param>
        public bool IsUnderInfluence(Transform magnetPart,float magnetInfluenceRadius){
            return Vector3.Distance(magnetPart.position,transform.position)<= magnetInfluenceRadius;
        }
    }
}
