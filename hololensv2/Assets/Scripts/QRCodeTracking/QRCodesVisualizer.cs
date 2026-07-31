/// <summary>
/// Enthält Klassen zur Verwaltung und Visualisierung von QR-Codes.
/// </summary>
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Events;
using Microsoft.MixedReality.QR;

namespace AMI.QRTracking
{
    /// <summary>
    /// Creates & Updates QR Code Objects per Events
    /// </summary>
    public class QRCodesVisualizer : MonoBehaviour
    {
        /// <summary>
        /// Qr Code Prefab for Instantiating
        /// </summary>
        [Tooltip("Qr Code Prefab for Instantiating")]
        public GameObject qrCodePrefab;

        /// <summary>
        /// QRCode scaling factor
        /// </summary>
        [Tooltip("QRCode scaling factor")] public float qrScale = 10f;

        [SerializeField] UnityEvent<Microsoft.MixedReality.QR.QRCode> qrAdded;
        [SerializeField] UnityEvent<Microsoft.MixedReality.QR.QRCode> qrRemoved;
        [SerializeField] UnityEvent<Microsoft.MixedReality.QR.QRCode> qrUpdated;

        /// <summary>
        /// List of QRCodes
        /// </summary>
        private System.Collections.Generic.SortedDictionary<System.Guid, GameObject> qrCodesObjectsList;

        /// <summary>
        /// Clear List of QrCodes?
        /// </summary>
        [SerializeField] private bool clearExisting = false;


        /// <summary>
        /// Possible QrCode Actions with Data
        /// </summary>
        struct ActionData
        {
            /// <summary>
            /// Mögliche Arten von QR-Code-Aktionen.
            /// </summary>
            public enum Type
            {
                Added,
                Updated,
                Removed
            };

            /// <summary>
            /// Type of action
            /// </summary>
            [Tooltip("Type of action")] public Type type;

            /// <summary>
            /// Qr Code Data
            /// </summary>
            [Tooltip("Qr Code Data")] public Microsoft.MixedReality.QR.QRCode qrCode;

            /// <summary>
            /// Create ActionData
            /// </summary>
            /// <param name="type">Qr Code Action type </param>
            /// <param name="qRCode">Qr Code Data </param>
            public ActionData(Type type, Microsoft.MixedReality.QR.QRCode qRCode) : this()
            {
                this.type = type;
                qrCode = qRCode;
            }
        }

        private System.Collections.Generic.Queue<ActionData> pendingActions = new Queue<ActionData>();

        /// <summary>
/// Wird beim Laden der Instanz aufgerufen. Initialisiert Komponenten.
/// </summary>
void Awake()
        {
        }

        /// <summary>
/// Initialisiert den QR-Code-Visualisierer und abonniert Ereignisse des QRCodesManager.
/// </summary>
        void Start()
        {
            Debug.Log("QRCodesVisualizer start");
            qrCodesObjectsList = new SortedDictionary<System.Guid, GameObject>();
            QRCodesManager.Instance.QRCodesTrackingStateChanged += Instance_QRCodesTrackingStateChanged;
            QRCodesManager.Instance.QRCodeAdded += Instance_QRCodeAdded;
            QRCodesManager.Instance.QRCodeUpdated += Instance_QRCodeUpdated;
            QRCodesManager.Instance.QRCodeRemoved += Instance_QRCodeRemoved;
            if (qrCodePrefab == null)
            {
                throw new System.Exception("Prefab not assigned");
            }
        }

        /// <summary>
        /// Set clearExisting flag
        /// </summary>
        public void ClearExisting()
        {
            clearExisting = true;
        }

        /// <summary>
        /// Behandelt die Änderung des QR-Code-Tracking-Status.
        /// </summary>
        /// <param name="sender">Die Ereignisquelle.</param>
        /// <param name="status">Gibt an, ob das Tracking läuft.</param>
        private void Instance_QRCodesTrackingStateChanged(object sender, bool status)
        {
            Debug.Log("QRCodesVisualizer Instance_QRCodesTrackingStateChanged");
            if (!status)
            {
                clearExisting = true;
            }
        }

        /// <summary>
        /// Behandelt das Hinzufügen eines neuen QR-Codes.
        /// </summary>
        /// <param name="sender">Die Ereignisquelle.</param>
        /// <param name="e">Ereignisdaten mit dem hinzugefügten QR-Code.</param>
        private void Instance_QRCodeAdded(object sender, QRCodeEventArgs<Microsoft.MixedReality.QR.QRCode> e)
        {
            Debug.Log("QRCodesVisualizer Instance_QRCodeAdded");
            if (true)
            {
                lock (pendingActions)
                {
                    pendingActions.Enqueue(new ActionData(ActionData.Type.Added, e.Data));
                }
            }
        }

