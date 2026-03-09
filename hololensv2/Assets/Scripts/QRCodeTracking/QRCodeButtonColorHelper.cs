using System.Collections.Generic;
using UnityEngine;

namespace QR_Code_Tracking
{
    public class QRCodeButtonColorHelper : MonoBehaviour
    {
        private bool isDone = false;
        private List<Renderer> spriteRenderers = new List<Renderer>();
        [SerializeField] private Color DoneColor;
        [SerializeField] private Color NotDoneColor;
        [SerializeField] private GameObject[] IconGameObjects;


        public void Start()
        {
            Renderer sr;
            foreach (var go in IconGameObjects)
            {
                sr = go.GetComponent<Renderer>();
                spriteRenderers.Add(sr);
                sr.material.color = NotDoneColor;
            }
        }

        [ContextMenu("Done")]
        public void Done()
        {
            MarkAsDone();
        }

        [ContextMenu("UnDone")]
        public void UnDone()
        {
            MarkAsDone(false);
        }

        public void MarkAsDone(bool done = true)
        {
            foreach (var sr in spriteRenderers)
            {
                if (done) sr.material.color = DoneColor;
                else sr.material.color = NotDoneColor;
            }

            isDone = done;
        }
    }
}