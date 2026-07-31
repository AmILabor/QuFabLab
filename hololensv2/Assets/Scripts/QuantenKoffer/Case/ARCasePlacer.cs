/// <summary>
/// Platziert den Quantenkoffer in der AR-Umgebung mithilfe von QR-Code-Scans.
/// Ermittelt die Position des Koffers über zwei QR-Code-Markierungen (unten links, oben rechts).
/// </summary>
using MRTKExtensions.QRCodes;
using UnityEngine;

namespace QuantenKoffer.Case
{
    /// <summary>
    /// Platziert den Quantenkoffer in der AR-Umgebung mithilfe von QR-Code-Scans.
    /// </summary>
    public class ARCasePlacer : MonoBehaviour
    {
        private QRInfo bottomLeftInfo;
        private QRInfo topRightInfo;
        [SerializeField] public GameObject scanningClue;
        [SerializeField] public GameObject bottomLeftPosition;
        [SerializeField] public GameObject topRightPosition;

        /// <summary>
        /// Setzt die QR-Info für die untere linke Ecke und prüft, ob der Scan abgeschlossen ist.
        /// </summary>
        /// <param name="info">QR-Code-Informationen</param>
        public void SetBottomLeftInfo(QRInfo info)
        {
            AMI.Util.Console.Log("ScannedQR-BotLeft", info.PhysicalSideLength);
            bottomLeftInfo = info;
            handleScanningDone();
        }

        /// <summary>
        /// Setzt die QR-Info für die obere rechte Ecke und prüft, ob der Scan abgeschlossen ist.
        /// </summary>
        /// <param name="info">QR-Code-Informationen</param>
        public void SetTopRightInfo(QRInfo info)
        {
            topRightInfo = info;
            AMI.Util.Console.Log("ScannedQR-TopRight", info.PhysicalSideLength);
            handleScanningDone();
        }

        /// <summary>
        /// Reaktiviert den Scanvorgang, indem die gespeicherten QR-Informationen zurückgesetzt werden.
        /// </summary>
        public void ReactivateScanning()
        {
            bottomLeftInfo = null;
            topRightInfo = null;
            scanningClue.SetActive(true);
        }

        /// <summary>
        /// Prüft, ob beide QR-Codes gescannt wurden, und blendet die Scan-Hinweise aus.
        /// </summary>
        private void handleScanningDone()
        {
            if (isScanningDone())
            {
                scanningClue.SetActive(false);
            }
        }

        /// <summary>
        /// Prüft, ob sowohl die untere linke als auch die obere rechte QR-Markierung gescannt wurden.
        /// </summary>
        /// <returns>True, wenn beide QR-Codes gescannt wurden</returns>
        private bool isScanningDone()
        {
            return topRightInfo != null && bottomLeftInfo != null;
        }

        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {
        }
    }
}