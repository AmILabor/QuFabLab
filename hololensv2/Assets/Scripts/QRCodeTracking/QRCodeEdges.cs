/// <summary>
/// Enthält die Klasse zur Verwaltung der QR-Code-Eckpunkte für die Positionierung.
/// </summary>
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.QR;

namespace AMI.QRTracking
{
    /// <summary>
    /// Verwaltet die vier Eck-QR-Codes zur Bestimmung der Position und Skalierung eines Objekts.
    /// </summary>
    public class QRCodeEdges : MonoBehaviour
    {
        [SerializeField]
        SpatialGraphCoordinateSystem topLeftPosition, topRightPosition, bottomLeftPosition, bottomRightPosition;

        [SerializeField] string topLeftString, topRightString, bottomLeftString, bottomRightString;
        [SerializeField] SpatialGraphCoordinateSystem edgePrefab;
        QRCode code;

        /// <summary>
        /// Wird aufgerufen, wenn ein neuer QR-Code erkannt wurde, und speichert die entsprechende Eckposition.
        /// </summary>
        /// <param name="args">Ereignisdaten mit dem hinzugefügten QR-Code.</param>
        public void QrCodeAdded(QRCodeAddedEventArgs args)
        {
            if (args.Code.Data == topLeftString)
            {
                topLeftPosition = Instantiate<SpatialGraphCoordinateSystem>(edgePrefab);
                topLeftPosition.Id = args.Code.SpatialGraphNodeId;
                topLeftPosition.transform.localScale = new Vector3(args.Code.PhysicalSideLength,
                    args.Code.PhysicalSideLength, args.Code.PhysicalSideLength);
            }
            else if (args.Code.Data == topRightString)
            {
                topRightPosition = Instantiate<SpatialGraphCoordinateSystem>(edgePrefab);
                topRightPosition.Id = args.Code.SpatialGraphNodeId;
                topRightPosition.transform.localScale = new Vector3(args.Code.PhysicalSideLength,
                    args.Code.PhysicalSideLength, args.Code.PhysicalSideLength);
            }
            else if (args.Code.Data == bottomLeftString)
            {
                bottomLeftPosition = Instantiate<SpatialGraphCoordinateSystem>(edgePrefab);
                bottomLeftPosition.Id = args.Code.SpatialGraphNodeId;
                bottomLeftPosition.transform.localScale = new Vector3(args.Code.PhysicalSideLength,
                    args.Code.PhysicalSideLength, args.Code.PhysicalSideLength);
            }
            else if (args.Code.Data == bottomRightString)
            {
                bottomRightPosition = Instantiate<SpatialGraphCoordinateSystem>(edgePrefab);
                bottomRightPosition.Id = args.Code.SpatialGraphNodeId;
                bottomRightPosition.transform.localScale = new Vector3(args.Code.PhysicalSideLength,
                    args.Code.PhysicalSideLength, args.Code.PhysicalSideLength);
            }

            if (topLeftPosition && topRightPosition && bottomLeftPosition && bottomRightPosition)
            {
                // All Edges detected
                // Place Grid at Center 
                Debug.Log("All Edges detected");
            }
        }

        /// <summary>
        /// Wird aufgerufen, wenn ein QR-Code aktualisiert wurde, und aktualisiert die entsprechende Eckposition.
        /// </summary>
        /// <param name="args">Ereignisdaten mit dem aktualisierten QR-Code.</param>
        public void QrCodeUpdated(QRCodeUpdatedEventArgs args)
        {
            if (args.Code.Data == topLeftString)
            {
                if (topLeftPosition == null)
                {
                    topLeftPosition = Instantiate<SpatialGraphCoordinateSystem>(edgePrefab);
                    topLeftPosition.Id = args.Code.SpatialGraphNodeId;
                    topLeftPosition.transform.localScale = new Vector3(args.Code.PhysicalSideLength,
                        args.Code.PhysicalSideLength, args.Code.PhysicalSideLength);
                }
            }
            else if (args.Code.Data == topRightString)
            {
                if (topRightPosition == null)
                {
                    topRightPosition = Instantiate<SpatialGraphCoordinateSystem>(edgePrefab);
                    topRightPosition.Id = args.Code.SpatialGraphNodeId;
                    topRightPosition.transform.localScale = new Vector3(args.Code.PhysicalSideLength,
                        args.Code.PhysicalSideLength, args.Code.PhysicalSideLength);
                }
            }
            else if (args.Code.Data == bottomLeftString)
            {
                if (bottomLeftPosition == null)
                {
                    bottomLeftPosition = Instantiate<SpatialGraphCoordinateSystem>(edgePrefab);
                    bottomLeftPosition.Id = args.Code.SpatialGraphNodeId;
                    bottomLeftPosition.transform.localScale = new Vector3(args.Code.PhysicalSideLength,
                        args.Code.PhysicalSideLength, args.Code.PhysicalSideLength);
                }
            }
            else if (args.Code.Data == bottomRightString)
            {
                if (bottomRightPosition == null)
                {
                    bottomRightPosition = Instantiate<SpatialGraphCoordinateSystem>(edgePrefab);
                    bottomRightPosition.Id = args.Code.SpatialGraphNodeId;
                    bottomRightPosition.transform.localScale = new Vector3(args.Code.PhysicalSideLength,
                        args.Code.PhysicalSideLength, args.Code.PhysicalSideLength);
                }
            }
        }
    }
}