        /// <summary>
        /// Behandelt die Aktualisierung eines vorhandenen QR-Codes.
        /// </summary>
        /// <param name="sender">Die Ereignisquelle.</param>
        /// <param name="e">Ereignisdaten mit dem aktualisierten QR-Code.</param>
        private void Instance_QRCodeUpdated(object sender, QRCodeEventArgs<Microsoft.MixedReality.QR.QRCode> e)
        {
            Debug.Log("QRCodesVisualizer Instance_QRCodeUpdated");
            if (true)
            {
                lock (pendingActions)
                {
                    pendingActions.Enqueue(new ActionData(ActionData.Type.Updated, e.Data));
                }
            }
        }

        /// <summary>
        /// Behandelt das Entfernen eines QR-Codes.
        /// </summary>
        /// <param name="sender">Die Ereignisquelle.</param>
        /// <param name="e">Ereignisdaten mit dem entfernten QR-Code.</param>
        private void Instance_QRCodeRemoved(object sender, QRCodeEventArgs<Microsoft.MixedReality.QR.QRCode> e)
        {
            Debug.Log("QRCodesVisualizer Instance_QRCodeRemoved");

            lock (pendingActions)
            {
                pendingActions.Enqueue(new ActionData(ActionData.Type.Removed, e.Data));
            }
        }

        /// <summary>
        /// Verarbeitet die ausstehenden QR-Code-Aktionen (Hinzufügen, Aktualisieren, Entfernen).
        /// </summary>
        private void HandleEvents()
        {
            lock (pendingActions)
            {
                while (pendingActions.Count > 0)
                {
                    //Debug.Log("QRCodesVisualizer Update: pendingactions > 0");
                    var action = pendingActions.Dequeue();
                    if (action.type == ActionData.Type.Added)
                    {
                        //Debug.Log("Add");
                        qrAdded.Invoke(action.qrCode);
                        GameObject qrCodeObject = Instantiate(qrCodePrefab, new Vector3(0, 0, 0), Quaternion.identity);
                        qrCodeObject.GetComponent<SpatialGraphCoordinateSystem>().Id = action.qrCode.SpatialGraphNodeId;
                        qrCodeObject.GetComponent<QRCode>().qrCode = action.qrCode;
                        qrCodeObject.transform.localScale = new Vector3(action.qrCode.PhysicalSideLength * qrScale,
                            action.qrCode.PhysicalSideLength * qrScale, 1);
                        qrCodesObjectsList.Add(action.qrCode.Id, qrCodeObject);
                    }
                    else if (action.type == ActionData.Type.Updated)
                    {
                        if (!qrCodesObjectsList.ContainsKey(action.qrCode.Id))
                        {
                            //Debug.Log("Update");
                            qrUpdated.Invoke(action.qrCode);
                            GameObject qrCodeObject =
                                Instantiate(qrCodePrefab, new Vector3(0, 0, 0), Quaternion.identity);
                            qrCodeObject.GetComponent<SpatialGraphCoordinateSystem>().Id =
                                action.qrCode.SpatialGraphNodeId;
                            qrCodeObject.GetComponent<QRCode>().qrCode = action.qrCode;
                            qrCodeObject.transform.localScale = new Vector3(action.qrCode.PhysicalSideLength * qrScale,
                                action.qrCode.PhysicalSideLength * qrScale, 1);
                            qrCodesObjectsList.Add(action.qrCode.Id, qrCodeObject);
                        }
                    }
                    else if (action.type == ActionData.Type.Removed)
                    {
                        //Debug.Log("Delete");
                        if (qrCodesObjectsList.ContainsKey(action.qrCode.Id))
                        {
                            qrRemoved.Invoke(action.qrCode);
                            Destroy(qrCodesObjectsList[action.qrCode.Id]);
                            qrCodesObjectsList.Remove(action.qrCode.Id);
                        }
                    }
                }
            }

            if (clearExisting)
            {
                clearExisting = false;
                foreach (var obj in qrCodesObjectsList)
                {
                    Destroy(obj.Value);
                }

                qrCodesObjectsList.Clear();
            }
        }

        /// <summary>
        /// Wird pro Frame aufgerufen und verarbeitet anstehende QR-Code-Aktionen.
        /// </summary>
        void Update()
        {
            HandleEvents();
        }

        /// <summary>
        /// Testmethode zum Auslösen des QRAdded-Ereignisses über das Kontextmenü.
        /// </summary>
        [ContextMenu("Test")]
        public void Test()
        {
            qrAdded.Invoke(null);
        }
    }
}