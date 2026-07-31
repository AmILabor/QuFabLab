/// <summary>
/// Benutzerdefinierter Debug-Logger mit farbcodierten Kategorien.
/// Bietet Log-, Warnungs- und Fehlerausgaben mit optionalem GameObject-Kontext.
/// </summary>
using System;
using System.Diagnostics;
using UnityEngine;

namespace AMI.Util
{
    /// <summary>
    /// Custom Debug logger with color coded Categories. Wrapper der Farben dranhängt. Eigentlich zum Filtern von
    /// Nachrichten gedacht. Diese Funktionalität muss noch.
    /// </summary>
    public static class Console
    {
        private const string infoColor = nameof(Color.white);
        private const string warningColor = nameof(Color.yellow);
        private const string errorColor = nameof(Color.red);

        [Conditional("DEBUG")]
        public static void Log(object message)
        {
            UnityEngine.Debug.Log(FormatMessage(infoColor, message));
        }

        [Conditional("DEBUG")]
        public static void Log(object message, GameObject context)
        {
            UnityEngine.Debug.Log(FormatMessage(infoColor, message), context);
        }

        [Conditional("DEBUG")]
        public static void Log(string category, object message)
        {
            UnityEngine.Debug.Log(FormatMessageWithCategory(infoColor, category, message));
        }

        [Conditional("DEBUG")]
        public static void Log(string category, object message, GameObject context)
        {
            UnityEngine.Debug.Log(FormatMessageWithCategory(infoColor, category, message),context);
        }

        [Conditional("DEBUG")]
        public static void LogFormat(string format, params object[] args)
        {
            UnityEngine.Debug.Log(FormatMessage(infoColor, string.Format(format, args)));
        }

        [Conditional("DEBUG")]
        public static void LogFormat(string format, GameObject context , params object[] args)
        {
            UnityEngine.Debug.Log(FormatMessage(infoColor, string.Format(format, args)), context);
        }

        [Conditional("DEBUG")]
        public static void LogFormat(string category, string format, params object[] args)
        {
            UnityEngine.Debug.Log(FormatMessageWithCategory(infoColor, category, string.Format(format, args)));
        }

        [Conditional("DEBUG")]
        public static void LogFormat(string category, string format, GameObject context , params object[] args)
        {
            UnityEngine.Debug.Log(FormatMessageWithCategory(infoColor, category, string.Format(format, args)), context);
        }

        [Conditional("DEBUG")]
        public static void LogWarning(object message)
        {
            UnityEngine.Debug.LogWarning(FormatMessage(warningColor, message));
        }

        [Conditional("DEBUG")]
        public static void LogWarning(object message, GameObject context)
        {
            UnityEngine.Debug.LogWarning(FormatMessage(warningColor, message), context);
        }

        [Conditional("DEBUG")]
        public static void LogWarning(string category, object message)
        {
            UnityEngine.Debug.LogWarning(FormatMessageWithCategory(warningColor, category, message));
        }

        [Conditional("DEBUG")]
        public static void LogWarning(string category, object message, GameObject context)
        {
            UnityEngine.Debug.LogWarning(FormatMessageWithCategory(warningColor, category, message), context);
        }

        [Conditional("DEBUG")]
        public static void LogWarningFormat(string format, params object[] args)
        {
            UnityEngine.Debug.LogWarningFormat(FormatMessage(warningColor, string.Format(format, args)));
        }

        [Conditional("DEBUG")]
        public static void LogWarningFormat(string format, GameObject context, params object[] args)
        {
            UnityEngine.Debug.LogWarningFormat(FormatMessage(warningColor, string.Format(format, args)), context);
        }

        [Conditional("DEBUG")]
        public static void LogWarningFormat(string category, string format, params object[] args)
        {
            UnityEngine.Debug.LogWarningFormat(FormatMessageWithCategory(warningColor, category, string.Format(format, args)));
        }

        [Conditional("DEBUG")]
        public static void LogWarningFormat(string category, string format, GameObject context, params object[] args)
        {
            UnityEngine.Debug.LogWarningFormat(FormatMessageWithCategory(warningColor, category, string.Format(format, args)), context);
        }

        [Conditional("DEBUG")]
        public static void LogError(object message)
        {
            UnityEngine.Debug.LogError(FormatMessage(errorColor, message));
        }

        [Conditional("DEBUG")]
        public static void LogError(object message, GameObject context)
        {
            UnityEngine.Debug.LogError(FormatMessage(errorColor, message), context);
        }

        [Conditional("DEBUG")]
        public static void LogError(string category, object message)
        {
            UnityEngine.Debug.LogError(FormatMessageWithCategory(errorColor, category, message));
        }

        [Conditional("DEBUG")]
        public static void LogError(string category, object message, GameObject context)
        {
            UnityEngine.Debug.LogError(FormatMessageWithCategory(errorColor, category, message), context);
        }

        [Conditional("DEBUG")]
        public static void LogErrorFormat(string format, params object[] args)
        {
            UnityEngine.Debug.LogErrorFormat(FormatMessage(errorColor, string.Format(format, args)));
        }

        [Conditional("DEBUG")]
        public static void LogErrorFormat(string format, GameObject context, params object[] args)
        {
            UnityEngine.Debug.LogErrorFormat(FormatMessage(errorColor, string.Format(format, args)), context);
        }

        [Conditional("DEBUG")]
        public static void LogErrorFormat(string category, string format, params object[] args)
        {
            UnityEngine.Debug.LogErrorFormat(FormatMessageWithCategory(errorColor, category, string.Format(format, args)));
        }

        [Conditional("DEBUG")]
        public static void LogErrorFormat(string category, string format, GameObject context, params object[] args)
        {
            UnityEngine.Debug.LogErrorFormat(FormatMessageWithCategory(errorColor, category, string.Format(format, args)), context);
        }

        [Conditional("DEBUG")]
        public static void LogException(Exception exception)
        {
            UnityEngine.Debug.LogError(FormatMessage(errorColor, exception.Message));
        }

        [Conditional("DEBUG")]
        public static void LogException(Exception exception, GameObject context)
        {
            UnityEngine.Debug.LogError(FormatMessage(errorColor, exception.Message), context);
        }

        [Conditional("DEBUG")]
        public static void LogException(string category, Exception exception)
        {
            UnityEngine.Debug.LogError(FormatMessageWithCategory(errorColor, category, exception.Message));
        }

        [Conditional("DEBUG")]
        public static void LogException(string category, Exception exception, GameObject context)
        {
            UnityEngine.Debug.LogError(FormatMessageWithCategory(errorColor, category, exception.Message), context);
        }

        private static string FormatMessage(string color, object message)
        {
            return $"<color={color}></color>{message}";
        }

        private static string FormatMessageWithCategory(string color, string category, object message)
        {
            return $"<color={color}><b>[{category}]</b></color> {message}";
        }
    }
}