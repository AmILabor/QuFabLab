/// <summary>
/// Enthält die abstrakte Basisklasse zur Positionierung von Objekten anhand räumlicher QR-Code-Koordinaten.
/// </summary>
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;
#if WINDOWS_UWP
#endif

namespace MRTKExtensions.QRCodes
{
    /// <summary>
    /// Abstrakte Basisklasse zum Setzen der Position eines GameObjects basierend auf einem räumlichen Koordinatensystem eines QR-Codes.
    /// </summary>
    public abstract class SpatialGraphCoordinateSystemSetter : MonoBehaviour
    {
        public EventHandler<Pose> PositionAcquired;
        public EventHandler PositionAcquisitionFailed; 

        private Queue<Tuple<Guid, float>> locationIdSizes = new Queue<Tuple<Guid, float>>();

        /// <summary>
        /// Fügt eine neue Positionsanfrage mit der ID des räumlichen Knotens und der physischen Seitenlänge zur Warteschlange hinzu.
        /// </summary>
        /// <param name="spatialGraphNodeId">Die ID des räumlichen Graphknotens.</param>
        /// <param name="physicalSideLength">Die physische Seitenlänge des QR-Codes.</param>
        public void SetLocationIdSize(Guid spatialGraphNodeId, float physicalSideLength)
        {
            locationIdSizes.Enqueue(new Tuple<Guid, float>(spatialGraphNodeId, physicalSideLength));
        }

        void Update()
        {
            if (locationIdSizes.Any())
            {
                var locationIdSize = locationIdSizes.Dequeue();
                UpdateLocation(locationIdSize.Item1, locationIdSize.Item2);
            }
        }

        /// <summary>
        /// Aktualisiert die Position basierend auf der räumlichen Knoten-ID und der physischen Größe.
        /// </summary>
        /// <param name="spatialGraphNodeId">Die ID des räumlichen Graphknotens.</param>
        /// <param name="physicalSideLength">Die physische Seitenlänge des QR-Codes.</param>
        protected abstract void UpdateLocation(Guid spatialGraphNodeId, float physicalSideLength);

        /// <summary>
        /// Berechnet die Pose aus der relativen Positionsmatrix und verschiebt sie zur Mitte des QR-Codes.
        /// </summary>
        /// <param name="relativePose">Die relative Transformationsmatrix.</param>
        /// <param name="physicalSideLength">Die physische Seitenlänge des QR-Codes.</param>
        protected void CalculatePosition(System.Numerics.Matrix4x4? relativePose, float physicalSideLength)
        {
            if (relativePose == null)
            {
                PositionAcquisitionFailed?.Invoke(this, null);
                return;
            }
            System.Numerics.Matrix4x4 newMatrix = relativePose.Value;

            // Platform coordinates are all right handed and unity uses left handed matrices. so we convert the matrix
            // from rhs-rhs to lhs-lhs 
            // Convert from right to left coordinate system
            newMatrix.M13 = -newMatrix.M13;
            newMatrix.M23 = -newMatrix.M23;
            newMatrix.M43 = -newMatrix.M43;

            newMatrix.M31 = -newMatrix.M31;
            newMatrix.M32 = -newMatrix.M32;
            newMatrix.M34 = -newMatrix.M34;

            System.Numerics.Vector3 scale;
            System.Numerics.Quaternion rotation1;
            System.Numerics.Vector3 translation1;

            System.Numerics.Matrix4x4.Decompose(newMatrix, out scale, out rotation1, out translation1);
            var translation = new Vector3(translation1.X, translation1.Y, translation1.Z);
            var rotation = new Quaternion(rotation1.X, rotation1.Y, rotation1.Z, rotation1.W);
            var pose = new Pose(translation, rotation);

            // If there is a parent to the camera that means we are using teleport and we should not apply the teleport
            // to these objects so apply the inverse
            if (CameraCache.Main.transform.parent != null)
            {
                pose = pose.GetTransformedBy(CameraCache.Main.transform.parent);
            }
            
            MovePoseToCenter(pose,physicalSideLength);
        }

        /// <summary>
        /// Verschiebt die Pose zur Mitte des QR-Codes und rotiert sie entsprechend.
        /// </summary>
        /// <param name="pose">Die zu verschiebende Pose.</param>
        /// <param name="physicalSideLength">Die physische Seitenlänge des QR-Codes.</param>
        protected void MovePoseToCenter(Pose pose,float physicalSideLength)
        {
            // Rotate 90 degrees 'forward' over 'right' so 'up' is pointing straight up from the QR code
            pose.rotation *= Quaternion.Euler(90, 0, 0);

            // Move the anchor point to the *center* of the QR code
            var deltaToCenter = physicalSideLength * 0.5f;
            pose.position += (pose.rotation * (deltaToCenter * Vector3.right) -
                              pose.rotation * (deltaToCenter * Vector3.forward));
            CheckPosition(pose);
        }

        private Pose? lastPose;

        /// <summary>
        /// Überprüft die Position auf Stabilität und löst bei Übereinstimmung das PositionAcquired-Ereignis aus.
        /// </summary>
        /// <param name="pose">Die zu überprüfende Pose.</param>
        private void CheckPosition(Pose pose)
        {
            if (lastPose == null)
            {
                lastPose = pose;
                return;
            }

            if (Mathf.Abs(Quaternion.Dot(lastPose.Value.rotation, pose.rotation)) > 0.99f &&
                Vector3.Distance(lastPose.Value.position, pose.position) < 0.5f)
            {
                locationIdSizes.Clear();
                lastPose = null;
                gameObject.transform.SetPositionAndRotation(pose.position, pose.rotation);
                PositionAcquired?.Invoke(this, pose);
            }
            else
            {
                lastPose = pose;
            }
        }
    }
}