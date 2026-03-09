using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Microsoft.MixedReality.QR;

namespace AMI.QRTracking
{
    /// <summary>
    /// QR Code EventArgs
    /// </summary>
    public static class QRCodeEventArgs
    {
        public static QRCodeEventArgs<TData> Create<TData>(TData data)
        {
            return new QRCodeEventArgs<TData>(data);
        }
    }

    [Serializable]
    public class QRCodeEventArgs<TData> : EventArgs
    {
        public TData Data { get; private set; }

        public QRCodeEventArgs(TData data)
        {
            Data = data;
        }
    }

    /// <summary>
    /// Manages detected QR Codes & calls respective Events
    /// </summary>
    public class QRCodesManager : Singleton<QRCodesManager>
    {
        /// <summary>
        /// Determines if the QR codes scanner should be automatically started.
        /// </summary>
        [Tooltip("Determines if the QR codes scanner should be automatically started.")]
        public bool AutoStartQRTracking = true;

        public bool IsTrackerRunning { get; private set; }

        public bool IsSupported { get; private set; }

        /// <summary>
        /// QR Code Changed Event
        /// </summary>
        public event EventHandler<bool> QRCodesTrackingStateChanged;

        /// <summary>
        /// QR Code Added Event
        /// </summary>
        public event EventHandler<QRCodeEventArgs<Microsoft.MixedReality.QR.QRCode>> QRCodeAdded;

        [SerializeField] UnityEvent<QRCodeAddedEventArgs> QRAdded;

        /// <summary>
        /// QR Code Updated Event
        /// </summary>
        public event EventHandler<QRCodeEventArgs<Microsoft.MixedReality.QR.QRCode>> QRCodeUpdated;

        [SerializeField] UnityEvent<QRCodeUpdatedEventArgs> QRUpdated;

        /// <summary>
        /// QR Code Removed Event
        /// </summary>
        public event EventHandler<QRCodeEventArgs<Microsoft.MixedReality.QR.QRCode>> QRCodeRemoved;

        [SerializeField] UnityEvent<QRCodeRemovedEventArgs> QRRemoved;

        /// <summary>
        /// QR Code Dictionary mit <Guid,QR-Code>
        /// </summary>
        private System.Collections.Generic.SortedDictionary<System.Guid, Microsoft.MixedReality.QR.QRCode> qrCodesList =
            new SortedDictionary<System.Guid, Microsoft.MixedReality.QR.QRCode>();

        private QRCodeWatcher qrTracker;
        private bool capabilityInitialized = false;
        private QRCodeWatcherAccessStatus accessStatus;
        private System.Threading.Tasks.Task<QRCodeWatcherAccessStatus> capabilityTask;


        /// <summary>
        /// Get Guid for qrCode
        /// </summary>
        /// <param name="qrCodeData">Qr Code string representation </param>
        /// <returns>Returns a Guid based on the input QRCode</returns>
        public System.Guid GetIdForQRCode(string qrCodeData)
        {
            lock (qrCodesList)
            {
                foreach (var ite in qrCodesList)
                {
                    if (ite.Value.Data == qrCodeData)
                    {
                        return ite.Key;
                    }
                }
            }

            return new System.Guid();
        }

        /// <summary>
        /// Get full QRCode List
        /// </summary>
        /// <returns>List of all QRCodes</returns>
        public System.Collections.Generic.IList<Microsoft.MixedReality.QR.QRCode> GetList()
        {
            lock (qrCodesList)
            {
                return new List<Microsoft.MixedReality.QR.QRCode>(qrCodesList.Values);
            }
        }

        protected void Awake()
        {
        }

        // Use this for initialization
        async protected virtual void Start()
        {
            IsSupported = QRCodeWatcher.IsSupported();
            capabilityTask = QRCodeWatcher.RequestAccessAsync();
            accessStatus = await capabilityTask;
            capabilityInitialized = true;
        }

        /// <summary>
        /// Initial QR Tracking Setup + optional Autostart
        /// </summary>
        private void SetupQRTracking()
        {
            Debug.Log("QRCodesManager SetupQRTracking");
            try
            {
                qrTracker = new QRCodeWatcher();
                IsTrackerRunning = false;
                qrTracker.Added += QRCodeWatcher_Added;
                qrTracker.Updated += QRCodeWatcher_Updated;
                qrTracker.Removed += QRCodeWatcher_Removed;
                qrTracker.EnumerationCompleted += QRCodeWatcher_EnumerationCompleted;
            }
            catch (Exception ex)
            {
                Debug.Log("QRCodesManager : exception starting the tracker " + ex.ToString());
            }

            if (AutoStartQRTracking)
            {
                StartQRTracking();
            }
        }

