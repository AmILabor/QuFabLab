/// <summary>
/// Enthält das Konfigurationsprofil für den QR-Code-Tracking-Dienst.
/// </summary>
using UnityEngine;
using Microsoft.MixedReality.Toolkit;

namespace MRTKExtensions.QRCodes
{
	/// <summary>
	/// Konfigurationsprofil für den QR-Code-Tracking-Dienst.
	/// </summary>
	[MixedRealityServiceProfile(typeof(IQRCodeTrackingService))]
	[CreateAssetMenu(fileName = "QRCodeTrackingServiceProfile", menuName = "MRTKExtensions/QRCodeTrackingService Configuration Profile")]
	public class QRCodeTrackingServiceProfile : BaseMixedRealityProfile
	{
        [SerializeField] 
		[Tooltip("Number of seconds before retrying to get access to the camera")]
        private int accessRetryTime = 5000;
        /// <summary>
        /// Gibt die Anzahl der Sekunden zurück, die vor einem erneuten Zugriffsversuch gewartet wird.
        /// </summary>
        public int AccessRetryTime => accessRetryTime;

        [SerializeField]
        [Tooltip("Expose progress and debug messages")]
        private bool exposedProgressMessages = true;
        /// <summary>
        /// Gibt an, ob Fortschritts- und Debugmeldungen angezeigt werden sollen.
        /// </summary>
        public bool
        ExposedProgressMessages => exposedProgressMessages;

        [SerializeField]
        [Tooltip("Number of debug message lines")]
        private int debugMessages = 10;
        /// <summary>
        /// Gibt die Anzahl der zu speichernden Debugmeldungen zurück.
        /// </summary>
        public int DebugMessages => debugMessages;
    }
}