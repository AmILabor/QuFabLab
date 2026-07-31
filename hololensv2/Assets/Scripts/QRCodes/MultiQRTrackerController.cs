/// <summary>
/// Enthält Klassen zur erweiterten QR-Code-Verfolgung und -Positionierung für MRTK.
/// </summary>
using System;
using System.Collections.Generic;
using Microsoft.MixedReality.Toolkit;
using MRTKExtensions.QRCodes;
using UnityEngine;

namespace MRTKExtensions.QRCodes
{   
    /// <summary>
    /// Steuert die Verfolgung mehrerer QR-Codes und positioniert die zugehörigen Objekte.
    /// </summary>
    public class MultiQRTrackerController : MonoBehaviour
    {
        [SerializeField] private ReferenceQRTextLibrary _referenceQrTextLibrary;

        public EventHandler<Pose> PositionSet;

        private SpatialGraphCoordinateSystemSetter _spatialGraphCoordinateSystemSetter;
        private Transform markerHolder;
        private AudioSource audioSource;
        private GameObject markerDisplay;
        private QRInfo lastMessage;
        private GameObject placeObject;
        private List<GameObject> placedGameObjects = new List<GameObject>();

        /// <summary>
        /// Gibt an, ob das Tracking aktiv ist.
        /// </summary>
        public bool IsTrackingActive { get; private set; } = true;

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
        /// Initialisiert den SpatialGraphCoordinateSystemSetter.
        /// </summary>
        void Awake()
        {
            _spatialGraphCoordinateSystemSetter = GetComponentInChildren<SpatialGraphCoordinateSystemSetter>();
        }

        /// <summary>
        /// Startet das QR-Code-Tracking und abonniert die erforderlichen Ereignisse.
        /// </summary>
        private void Start()
        {
            if (!QRCodeTrackingService.IsSupported)
            {
                return;
            }

            markerHolder = _spatialGraphCoordinateSystemSetter.gameObject.transform;
            markerDisplay = markerHolder.GetChild(0).gameObject;
            markerDisplay.SetActive(false);

            audioSource = markerHolder.gameObject.GetComponent<AudioSource>();

            QRCodeTrackingService.QRCodeFound += ProcessTrackingFound;
            _spatialGraphCoordinateSystemSetter.PositionAcquired += SetPosition;
            _spatialGraphCoordinateSystemSetter.PositionAcquisitionFailed +=
                (s, e) => ResetTracking();


            if (QRCodeTrackingService.IsInitialized)
            {
                StartTracking();
            }
            else
            {
                QRCodeTrackingService.Initialized += QRCodeTrackingService_Initialized;
            }
        }
        /// <summary>
        /// Wird aufgerufen, wenn der QR-Code-Tracking-Dienst initialisiert wurde.
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
        /// Setzt das Tracking zurück und aktiviert die Markierungsanzeige.
        /// </summary>
        public void ResetTracking()
        {
            if (QRCodeTrackingService.IsInitialized)
            {
                markerDisplay.SetActive(false);
                IsTrackingActive = true;
            }
        }

        /// <summary>
        /// Verarbeitet einen gefundenen QR-Code und leitet die Positionierung ein.
        /// </summary>
        /// <param name="sender">Die Ereignisquelle.</param>
        /// <param name="msg">Die Informationen des gefundenen QR-Codes.</param>
        private void ProcessTrackingFound(object sender, QRInfo msg)
        {
            if (msg == null || !IsTrackingActive)
            {
                return;
            }

            lastMessage = msg;

            foreach (var item in _referenceQrTextLibrary.QRContentPrefabs)
            {
                if (msg.Data == item.QRText &&
                    Math.Abs((DateTimeOffset.UtcNow - msg.LastDetectedTime.UtcDateTime).TotalMilliseconds) < 200)
                {
                    _spatialGraphCoordinateSystemSetter.SetLocationIdSize(msg.SpatialGraphNodeId,
                        msg.PhysicalSideLength);
                    placeObject = item.Prefab;
                }
            }
        }

        /// <summary>
        /// Setzt die Position des markierten Objekts und instanziiert das entsprechende Prefab.
        /// </summary>
        /// <param name="sender">Die Ereignisquelle.</param>
        /// <param name="pose">Die zu setzende Position und Rotation.</param>
        private void SetPosition(object sender, Pose pose)
        {
            IsTrackingActive = false;
            markerHolder.localScale = Vector3.one * lastMessage.PhysicalSideLength;

            if (placeObject != null)
            {
                if (GetPlacedObject(placeObject.name) == null)
                {
                    GameObject go = Instantiate(placeObject, markerHolder.position, markerHolder.rotation);
                    go.name = placeObject.name;
                    placedGameObjects.Add(go);
                }
                else
                {
                    GetPlacedObject(placeObject.name).transform.SetPositionAndRotation(markerHolder.position, markerHolder.rotation);
                }
            }
            
            markerDisplay.SetActive(true);
            PositionSet?.Invoke(this, pose);
            audioSource.Play();
        }

        /// <summary>
        /// Sucht ein bereits platziertes Objekt anhand des Namens.
        /// </summary>
        /// <param name="name">Der Name des Objekts.</param>
        /// <returns>Das gefundene GameObject oder null.</returns>
        GameObject GetPlacedObject(string name)
        {
            return placedGameObjects.Find(x => x.name.Equals(name));
        }
    }
}
