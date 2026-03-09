using MRTKExtensions.QRCodes;
using UnityEngine;
using UnityEngine.Events;

namespace MRTKExtensions.QRCodes{
    public class QRDetectedAction : MonoBehaviour{
        [SerializeField] QRTrackerController trackerController;
        [SerializeField] Transform objectToPosition;
        [SerializeField] bool activateObject = true;
        [SerializeField] UnityEvent<Transform> actions;
        [SerializeField] bool makeChildOf = false;
        [SerializeField] Transform optionalParent;


        private void Start(){
            trackerController.PositionSet += PoseFound;
        }

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