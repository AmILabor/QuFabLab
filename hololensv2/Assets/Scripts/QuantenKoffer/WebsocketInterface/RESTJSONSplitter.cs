using UnityEngine;

namespace QuantenKoffer.WebsocketInterface
{
    /// <summary>
    /// Holds the Data which is received via RESTRequest (Variables have to be Named exactly like the received JSON property Names)
    /// Currently for testing purposes only
    /// </summary>  
    [System.Serializable]
    public struct RESTData
    {
        /// <summary>
        /// EXAMPLE CONTENT !! (the Variable names of this class have to exactly match the received JSON data names)
        /// </summary>
        public string origin;

        /// <summary>
        /// Returns this Object as a json string
        /// </summary>
        /// <returns>This Object as JSON string</returns>
        public override string ToString()
        {
            return JsonUtility.ToJson(this);
        }

        /// <summary>
        /// Creates a RESTData Object from a json string
        /// </summary>
        /// <param name="jsonString">json string from which the object should be created</param>
        public static RESTData FromJSON(string jsonString)
        {
            return JsonUtility.FromJson<RESTData>(jsonString);
        }
    }

    public class RESTJSONSplitter : MonoBehaviour
    {
        [SerializeField] RESTData data;

        /// <summary>
        /// Split the given JSON-string to JSONData (only runtime)
        /// </summary>
        /// <param name="json">json string from which the object should be created</param>
        public void Split(string json)
        {
            data = RESTData.FromJSON(json);
        }
    }
}