        /// <summary>
        /// Start QRTracking here after it is correctly set up
        /// </summary>
        public void StartQRTracking()
        {
            //startTime =DateTime.Now;
            if (qrTracker != null && !IsTrackerRunning)
            {
                Debug.Log("QRCodesManager starting QRCodeWatcher");
                try
                {
                    qrTracker.Start();
                    IsTrackerRunning = true;
                    QRCodesTrackingStateChanged?.Invoke(this, true);
                }
                catch (Exception ex)
                {
                    Debug.Log("QRCodesManager starting QRCodeWatcher Exception:" + ex.ToString());
                }
            }
        }

        /// <summary>
        /// Stop QRTracking here
        /// </summary>
        public void StopQRTracking()
        {
            if (IsTrackerRunning)
            {
                IsTrackerRunning = false;
                if (qrTracker != null)
                {
                    qrTracker.Stop();
                    qrCodesList.Clear();
                }

                var handlers = QRCodesTrackingStateChanged;
                if (handlers != null)
                {
                    handlers(this, false);
                }
            }
        }

        /// <summary>
        /// Sets Tracking state
        /// </summary>
        /// <param name="state">should Tracking be activated or deactivated </param>
        public void SetQRTrackingState(bool state)
        {
            if (state == true)
            {
                StartQRTracking();
            }
            else
            {
                StopQRTracking();
            }
        }

        /// <summary>
        /// QR Code Remove-Event Callback
        /// </summary>
        /// <param name="args">QR Code event args </param>
        private void QRCodeWatcher_Removed(object sender, QRCodeRemovedEventArgs args)
        {
            Debug.Log("QRCodesManager QRCodeWatcher_Removed");

            bool found = false;
            lock (qrCodesList)
            {
                if (qrCodesList.ContainsKey(args.Code.Id))
                {
                    qrCodesList.Remove(args.Code.Id);
                    found = true;
                }
            }

            if (found)
            {
                var handlers = QRCodeRemoved;
                QRRemoved?.Invoke(args);
                if (handlers != null)
                {
                    handlers(this, QRCodeEventArgs.Create(args.Code));
                }
            }
        }

        /// <summary>
        /// QR Code Updated-Event Callback
        /// </summary>
        /// <param name="args">QR Code event args </param>
        private void QRCodeWatcher_Updated(object sender, QRCodeUpdatedEventArgs args)
        {
            Debug.Log("QRCodesManager QRCodeWatcher_Updated");
            //if(startTime <= args.Code.LastDetectedTime){
            if (true)
            {
                bool found = false;
                lock (qrCodesList)
                {
                    if (qrCodesList.ContainsKey(args.Code.Id))
                    {
                        found = true;
                        qrCodesList[args.Code.Id] = args.Code;
                    }
                }

                if (found)
                {
                    QRUpdated?.Invoke(args);
                    var handlers = QRCodeUpdated;
                    if (handlers != null)
                    {
                        handlers(this, QRCodeEventArgs.Create(args.Code));
                    }
                }
            }
        }

        /// <summary>
        /// QR Code Added-Event Callback
        /// </summary>
        /// <param name="args">QR Code event args </param>
        private void QRCodeWatcher_Added(object sender, QRCodeAddedEventArgs args)
        {
            Debug.Log("QRCodesManager QRCodeWatcher_Added");
            //if(startTime <= args.Code.LastDetectedTime){
            if (true)
            {
                lock (qrCodesList)
                {
                    qrCodesList[args.Code.Id] = args.Code;
                }

                QRAdded?.Invoke(args);
                var handlers = QRCodeAdded;
                if (handlers != null)
                {
                    handlers(this, QRCodeEventArgs.Create(args.Code));
                }
            }
        }

        private void QRCodeWatcher_EnumerationCompleted(object sender, object e)
        {
            Debug.Log("QRCodesManager QrTracker_EnumerationCompleted");
        }

        private void Update()
        {
            if (qrTracker == null && capabilityInitialized && IsSupported)
            {
                if (accessStatus == QRCodeWatcherAccessStatus.Allowed)
                {
                    SetupQRTracking();
                }
                else
                {
                    Debug.Log("Capability access status : " + accessStatus);
                }
            }
        }
    }
}