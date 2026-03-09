using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AMI.Util
{
    /// <summary>
    ///     Display Log messages within a TMPro text element
    /// </summary>
    public class WSDebugMessageEvent : MonoBehaviour
    {
        /// <summary>
        ///     Which type of Log do you want to display
        /// </summary>
        [Tooltip("Which type of Log do you want to display")] [SerializeField]
        private LogType logType;

        /// <summary>
        ///     Also log stackTrace ?
        /// </summary>
        [Tooltip("Also log stackTrace ?")] [SerializeField]
        private bool logStackTrace;

        private WebSocketBridge wsBridge;
        private bool isConnected;
        private List<string> queue = new List<string>();

        private void Awake()
        {
            Application.logMessageReceived += HandleLog;
            wsBridge = GetComponent<WebSocketBridge>();
        }

        public void setConnected(bool connected)
        {
            isConnected = connected;
            if (!isConnected) return;
            while (queue.Count > 0)
            {
                wsBridge.SendWebSocketMessage(queue.First());
                queue.RemoveAt(0);
            }
        }

        /// <summary>
        ///     Callback handler for received Debug messages
        /// </summary>
        private void HandleLog(string logString, string stackTrace, LogType type)
        {
            var msg = logString;
            if (logStackTrace && stackTrace.Length > 0 && type == LogType.Error)
                msg += "_____________________\\n" + stackTrace;
            msg = msg.Replace("\r", "");
            msg = msg.Replace("\n", "");
            var msgob = string.Format(@"{{""message"":""{0}"", ""type"":""{1}""}}", msg.Replace("\"", "'"),
                type.ToString());
            if (!isConnected)
            {
                queue.Add(msgob);
                return;
            }

            wsBridge.SendWebSocketMessage(msgob);
        }
    }
}