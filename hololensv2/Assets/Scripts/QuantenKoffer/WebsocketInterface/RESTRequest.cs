using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

namespace QuantenKoffer.WebsocketInterface
{
    /// <summary>
    /// Makes a Http GET Request to a given URl and fires a UnityEvent<string> on receiving an answer
    /// </summary>
    public class RESTRequest : MonoBehaviour
    {
        /// <summary>
        /// Request Url
        /// </summary>
        [SerializeField] string url;

        public string Url
        {
            get => url;
            set => url = value;
        }

        /// <summary>
        /// The Event which is called when an answer is received (with the answer as a param)
        /// </summary>
        [SerializeField] UnityEvent<string> onReceiveMessage;

        /// <summary>
        /// Send the GET Request
        /// </summary>
        [ContextMenu("Request")]
        public void GenerateRequest()
        {
            StartCoroutine(ProcessRequest(url));
        }

        /// <summary>
        /// Sends and waits for an answer for a GET Request to an address
        /// </summary>
        /// <param name="uri">GET request target uri</param>
        private IEnumerator ProcessRequest(string uri)
        {
            AMI.Util.Console.Log("REST", $"Starting Get Request to {uri}", gameObject);
            using (UnityWebRequest request = UnityWebRequest.Get(uri))
            {
                yield return request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.ConnectionError)
                {
                    AMI.Util.Console.LogError("REST", $"Error: {request.error}", gameObject);
                }
                else
                {
                    AMI.Util.Console.Log("REST", $"Received data {request.downloadHandler.text}", gameObject);
                    onReceiveMessage.Invoke(request.downloadHandler.text);
                }
            }
        }
    }
}