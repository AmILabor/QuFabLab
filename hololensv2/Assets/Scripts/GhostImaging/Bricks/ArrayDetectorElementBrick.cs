using System.Collections;
using System.Linq;
using QuantenKoffer.Bricks;
using QuantenKoffer.Laser;
using UnityEngine;
using UnityEngine.Events;

namespace GhostImaging.Bricks
{
    public class ArrayDetectorElementBrick : Brick
    {
        [SerializeField] private float fadeTime = 5;
        [SerializeField] private float HitTurnOffDelay = 0.8f;
        [SerializeField] private UnityEvent OnHit;
        [SerializeField] private UnityEvent OffHit;
        [SerializeField] private MeshRenderer NotHitRenderer;
        [SerializeField] private MeshRenderer HitRenderer;

        private float timeLeft = 0;
        [SerializeField] public int MyIndex { get; private set; } = -1;
        [SerializeField] public bool IsCurrentlyHit { get; private set; }

        public void Start()
        {
            ArrayDetectorElementBrick[] bricks =
                transform.parent.transform.parent.GetComponentsInChildren<ArrayDetectorElementBrick>();
            MyIndex = bricks.TakeWhile(brick => brick != this).Count();
            OnHit.AddListener(() => { IsCurrentlyHit = true; });
            OffHit.AddListener(() => { IsCurrentlyHit = false; });
            IsCurrentlyHit = false;
        }

        public bool ReadHitState()
        {
            if (!IsCurrentlyHit)
                return false;

            IsCurrentlyHit = false;
            return true;
        }

        public override LaserBeam[] HandleLaser(LaserBeam beam)
        {
            LaserBeam[] beams = HandleLaserBase(beam, getOutVectors(beam.direction));
            foreach (var next_beam in beams)
            {
                next_beam.Draw();
            }

            return beams;
        }

        public override void HandleColision(LaserBeam beam)
        {
            OnHit.Invoke();
            StartCoroutine(TurnHitFlagOff());
            StartCoroutine(FadeBackFromHit());
        }

        private IEnumerator TurnHitFlagOff()
        {
            yield return new WaitForSeconds(HitTurnOffDelay);
            OffHit.Invoke();
        }

        private void SetAlphaColor(MeshRenderer renderer, float alpha)
        {
            Color a = renderer.material.color;
            renderer.material.color = new Color(a.r, a.g, a.b, alpha);
        }

        private IEnumerator FadeBackFromHit()
        {
            SetAlphaColor(HitRenderer, 1.0f);
            SetAlphaColor(NotHitRenderer, 0.0f);

            timeLeft = fadeTime;
            while (timeLeft > 0)
            {
                timeLeft -= Time.deltaTime;
                float alpha = (timeLeft / fadeTime);
                SetAlphaColor(HitRenderer, alpha);
                SetAlphaColor(NotHitRenderer, 1 - alpha);
                yield return null;
            }

            SetAlphaColor(HitRenderer, 0.0f);
            SetAlphaColor(NotHitRenderer, 1.0f);
            yield return null;
        }

        protected override Vector3[] getOutVectors(Vector3 inVector)
        {
            return new Vector3[] { };
        }
    }
}