/// <summary>
/// Enthält die Datenklasse für Informationen eines erkannten QR-Codes.
/// </summary>
using System;
using Microsoft.MixedReality.QR;

namespace MRTKExtensions.QRCodes
{
	/// <summary>
	/// Enthält die Informationen eines erkannten QR-Codes.
	/// </summary>
	public class QRInfo
	{
        /// <summary>
        /// Erstellt eine neue QRInfo-Instanz aus einem QRCode-Objekt.
        /// </summary>
        /// <param name="code">Der zugrunde liegende QR-Code.</param>
        public QRInfo(QRCode code)
        {
            Id = code.Id;
            SpatialGraphNodeId = code.SpatialGraphNodeId;
            Version = code.Version;
            PhysicalSideLength = code.PhysicalSideLength;
            Data = code.Data;
            SystemRelativeLastDetectedTime = code.SystemRelativeLastDetectedTime;
            LastDetectedTime = code.LastDetectedTime;
        }

        /// <summary>
        /// Gibt die eindeutige ID des QR-Codes zurück.
        /// </summary>
        public Guid Id { get; }
        /// <summary>
        /// Gibt die ID des räumlichen Graphknotens zurück.
        /// </summary>
        public Guid SpatialGraphNodeId { get; }
        /// <summary>
        /// Gibt die Version des QR-Codes zurück.
        /// </summary>
        public QRVersion Version { get; }
        /// <summary>
        /// Gibt die physische Seitenlänge des QR-Codes zurück.
        /// </summary>
        public float PhysicalSideLength { get; }
        /// <summary>
        /// Gibt den Dateninhalt des QR-Codes zurück.
        /// </summary>
        public string Data { get; }
        /// <summary>
        /// Gibt die systemrelative letzte Erkennungszeit zurück.
        /// </summary>
        public TimeSpan SystemRelativeLastDetectedTime { get; }
        /// <summary>
        /// Gibt den Zeitstempel der letzten Erkennung zurück.
        /// </summary>
        public DateTimeOffset LastDetectedTime { get; }

    }
}