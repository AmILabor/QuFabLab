using System.Collections;
using QuantenKoffer.Bricks;
using UnityEngine;

namespace QuantenKoffer.Laser
{
    public class LaserBeam : MonoBehaviour
    {
        [SerializeField] private ParticlePath ParticlePath;

        public int amplitude { get; private set; } = 1;
        public float wavelength { get; private set; } = 1;
        public Vector3 direction { get; private set; } = Vector3.zero;
        public Transform from { get; private set; }
        public Transform to { get; private set; }
        private float CurrentSpeedup = 0.5f;

        public void SetSpeed(float speedMultiplier)
        {
            CurrentSpeedup = speedMultiplier;
            ParticlePath.SetSpeedup(speedMultiplier);
        }

        public void SetAmplitude(float amplitudeMultiplier)
        {
            amplitude = (int)(amplitudeMultiplier);
        }

        public void SetColor(Color color)
        {
            ParticlePath.ParticleColor = color;
        }

        public void SetWaveLengthMultiplier(float wavelengthMultiplier)
        {
            wavelength = wavelengthMultiplier;
        }

        private Vector3 NormalizeBeamDirection(Vector3 inVector)
        {
            if (inVector.x < -0.5) return Vector3.left;
            if (inVector.x > 0.5) return Vector3.right;
            if (inVector.y < -0.5) return Vector3.down;
            if (inVector.y > 0.5) return Vector3.up;
            if (inVector.z < -0.5) return Vector3.back;
            if (inVector.z > 0.5) return Vector3.forward;
            AMI.Util.Console.LogError("Beam", $"Encountered Strange inVector: {inVector}");
            return Vector3.zero;
        }

        private void InitializeLaserBeam(LaserBeam lb, Transform sourceTransform, Transform targetTransform,
            int _amplitude, float _wavelength)
        {
            Brick90 srcBrick = sourceTransform.gameObject.GetComponentInParent<Brick90>();
            if (srcBrick)
            {
                lb.ParticlePath.Distance = srcBrick.GetSetting();
            }


            //lb.transform.position = Vector3.zero;
            lb.transform.parent = transform.parent;
            lb.SetSpeed(CurrentSpeedup);
            lb.from = sourceTransform;
            lb.to = targetTransform;
            lb.amplitude = _amplitude;
            lb.wavelength = _wavelength;
            lb.direction = NormalizeBeamDirection((lb.from.position - lb.to.position).normalized);
            TrailRenderer tr = lb.GetComponentInChildren<TrailRenderer>();
            if (tr != null)
            {
                Destroy(tr.gameObject);
            }
        }

        public void RedirectBeamToPosition(Vector3 position)
        {
            transform.position = position;
            InitializeLaserBeam(this, this.from, transform, this.amplitude, this.wavelength);
        }

        public LaserBeam Clone(Brick source, Brick target, int _amplitude, float _wavelength)
        {
            LaserBeam lb = GameObject.Instantiate(this);
            Transform sourceCenter = source.getCenterTransform();
            Transform targetCenter = target.getCenterTransform();
            InitializeLaserBeam(lb, sourceCenter, targetCenter, _amplitude, _wavelength);
            string sourceName = source.transform.parent.gameObject.name.Split("_")[0];
            string targetName = target.transform.parent.gameObject.name.Split("_")[0];
            lb.gameObject.name = $"{sourceName} -> {targetName}";
            return lb;
        }

        public LaserBeam Clone(Brick source, Brick target)
        {
            return Clone(source, target, amplitude, wavelength);
        }

        public void PerformInterference(LaserBeam other)
        {
            //NewParticlePath pp = GetComponentInChildren<NewParticlePath>();
            ParticlePath.Add(other.GetComponentInChildren<ParticlePath>());
        }

        public void Draw()
        {
            //NewParticlePath pp = gameObject.GetComponentInChildren<NewParticlePath>();
            if (ParticlePath == null)
            {
                AMI.Util.Console.LogError("LaserBeam", "Could not find ParticlePath");
                return;
            }

            ParticlePath.Amplitude *= amplitude;
            ParticlePath.Frequency *= wavelength;
            ParticlePath.SetPath(from, to);
            ParticlePath.ShowPath();
        }

        public void DestroyWhenDone()
        {
            StartCoroutine(DestroyWhenDoneCoroutine());
        }

        private IEnumerator DestroyWhenDoneCoroutine()
        {
            yield return new WaitForSeconds(ParticlePath.TrailLifetime);
            Destroy(gameObject);
            AMI.Util.Console.Log("Destroyed trail.");
        }
    }
}