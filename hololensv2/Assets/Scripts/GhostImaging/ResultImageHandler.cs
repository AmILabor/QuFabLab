using System.Collections;
using System.Linq;
using GhostImaging.Bricks;
using UnityEngine;

namespace QuantenKoffer.Bricks
{
    public class ResultImageHandler : MonoBehaviour

    {
        public GameObject ArrayDetector;
        private ArrayDetectorElementBrick[] DetectorBricks;
        private ResultDisplayElementBrick[] DisplayBricks;

        public void Start()
        {
            DetectorBricks = ArrayDetector.GetComponentsInChildren<ArrayDetectorElementBrick>();
            DisplayBricks = gameObject.GetComponentsInChildren<ResultDisplayElementBrick>();
        }

        [ContextMenu("Clear")]
        public void Clear()
        {
            foreach (var brick in DisplayBricks)
            {
                brick.Highlight(false);
            }
        }

        public void NotifyDetectorHit()
        {
            StartCoroutine(AwaitCurrentlyActiveBrick());
        }

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

        private void Update()
        {
        }
    }
}