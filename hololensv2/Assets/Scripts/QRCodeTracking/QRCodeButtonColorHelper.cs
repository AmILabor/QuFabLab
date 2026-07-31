/// <summary>
/// Enthält Hilfsklassen zur Farbverwaltung von QR-Code-Buttons.
/// </summary>
using System.Collections.Generic;
using UnityEngine;

namespace QR_Code_Tracking
{
    /// <summary>
    /// Hilfsklasse zum Ändern der Button-Farbe basierend auf dem QR-Code-Status.
    /// </summary>
    public class QRCodeButtonColorHelper : MonoBehaviour
    {
        private bool isDone = false;
        private List<Renderer> spriteRenderers = new List<Renderer>();
        [SerializeField] private Color DoneColor;
        [SerializeField] private Color NotDoneColor;
        [SerializeField] private GameObject[] IconGameObjects;


        /// <summary>
        /// Initialisiert die Farbe der Icon-GameObjects.
        /// </summary>
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

        /// <summary>
        /// Markiert alle Icons als erledigt über das Kontextmenü.
        /// </summary>
        [ContextMenu("Done")]
        public void Done()
        {
            MarkAsDone();
        }

        /// <summary>
        /// Macht die Erledigt-Markierung aller Icons rückgängig über das Kontextmenü.
        /// </summary>
        [ContextMenu("UnDone")]
        public void UnDone()
        {
            MarkAsDone(false);
        }

        /// <summary>
        /// Setzt den Erledigt-Status der Icons und aktualisiert die Farben.
        /// </summary>
        /// <param name="done">Gibt an, ob die Icons als erledigt markiert werden sollen.</param>
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