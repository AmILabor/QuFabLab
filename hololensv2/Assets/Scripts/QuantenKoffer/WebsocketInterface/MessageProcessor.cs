using UnityEngine;
using UnityEngine.Events;

namespace QuantenKoffer.WebsocketInterface
{
    /// <summary>
    /// Processes a string message and calls the corresponding UnityEvent<string>
    /// We use this to split messages received from our WebSocketBridge 
    /// </summary>
    public class MessageProcessor : MonoBehaviour
    {
        /// <summary>
        /// Called when message received is "ACK?"
        /// </summary>
        [SerializeField] UnityEvent<string> askedForACK;

        /// <summary>
        /// Called when message received is not "ACK?"
        /// </summary>
        [SerializeField] UnityEvent<string> processMessage;

        /// <summary>
        /// Processes a string message and calls the corresponding UnityEvent<string>
        /// </summary>
        /// <param name="message">string to be processed</param>
        public void process(string message)
        {
            if (message == "ACK?")
            {
                askedForACK.Invoke(message);
            }
            else
            {
                processMessage.Invoke(message);
            }
        }
    }
}