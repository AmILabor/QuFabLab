/// <summary>
/// Enthält die OpenXR-Implementierung zur Positionierung mittels räumlichem Koordinatensystem.
/// </summary>
using System;
using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;
using Microsoft.MixedReality.OpenXR;

namespace MRTKExtensions.QRCodes
{
    /// <summary>
    /// Setzt das räumliche Koordinatensystem für OpenXR-basierte Plattformen.
    /// </summary>
    public class OpenXRSpatialGraphCoordinateSystemSetter : SpatialGraphCoordinateSystemSetter
    {
        /// <summary>
        /// Aktualisiert die Position mithilfe des OpenXR-räumlichen Graphknotens.
        /// </summary>
        /// <param name="spatialGraphNodeId">Die ID des räumlichen Graphknotens.</param>
        /// <param name="physicalSideLength">Die physische Seitenlänge des QR-Codes.</param>
        protected override void UpdateLocation(Guid spatialGraphNodeId, float physicalSideLength)
        {
            var node = spatialGraphNodeId != Guid.Empty ? SpatialGraphNode.FromStaticNodeId(spatialGraphNodeId) : null;
            if (node != null && node.TryLocate(FrameTime.OnUpdate, out Pose pose))
            {
                if (CameraCache.Main.transform.parent != null)
                {
                    pose = pose.GetTransformedBy(CameraCache.Main.transform.parent);
                }

                MovePoseToCenter(pose,physicalSideLength);
            }
            else
            {
                PositionAcquisitionFailed?.Invoke(this, null);
            }
        }
    }
}