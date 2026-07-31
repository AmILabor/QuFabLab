/// <summary>
/// Zerlegt JSON-Strings aus WebSocket-Nachrichten in WebsocketData-Objekte und verarbeitet sie.
/// Enthält die Datenstrukturen für WebSocket-Kommunikation und Befehlsverarbeitung.
/// </summary>
using System.Collections.Generic;
using QuantenKoffer.Bricks;
using QuantenKoffer.Case;
using UnityEngine;
using UnityEngine.Events;

namespace QuantenKoffer.WebsocketInterface
{
    /// <summary>
    /// Receivable Commands enumeration
    /// </summary>
    public enum Commands
    {
        place,
        remove,
        settings,
        start
    }

    /// <summary>
    /// Holds the raw Data which is sent via the Websocket (Variables have to be Named exactly like the received JSON property Names)
    /// </summary>  
    [System.Serializable]
    public struct WebsocketData
    {
        /// <summary>
        /// The Command to be executed
        /// </summary>
        public string command;

        public int posX, posY, rotation, type;
        public float value;

        /// <summary>
        /// Returns the Object as a json string
        /// </summary>
        /// <returns>Generated json string</returns>
        public override string ToString()
        {
            return JsonUtility.ToJson(this);
        }

        /// <summary>
        /// Generates a WebsocketData Object from a given json string 
        /// </summary>
        /// <param name="jsonString">WebsocketData in json Format</param>
        /// <returns>Generated WebsocketData</returns>
        public static WebsocketData FromJSON(string jsonString)
        {
            return JsonUtility.FromJson<WebsocketData>(jsonString);
        }
    }

    /// <summary>
    /// Holds the processed Data from WebsocketData
    /// </summary>  
    [System.Serializable]
    public struct WebsocketDataProcessed
    {
        /// <summary>
        /// The Command to be executed
        /// </summary>
        [SerializeField] Commands command;

        /// <summary>
        /// X and Y position of the QuBrick
        /// </summary>
        [SerializeField] int posX, posY;

        /// <summary>
        /// Rotation of the QuBrick
        /// </summary>
        [SerializeField] GridDirection rotation;

        /// <summary>
        /// Type of the QuBrick
        /// </summary>
        [SerializeField] QuBrickType type;

        /// <summary>
        /// Value of the QuBrick
        /// </summary>
        [SerializeField] float value;

        public Commands Command
        {
            get => command;
            set => command = value;
        }

        public int PosX
        {
            get => posX;
            set => posX = value;
        }

        public int PosY
        {
            get => posY;
            set => posY = value;
        }

        public GridDirection Rotation
        {
            get => rotation;
            set => rotation = value;
        }

        public QuBrickType Type
        {
            get => type;
            set => type = value;
        }

        public float Value
        {
            get => value;
            set => this.value = value;
        }

        /// <summary>
        /// Processes the raw Websocket data to a usable format 
        /// </summary>
        /// <param name="data">The data to be processed</param>
        /// <param name="typeList">A List of our QuBrickType SO's to match them to the id of the input type</param>
        /// <returns>The processed websocket data</returns>
        public static WebsocketDataProcessed Process(WebsocketData data, List<QuBrickType> typeList)
        {
            WebsocketDataProcessed processed = new();
            processed.posX = data.posX;
            processed.posY = data.posY;
            processed.value = data.value;
            switch (data.command)
            {
                case "place":
                    processed.command = Commands.place;
                    break;
                case "remove":
                    processed.command = Commands.remove;
                    break;
                case "setting":
                    processed.command = Commands.settings;
                    break;
                case "start":
                    processed.command = Commands.start;
                    break;
            }

            processed.rotation = (GridDirection)data.rotation;
            processed.type = typeList[data.type];

            return processed;
        }
    }

    /// <summary>
    /// Splits a json string to the exakt format our Websocket uses and translates their types to the matching unity types.
    /// Then fires a UnityEvent with the processed data.
    /// </summary>
    public class WebsocketJSONSplitter : MonoBehaviour
    {
        /// <summary>
        /// List of QuBrickType which has to match the exact enum order from the python Websocket
        /// </summary>
        [SerializeField] List<QuBrickType> typeList;

        /// <summary>
        /// Processed Data is stored here
        /// </summary>
        [SerializeField] WebsocketDataProcessed processedData;

        public WebsocketDataProcessed ProcessedData
        {
            get => processedData;
            set => processedData = value;
        }

        /// <summary>
        /// Event which is called when Data is received and was processed
        /// </summary>
        [SerializeField] UnityEvent<WebsocketDataProcessed> onDataReceived;

        /// <summary>
        /// Splits the given JSON-string to WebsocketData and processes it, then propagates the processedData via an inspector configurable UnityEvent (only runtime)
        /// </summary>
        /// <param name="json">The json string to be split</param>
        public void Split(string json)
        {
            processedData = WebsocketDataProcessed.Process(WebsocketData.FromJSON(json), typeList);
            onDataReceived.Invoke(processedData);
        }
    }
}