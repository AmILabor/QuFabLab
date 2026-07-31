/// <summary>
/// Enthält die Schnittstellendefinition für den QR-Code-Tracking-Dienst.
/// </summary>
using System;
using Microsoft.MixedReality.Toolkit;

namespace MRTKExtensions.QRCodes
{
    /// <summary>
    /// Schnittstelle für den QR-Code-Tracking-Dienst.
    /// </summary>
    public interface IQRCodeTrackingService : IMixedRealityExtensionService
    {
        /// <summary>
        /// Tritt ein, wenn der Dienst initialisiert wurde.
        /// </summary>
        event EventHandler Initialized;
        /// <summary>
        /// Tritt ein, wenn eine Fortschrittsmeldung gesendet wird.
        /// </summary>
        event EventHandler<string> ProgressMessageSent;
        /// <summary>
        /// Tritt ein, wenn ein QR-Code gefunden wurde.
        /// </summary>
        event EventHandler<QRInfo> QRCodeFound;
        /// <summary>
        /// Gibt an, ob die Initialisierung fehlgeschlagen ist.
        /// </summary>
        bool InitializationFailed { get;}
        /// <summary>
        /// Gibt die Fehlermeldung bei einem Initialisierungsfehler zurück.
        /// </summary>
        string ErrorMessage { get; }
        /// <summary>
        /// Gibt die Fortschrittsmeldungen zurück.
        /// </summary>
        string ProgressMessages { get; }
        /// <summary>
        /// Gibt an, ob die QR-Code-Erkennung unterstützt wird.
        /// </summary>
        bool IsSupported { get; }
        /// <summary>
        /// Gibt an, ob derzeit getrackt wird.
        /// </summary>
        bool IsTracking { get; }
        /// <summary>
        /// Gibt an, ob der Dienst initialisiert wurde.
        /// </summary>
        bool IsInitialized { get; }
    }
}