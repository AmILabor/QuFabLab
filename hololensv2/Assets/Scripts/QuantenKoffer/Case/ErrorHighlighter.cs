/// <summary>
/// Zeigt Fehlerdialoge für falsch platzierte, falsch rotierte oder falsche Brick-Typen an.
/// Verwaltet die Sichtbarkeit von Platzierungs-, Rotations- und Typfehler-Texten.
/// </summary>
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;

namespace QuantenKoffer.Case
{
    /// <summary>
    /// Zeigt Fehlerdialoge für falsch platzierte, falsch rotierte oder falsche Brick-Typen an.
    /// </summary>
    public class ErrorHighlighter : MonoBehaviour
    {
        [SerializeField] private GameObject PlacementText;
        [SerializeField] private GameObject RotationText;
        [SerializeField] private GameObject TypeText;
        private bool wrongPlace = false;
        private bool wrongType = false;
        private bool wrongRotation = false;
        private List<GameObject> Children = new List<GameObject>();
        // Start is called before the first frame update

        /// <summary>
        /// Sammelt alle Child-GameObjects beim Start.
        /// </summary>
        public void Awake()
        {
            gameObject.GetChildGameObjects(Children);
        }

        /// <summary>
        /// Zeigt alle Fehlerdialog-Elemente an.
        /// </summary>
        [ContextMenu("ShowErrorDialog")]
        public void Show()
        {
            foreach (var child in Children)
            {
                child.SetActive(true);
            }
        }

        /// <summary>
        /// Versteckt alle Fehlerdialog-Elemente und setzt die Fehler zurück.
        /// </summary>
        [ContextMenu("HideErrorDialog")]
        public void Hide()
        {
            foreach (var child in Children)
            {
                child.SetActive(false);
            }

            ResetErrors();
        }

        /// <summary>
        /// Setzt alle Fehlerflags zurück und deaktiviert die Fehlertexte.
        /// </summary>
        private void ResetErrors()
        {
            wrongPlace = false;
            PlacementText.gameObject.SetActive(false);
            wrongRotation = false;
            RotationText.gameObject.SetActive(false);
            wrongType = false;
            TypeText.gameObject.SetActive(false);
        }

        /// <summary>
        /// Aktualisiert die Fehleranzeige basierend auf den übergebenen Fehlerflags.
        /// Zeigt den entsprechenden Fehlerdialog oder versteckt alle, wenn keine Fehler vorliegen.
        /// </summary>
        /// <param name="placeError">Platzierungsfehler</param>
        /// <param name="typeError">Typfehler</param>
        /// <param name="rotationError">Rotationsfehler</param>
        [ContextMenu("ShowWrongPlace")]
        public void UpdateErrors(bool placeError, bool typeError, bool rotationError)
        {
            ResetErrors();

            if (placeError) SetWrongPlace();
            else if (typeError) SetWrongType();
            else if (rotationError) SetWrongRotation();

            if (!(placeError || typeError || rotationError)) Hide();
            else Show();
        }

        /// <summary>
        /// Markiert einen Platzierungsfehler und zeigt den entsprechenden Text an.
        /// </summary>
        [ContextMenu("ShowWrongPlace")]
        public void SetWrongPlace()
        {
            wrongPlace = true;
            PlacementText.gameObject.SetActive(true);
        }

        /// <summary>
        /// Markiert einen Rotationsfehler und zeigt den entsprechenden Text an.
        /// </summary>
        [ContextMenu("ShowWrongRotation")]
        public void SetWrongRotation()
        {
            wrongRotation = true;
            RotationText.gameObject.SetActive(true);
        }

        /// <summary>
        /// Markiert einen Typfehler und zeigt den entsprechenden Text an.
        /// </summary>
        [ContextMenu("ShowWrongType")]
        public void SetWrongType()
        {
            wrongType = true;
            TypeText.gameObject.SetActive(true);
        }
    }
}