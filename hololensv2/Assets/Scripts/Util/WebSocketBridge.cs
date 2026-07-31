/// <summary>
/// Stellt eine WebSocket-Verbindung her und ermöglicht das Senden und Empfangen von Nachrichten.
/// Unterstützt Verbindungsaufbau über IP-Adresse (auch aus QR-Codes) und automatische Wiederverbindung.
/// </summary>
using MRTKExtensions.QRCodes;
using UnityEngine.Events;
using UnityEngine;
using NativeWebSocket;


namespace AMI.Util
{
    /// <summary>
    /// Connect to a Websocket + send/receive Messages
    /// </summary>
    class WebSocketBridge : MonoBehaviour
    {
        /// <summary>
        /// ip to connect to
        /// </summary>
        [Tooltip("ip to connect to")] [SerializeField]
        string ip = "127.0.0.1:10000";

        /// <summary>
        /// connect on Start?
        /// </summary>
        [Tooltip("connect on Start?")] [SerializeField]
        bool connectOnStart = false;

        /// <summary>
        /// use wss communication default is ws (WIP)
        /// </summary>
        [Tooltip("use wss instead of ws protocoll")] [SerializeField]
        bool useWss = false;

        /// <summary>
        /// StringEvent raised when a Message is received
        /// </summary>
        [Tooltip("StringEvent raised when a Message is received")] [SerializeField]
        UnityEvent<string> messageReceived;

        /// <summary>
        /// state of the Connection
        /// </summary>
        [Tooltip("state of the Connection changed")] [SerializeField]
        UnityEvent<bool> isConnected;

        /// <summary>
        /// a string that is not outputted within the debugger, otherwise the Console is flooded with 'ACK?' messages
        /// </summary>
        [Tooltip("a string that should not be outputted within the console to avoid overloading")] [SerializeField]
        string loggerIgnoreString = "ACK?";

        bool connected = false;
        WebSocket websocket;

        /// <summary>
        /// Manualy start connection
        /// </summary>
        [ContextMenu("Connect")]
        public void Connect()
        {
            ConnectTo(ip);
        }

        public void SetConnectionIp(QRInfo info)
        {
            this.ip = info.Data;
            Debug.Log("[Websocket] Setting Connection ip to " + info.Data);
        }

        /// <summary>
        /// Set ip and connect to it
        /// </summary>
        /// <param name="ip">ip to be connected to</param>
        public void ConnectTo(string ip)
        {
            if (ip.Substring(0, 5) == "ws://")
                ip = ip.Substring(5);
            this.ip = ip;
            Debug.Log("[Websocket] Connecting to " + ip.ToString());
            StartConnection();
        }

        /// <summary>
        /// Sets Certificate Policy to accept all(otherwise connecting fails due to self signed Certificate)
        /// </summary>
        private void Awake()
        {
            //ServicePointManager.CertificatePolicy = new CertificateValidator(); // obsolete

            // WARNING : this should only be used for testing purposes a real certificate validation has to be implemented for production
            /* ServicePointManager.ServerCertificateValidationCallback =
                (sender, certificate, chain, sslPolicyErrors) => { return true; }; */

            if (connectOnStart)
            {
                ConnectTo(ip);
            }
        }

        /// <summary>
        /// async: start connecting
        /// </summary>
        public async void StartConnection()
        {
            if (websocket != null && websocket.State == WebSocketState.Open)
            {
                Debug.Log("[Websocket] Closing current Connection");
                await websocket.Close();
            }

            if (!useWss)
            {
                websocket = new WebSocket($"ws://{ip}");
            }
            else
            {
                websocket = new WebSocket($"wss://{ip}");
            }

            Debug.Log("[Websocket] Trying to start connection to" + ip.ToString());

            websocket.OnOpen += () =>
            {
                Debug.Log("[Websocket] Connection open!");
                connected = true;
                isConnected.Invoke(true);
            };

            websocket.OnError += (e) => { Debug.Log("[Websocket] Error: " + e.ToString()); };

            websocket.OnClose += (e) =>
            {
                Debug.Log("[Websocket] Connection closed!");
                connected = false;
                isConnected.Invoke(false);
            };

            websocket.OnMessage += (bytes) =>
            {
                // getting the message as a string
                var message = System.Text.Encoding.UTF8.GetString(bytes);
                if (message != loggerIgnoreString)
                {
                    Debug.Log("[Websocket] Received Message: " + message.ToString());
                }

                messageReceived.Invoke(message);
            };
            // waiting for messages
            await websocket.Connect();
        }

        [ContextMenu("Disconnect")]
        async public void Disconnect()
        {
            await websocket.Close();
        }

        void Update()
        {
#if !UNITY_WEBGL || UNITY_EDITOR
            if (websocket != null && websocket.State == WebSocketState.Open)
            {
                websocket.DispatchMessageQueue();
            }
#endif
        }

        /// <summary>
        /// async: send message to connected Socket
        /// </summary>
        /// <param name="text">message to be sent</param>
        public async void SendWebSocketMessage(string text)
        {
            if (websocket != null && websocket.State == WebSocketState.Open)
            {
                // Sending plain text
                await websocket.SendText(text);
            }
        }

        private async void OnApplicationQuit()
        {
            if (connected)
            {
                await websocket.Close();
                connected = false;
                isConnected.Invoke(connected);
            }
        }
    }
}