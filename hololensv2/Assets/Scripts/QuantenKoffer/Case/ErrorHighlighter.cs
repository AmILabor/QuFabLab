using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;

namespace QuantenKoffer.Case
{
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

        public void Awake()
        {
            gameObject.GetChildGameObjects(Children);
        }

        [ContextMenu("ShowErrorDialog")]
        public void Show()
        {
            foreach (var child in Children)
            {
                child.SetActive(true);
            }
        }

        [ContextMenu("HideErrorDialog")]
        public void Hide()
        {
            foreach (var child in Children)
            {
                child.SetActive(false);
            }

            ResetErrors();
        }

        private void ResetErrors()
        {
            wrongPlace = false;
            PlacementText.gameObject.SetActive(false);
            wrongRotation = false;
            RotationText.gameObject.SetActive(false);
            wrongType = false;
            TypeText.gameObject.SetActive(false);
        }

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

        [ContextMenu("ShowWrongPlace")]
        public void SetWrongPlace()
        {
            wrongPlace = true;
            PlacementText.gameObject.SetActive(true);
        }

        [ContextMenu("ShowWrongRotation")]
        public void SetWrongRotation()
        {
            wrongRotation = true;
            RotationText.gameObject.SetActive(true);
        }

        [ContextMenu("ShowWrongType")]
        public void SetWrongType()
        {
            wrongType = true;
            TypeText.gameObject.SetActive(true);
        }
    }
}