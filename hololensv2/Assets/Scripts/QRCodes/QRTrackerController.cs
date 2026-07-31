/// <summary>
/// Enthält die Steuerungsklasse zur Verfolgung einzelner QR-Codes.
/// </summary>
using System;
using Microsoft.MixedReality.QR;
using Microsoft.MixedReality.Toolkit;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace MRTKExtensions.QRCodes
{
    /// <summary>
    /// Steuert die QR-Code-Verfolgung für einen einzelnen QR-Code und löst Positionierungsereignisse aus.
    /// </summary>
    public class QRTrackerController : MonoBehaviour
    {
        [SerializeField] private SpatialGraphCoordinateSystemSetter spatialGraphCoordinateSystemSetter;

        [SerializeField] private string locationQrValue = string.Empty;

        private Transform markerHolder;
        private AudioSource audioSource;
        private GameObject markerDisplay;
        private QRInfo lastMessage;

        public UnityEvent<QRInfo> OnQRScanned;
        public UnityEvent<QRInfo> OnQRLost;
        /// <summary>
        /// Gibt an, ob das Tracking aktiv ist.
        /// </summary>
        public bool IsTrackingActive { get; private set; } = true;

        [SerializeField] private GameObject scanningClue;
        private IQRCodeTrackingService qrCodeTrackingService;

        private IQRCodeTrackingService QRCodeTrackingService
        {
            get
            {
                while (!MixedRealityToolkit.IsInitialized && Time.time < 5) ;
                return qrCodeTrackingService ??
                       (qrCodeTrackingService = MixedRealityToolkit.Instance.GetService<IQRCodeTrackingService>());
            }
        }

        /// <summary>
        /// Aktiviert oder deaktiviert den Scan-Hinweis.
        /// </summary>
        /// <param name="active">Gibt an, ob der Hinweis aktiviert werden soll.</param>
        private void SetScanningClueActive(bool active)
        {
            Debug.Log("Setting scanning clue to  " + active);
            if (scanningClue)
                scanningClue.SetActive(active);
        }

        /// <summary>
        /// Initialisiert den Tracker und abonniert die QR-Code-Ereignisse.
        /// </summary>
        private void Start()
        {
            AMI.Util.Console.Log("Trying to start QRCodeTRackerController");

            if (!QRCodeTrackingService.IsSupported)
            {
                SetScanningClueActive(false);
                return;
            }

            SetScanningClueActive(true);

            markerHolder = spatialGraphCoordinateSystemSetter.gameObject.transform;
            markerDisplay = markerHolder.GetChild(0).gameObject;
            markerDisplay.SetActive(false);
            AMI.Util.Console.Log("Holder/Display set");

            audioSource = markerHolder.gameObject.GetComponent<AudioSource>();

            QRCodeTrackingService.QRCodeFound += ProcessTrackingFound;
            spatialGraphCoordinateSystemSetter.PositionAcquired += SetPosition;
            spatialGraphCoordinateSystemSetter.PositionAcquisitionFailed +=
                (s, e) => ResetTracking();

            AMI.Util.Console.Log("Trying to start stracking.");

            if (QRCodeTrackingService.IsInitialized)
            {
                StartTracking();
            }
            else
            {
                QRCodeTrackingService.Initialized += QRCodeTrackingService_Initialized;
            }

            AMI.Util.Console.Log("QRCodeTRackerController started..");
        }

        /// <summary>
        /// Wird aufgerufen, wenn der Tracking-Dienst initialisiert wurde.
        /// </summary>
        /// <param name="sender">Die Ereignisquelle.</param>
        /// <param name="e">Ereignisdaten.</param>
        private void QRCodeTrackingService_Initialized(object sender, EventArgs e)
        {
            StartTracking();
        }

        /// <summary>
        /// Aktiviert den QR-Code-Tracking-Dienst.
        /// </summary>
        private void StartTracking()
        {
            QRCodeTrackingService.Enable();
        }

        /// <summary>
        /// Setzt das Tracking zurück und aktiviert die Suche erneut.
        /// </summary>
        public void ResetTracking()
        {
            if (QRCodeTrackingService.IsInitialized)
            {
                SetScanningClueActive(true);
                markerDisplay.SetActive(false);
                IsTrackingActive = true;
            }
        }

        /// <summary>
        /// Verarbeitet einen gefundenen QR-Code und prüft, ob der Text mit dem erwarteten Wert übereinstimmt.
        /// </summary>
        /// <param name="sender">Die Ereignisquelle.</param>
        /// <param name="msg">Die Informationen des gefundenen QR-Codes.</param>
        private void ProcessTrackingFound(object sender, QRInfo msg)
        {
            if (msg == null || !IsTrackingActive)
            {
                return;
            }

            AMI.Util.Console.Log("QRCode Found.");

            lastMessage = msg;
            var checkValue = locationQrValue;
            bool hasWildCard = locationQrValue.Contains("*");
            bool qrFound = false;
            if (hasWildCard)
            {
                var startString = locationQrValue.Substring(0, locationQrValue.IndexOf("*"));
                qrFound = msg.Data.StartsWith(startString);
            }
            else
            {
                qrFound = msg.Data == locationQrValue;
            }

            if (qrFound && Math.Abs((DateTimeOffset.UtcNow - msg.LastDetectedTime.UtcDateTime).TotalMilliseconds) < 200)
            {
                spatialGraphCoordinateSystemSetter.SetLocationIdSize(msg.SpatialGraphNodeId,
                    msg.PhysicalSideLength);
                AMI.Util.Console.Log("QRCode Set.");
            }
        }

        /// <summary>
        /// Setzt die Position und Rotation und löst das QRScan-Ereignis aus.
        /// </summary>
        /// <param name="sender">Die Ereignisquelle.</param>
        /// <param name="pose">Die zu setzende Pose.</param>
        private void SetPosition(object sender, Pose pose)
        {
            IsTrackingActive = false;
            SetScanningClueActive(false);
            markerHolder.localScale = Vector3.one * lastMessage.PhysicalSideLength;
            markerDisplay.SetActive(true);
            PositionSet?.Invoke(this, pose);
            audioSource.Play();
            Debug.Log("SetPositionCalled " + lastMessage.LastDetectedTime.ToString());
            Debug.Log("SetPositionCalled " + lastMessage.PhysicalSideLength.ToString());
            //("SetPositionCalled",lastMessage.LastDetectedTime);
            OnQRScanned.Invoke(lastMessage);
        }

        public EventHandler<Pose> PositionSet;
    }
}