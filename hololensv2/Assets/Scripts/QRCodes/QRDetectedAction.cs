/// <summary>
/// Enthält die Aktionsklasse, die bei Erkennung eines QR-Codes ausgeführt wird.
/// </summary>
using MRTKExtensions.QRCodes;
using UnityEngine;
using UnityEngine.Events;

namespace MRTKExtensions.QRCodes{
    /// <summary>
    /// Führt Aktionen aus, wenn ein QR-Code erkannt und positioniert wurde.
    /// </summary>
    public class QRDetectedAction : MonoBehaviour{
        [SerializeField] QRTrackerController trackerController;
        [SerializeField] Transform objectToPosition;
        [SerializeField] bool activateObject = true;
        [SerializeField] UnityEvent<Transform> actions;
        [SerializeField] bool makeChildOf = false;
        [SerializeField] Transform optionalParent;


        /// <summary>
        /// Abonniert das PositionSet-Ereignis des Tracker-Controllers.
        /// </summary>
        private void Start(){
            trackerController.PositionSet += PoseFound;
        }

        /// <summary>
        /// Wird aufgerufen, wenn eine Pose gefunden wurde, und positioniert das zugehörige Objekt.
        /// </summary>
        /// <param name="sender">Die Ereignisquelle.</param>
        /// <param name="pose">Die gefundene Pose.</param>
        private void PoseFound(object sender, Pose pose){
            if(objectToPosition){
                objectToPosition.SetPositionAndRotation(pose.position, pose.rotation);
                if(makeChildOf)
                    objectToPosition.SetParent(optionalParent);
                if (activateObject)
                    objectToPosition.gameObject.SetActive(true);
                actions?.Invoke(objectToPosition);
            }else{
                Debug.Log("QRDetectedAction has no Transform to position assigned");
            }

        }
    }
}