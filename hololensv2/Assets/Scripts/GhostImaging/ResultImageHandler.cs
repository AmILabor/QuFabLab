/// <summary>
/// Enthält die Klasse zur Verarbeitung der Ergebnisdarstellung im Ghost-Imaging-Experiment.
/// </summary>
using System.Collections;
using System.Linq;
using GhostImaging.Bricks;
using UnityEngine;

namespace QuantenKoffer.Bricks
{
    /// <summary>
    /// Verarbeitet die Ergebnisdarstellung des Ghost-Imaging-Experiments.
    /// </summary>
    public class ResultImageHandler : MonoBehaviour

    {
        public GameObject ArrayDetector;
        private ArrayDetectorElementBrick[] DetectorBricks;
        private ResultDisplayElementBrick[] DisplayBricks;

        /// <summary>
        /// Initialisiert die Detektor- und Anzeigeelemente.
        /// </summary>
        public void Start()
        {
            DetectorBricks = ArrayDetector.GetComponentsInChildren<ArrayDetectorElementBrick>();
            DisplayBricks = gameObject.GetComponentsInChildren<ResultDisplayElementBrick>();
        }

        /// <summary>
        /// Löscht alle Markierungen auf dem Anzeigeelement.
        /// </summary>
        [ContextMenu("Clear")]
        public void Clear()
        {
            foreach (var brick in DisplayBricks)
            {
                brick.Highlight(false);
            }
        }

        /// <summary>
        /// Wird benachrichtigt, wenn ein Detektor getroffen wurde, und startet die Koroutine zur Aktualisierung der Anzeige.
        /// </summary>
        public void NotifyDetectorHit()
        {
            StartCoroutine(AwaitCurrentlyActiveBrick());
        }

        /// <summary>
        /// Wartet auf den aktuell aktiven Detektor und hebt das entsprechende Anzeigeelement hervor.
        /// </summary>
        private IEnumerator AwaitCurrentlyActiveBrick()
        {
            ArrayDetectorElementBrick currentlyActiveBricks =
                DetectorBricks.DefaultIfEmpty(null).FirstOrDefault(brick => brick.ReadHitState());
            while (currentlyActiveBricks == null)
            {
                currentlyActiveBricks =
                    DetectorBricks.DefaultIfEmpty(null).FirstOrDefault(brick => brick.ReadHitState());
                yield return null;
            }

            DisplayBricks[currentlyActiveBricks.MyIndex].Highlight(true);
        }

        /// <summary>
        /// Wird pro Frame aufgerufen (derzeit leer).
        /// </summary>
        private void Update()
        {
        }
    }
}