/// <summary>
/// Enthält die Implementierung des QR-Code-Tracking-Dienstes für MRTK.
/// </summary>
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.MixedReality.QR;
using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit;
using MRKTExtensions.Utilities;
using UnityEngine;

namespace MRTKExtensions.QRCodes
{
    /// <summary>
    /// Implementierung des QR-Code-Tracking-Dienstes für die Mixed Reality Toolkit-Erweiterung.
    /// </summary>
    [MixedRealityExtensionService(SupportedPlatforms.WindowsUniversal)]
    public class QRCodeTrackingService : BaseExtensionService, IQRCodeTrackingService
    {
        private QRCodeTrackingServiceProfile profile;

        public QRCodeTrackingService(string name, uint priority, BaseMixedRealityProfile profile) : base(name, priority,
            profile)
        {
            this.profile = (QRCodeTrackingServiceProfile)profile;
        }

        public event EventHandler Initialized;
        public event EventHandler<QRInfo> QRCodeFound;
        public event EventHandler<string> ProgressMessageSent;

        public bool InitializationFailed { get; private set; }
        public string ErrorMessage { get; private set; }
        public bool IsSupported { get; private set; }
        public bool IsTracking { get; private set; }
        public bool ServiceIsInitialized { get; private set; }
        public string ProgressMessages { get; private set; }

        private QRCodeWatcher qrTracker;
        private QRCodeWatcherAccessStatus accessStatus;

        private int initializationAttempt = 0;

        private readonly List<string> messageList = new List<string>();


        /// <summary>
        /// Initialisiert den QR-Code-Tracker asynchron.
        /// </summary>
        public override void Initialize()
        {
            _ = InitializeTracker();
        }

        /// <summary>
        /// Initialisiert den QR-Code-Watcher und fordert die erforderliche Berechtigung an.
        /// </summary>
        private async Task InitializeTracker()
        {
            try
            {
                IsSupported = QRCodeWatcher.IsSupported();
                if (IsSupported)
                {
                    SendProgressMessage($"Initializing QR tracker attempt {++initializationAttempt}");

                    var capabilityTask = QRCodeWatcher.RequestAccessAsync();
                    await capabilityTask.AwaitWithTimeout(profile.AccessRetryTime,
                        ProcessTrackerCapabilityReturned,
                        () => _ = InitializeTracker());
                }
                else
                {
                    InitializationFail("QR tracking not supported");
                }
            }
            catch (Exception ex)
            {
                InitializationFail($"QRCodeTrackingService initialization failed: {ex}");
            }
        }

        /// <summary>
        /// Verarbeitet das Ergebnis der Zugriffsberechtigungsanfrage.
        /// </summary>
        /// <param name="ast">Der Status der Zugriffsberechtigung.</param>
        private void ProcessTrackerCapabilityReturned(QRCodeWatcherAccessStatus ast)
        {
            if (ast != QRCodeWatcherAccessStatus.Allowed)
            {
                InitializationFail($"QR tracker could not be initialized: {ast}");
            }

            accessStatus = ast;
        }

        /// <summary>
        /// Wird pro Frame aufgerufen und richtet das Tracking ein, sobald die Berechtigung vorliegt.
        /// </summary>
        public override void Update()
        {
            if (qrTracker == null && accessStatus == QRCodeWatcherAccessStatus.Allowed)
            {
                SetupTracking();
            }
        }

        /// <summary>
        /// Richtet den QR-Code-Watcher ein und meldet die Initialisierung.
        /// </summary>
        private void SetupTracking()
        {
            qrTracker = new QRCodeWatcher();
            qrTracker.Updated += QRCodeWatcher_Updated;
            ServiceIsInitialized = true;
            Initialized?.Invoke(this, new EventArgs());
            SendProgressMessage("QR tracker initialized");
        }

        /// <summary>
        /// Wird aufgerufen, wenn ein QR-Code aktualisiert wurde, und löst das QRCodeFound-Ereignis aus.
        /// </summary>
        /// <param name="sender">Die Ereignisquelle.</param>
        /// <param name="e">Ereignisdaten mit dem aktualisierten QR-Code.</param>
        private void QRCodeWatcher_Updated(object sender, QRCodeUpdatedEventArgs e)
        {
            SendProgressMessage($"Found QR code {e.Code.Data}");
            QRCodeFound?.Invoke(this, new QRInfo(e.Code));
        }

        /// <summary>
        /// Aktiviert das QR-Code-Tracking.
        /// </summary>
        public override void Enable()
        {
            base.Enable();
            if (!ServiceIsInitialized)
            {
                return;
            }

            try
            {
                qrTracker.Start();
                IsTracking = true;
                SendProgressMessage("Enabled tracking");
            }
            catch (Exception ex)
            {
                InitializationFail($"QRCodeTrackingService starting QRCodeWatcher Exception: {ex}");
            }
        }

        /// <summary>
        /// Deaktiviert das QR-Code-Tracking.
        /// </summary>
        public override void Disable()
        {
            base.Disable();
            if (IsTracking)
            {
                IsTracking = false;
                qrTracker?.Stop();
                SendProgressMessage("Disabled tracking");
            }
        }

        /// <summary>
        /// Behandelt einen Initialisierungsfehler und setzt die entsprechenden Statusvariablen.
        /// </summary>
        /// <param name="message">Die Fehlermeldung.</param>
        private void InitializationFail(string message)
        {
            SendProgressMessage(message);
            ErrorMessage = message;
            InitializationFailed = true;
        }

        /// <summary>
        /// Sendet eine Fortschrittsmeldung, falls das Profil dies zulässt.
        /// </summary>
        /// <param name="msg">Die zu sendende Nachricht.</param>
        private void SendProgressMessage(string msg)
        {
            if (!profile.ExposedProgressMessages)
            {
                return;
            }

            Debug.Log(msg);
            messageList.Add(msg);
            if (messageList.Count > profile.DebugMessages)
            {
                messageList.RemoveAt(0);
            }

            ProgressMessages = string.Join(Environment.NewLine, messageList.AsEnumerable().Reverse());

            ProgressMessageSent?.Invoke(this, ProgressMessages);
        }
    }
}