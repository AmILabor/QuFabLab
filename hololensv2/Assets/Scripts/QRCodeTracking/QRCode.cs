using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if WINDOWS_UWP
using Windows.Perception.Spatial;

#endif

namespace AMI.QRTracking
{
    /// <summary>
    /// Set and update QR Code data for concrete QR Code Object
    /// </summary>
    [RequireComponent(typeof(SpatialGraphCoordinateSystem))]
    public class QRCode : MonoBehaviour
    {
        /// <summary>
        /// QR Code Data
        /// </summary>
        [Tooltip("QR Code Data")] public Microsoft.MixedReality.QR.QRCode qrCode;

        /// <summary>
        /// Object to scale QR Code
        /// </summary>
        [Tooltip("Object to scale QR Code")] [SerializeField]
        private GameObject qrCodeCube;

        /// <summary>
        /// physical Sidelength of rectangular QR Code
        /// </summary>
        //[Tooltip("physical Sidelength of rectangular QR Code")]
        public float PhysicalSize { get; private set; }

        /// <summary>
        /// QR Code content
        /// </summary>
        //[Tooltip("QR Code content")]
        public string CodeText { get; private set; }

        /// <summary>
        /// QR Code ID value field
        /// </summary>
        [Tooltip("QR Code ID value field")] [SerializeField]
        private TMPro.TMP_Text QRID;

        /// <summary>
        /// QR Code Node-ID value field
        /// </summary>
        [Tooltip("QR Code Node-ID value field")] [SerializeField]
        private TMPro.TMP_Text QRNodeID;

        /// <summary>
        /// QR Code content value field
        /// </summary>
        [Tooltip("QR Code content value field")] [SerializeField]
        private TMPro.TMP_Text QRText;

        /// <summary>
        /// QR Code Version value field
        /// </summary>
        [Tooltip("QR Code Version value field")] [SerializeField]
        private TMPro.TMP_Text QRVersion;

        /// <summary>
        /// QR Code timestamp value field
        /// </summary>
        [Tooltip("QR Code timestamp value field")] [SerializeField]
        private TMPro.TMP_Text QRTimeStamp;

        /// <summary>
        /// QR Code size value field
        /// </summary>
        [Tooltip("QR Code size value field")] [SerializeField]
        private TMPro.TMP_Text QRSize;

        /// <summary>
        /// QR value fields parent
        /// </summary>
        [Tooltip("QR value fields parent")] [SerializeField]
        private GameObject QRInfo;

        /// <summary>
        /// log Debug Messages?
        /// </summary>
        [Tooltip("log Debug Messages?")] [SerializeField]
        bool log = false;

        private bool validURI = false;
        private bool launch = false;
        private System.Uri uriResult;
        private long lastTimeStamp = 0;

        // Use this for initialization
        void Start()
        {
            PhysicalSize = 0.1f;
            CodeText = "Dummy";
            if (qrCode == null)
            {
                throw new System.Exception("QR Code Empty");
            }

            PhysicalSize = qrCode.PhysicalSideLength;
            CodeText = qrCode.Data;

            QRID.text = "Id:" + qrCode.Id.ToString();
            QRNodeID.text = "NodeId:" + qrCode.SpatialGraphNodeId.ToString();
            QRText.text = CodeText;


            if (System.Uri.TryCreate(CodeText, System.UriKind.Absolute, out uriResult))
            {
                validURI = true;
                QRText.color = Color.blue;
            }

            QRVersion.text = "Ver: " + qrCode.Version;
            QRSize.text = "Size:" + qrCode.PhysicalSideLength.ToString("F04") + "m";
            QRTimeStamp.text = "Time:" + qrCode.LastDetectedTime.ToString("MM/dd/yyyy HH:mm:ss.fff");
            QRTimeStamp.color = Color.yellow;
            if (log)
                Debug.Log("Id= " + qrCode.Id + "NodeId= " + qrCode.SpatialGraphNodeId + " PhysicalSize = " +
                          PhysicalSize + " TimeStamp = " + qrCode.SystemRelativeLastDetectedTime.Ticks +
                          " QRVersion = " + qrCode.Version + " QRData = " + CodeText);
        }

        void UpdatePropertiesDisplay()
        {
            // Update properties that change
            if (qrCode != null && lastTimeStamp != qrCode.SystemRelativeLastDetectedTime.Ticks)
            {
                QRSize.text = "Size:" + qrCode.PhysicalSideLength.ToString("F04") + "m";

                QRTimeStamp.text = "Time:" + qrCode.LastDetectedTime.ToString("MM/dd/yyyy HH:mm:ss.fff");
                QRTimeStamp.color = QRTimeStamp.color == Color.yellow ? Color.white : Color.yellow;
                PhysicalSize = qrCode.PhysicalSideLength;
                if (log)
                    Debug.Log("Id= " + qrCode.Id + "NodeId= " + qrCode.SpatialGraphNodeId + " PhysicalSize = " +
                              PhysicalSize + " TimeStamp = " + qrCode.SystemRelativeLastDetectedTime.Ticks +
                              " Time = " + qrCode.LastDetectedTime.ToString("MM/dd/yyyy HH:mm:ss.fff"));

                if (qrCodeCube)
                {
                    qrCodeCube.transform.localPosition = new Vector3(PhysicalSize / 2.0f, PhysicalSize / 2.0f, 0.0f);
                    qrCodeCube.transform.localScale = new Vector3(PhysicalSize, PhysicalSize, 0.005f);
                }

                lastTimeStamp = qrCode.SystemRelativeLastDetectedTime.Ticks;
                QRInfo.transform.localScale =
                    new Vector3(PhysicalSize / 0.2f, PhysicalSize / 0.2f, PhysicalSize / 0.2f);
            }
        }

        // Update is called once per frame
        void Update()
        {
            UpdatePropertiesDisplay();
            if (launch)
            {
                launch = false;
                LaunchUri();
            }
        }

        void LaunchUri()
        {
#if WINDOWS_UWP
            // Launch the URI
            UnityEngine.WSA.Launcher.LaunchUri(uriResult.ToString(), true);
#endif
        }

        public void OnInputClicked()
        {
            if (validURI)
            {
                launch = true;
            }
// eventData.Use(); // Mark the event as used, so it doesn't fall through to other handlers.
        }
    }
}