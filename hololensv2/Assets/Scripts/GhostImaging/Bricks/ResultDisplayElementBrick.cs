using QuantenKoffer.Bricks;
using QuantenKoffer.Laser;
using Unity.XR.CoreUtils;
using UnityEngine;

namespace GhostImaging.Bricks
{
    public class ResultDisplayElementBrick : Brick
    {
        private GameObject TopRenderer;
        private GameObject HitRenderer;

        private void Start()
        {
            TopRenderer = gameObject.GetNamedChild("oben");
            HitRenderer = gameObject.GetNamedChild("oben_hit");
            HitRenderer.SetActive(false);
        }

        public override LaserBeam[] HandleLaser(LaserBeam beam)
        {
            return new LaserBeam[] { };
        }

        [ContextMenu("HighlightOn")]
        public void HLOn()
        {
            Highlight(true);
        }

        [ContextMenu("HighlightOff")]
        public void HLOff()
        {
            Highlight(false);
        }

        public void Highlight(bool active)
        {
            HitRenderer.SetActive(active);
            TopRenderer.SetActive(!active);
        }

        protected override Vector3[] getOutVectors(Vector3 inVector)
        {
            return new Vector3[] { };
        }
    }
}