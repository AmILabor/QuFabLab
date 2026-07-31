/// <summary>
/// Repräsentiert einen Laserstrahl im Quantenkoffer. Enthält Funktionen zum Klonen, Zeichnen,
/// zur Interferenzberechnung und zur Geschwindigkeitssteuerung des Partikelpfads.
/// </summary>
using System.Collections;
using QuantenKoffer.Bricks;
using UnityEngine;

namespace QuantenKoffer.Laser
{
    /// <summary>
    /// Repräsentiert einen Laserstrahl im Quantenkoffer mit Funktionen zum Klonen, Zeichnen und zur Interferenz.
    /// </summary>
    public class LaserBeam : MonoBehaviour
    {
        [SerializeField] private ParticlePath ParticlePath;

        public int amplitude { get; private set; } = 1;
        public float wavelength { get; private set; } = 1;
        public Vector3 direction { get; private set; } = Vector3.zero;
        public Transform from { get; private set; }
        public Transform to { get; private set; }
        private float CurrentSpeedup = 0.5f;

        /// <summary>
        /// Setzt den Geschwindigkeitsmultiplikator für den Laser und den Partikelpfad.
        /// </summary>
        /// <param name="speedMultiplier">Geschwindigkeitsmultiplikator</param>
        public void SetSpeed(float speedMultiplier)
        {
            CurrentSpeedup = speedMultiplier;
            ParticlePath.SetSpeedup(speedMultiplier);
        }

        /// <summary>
        /// Setzt die Amplitude des Laserstrahls.
        /// </summary>
        /// <param name="amplitudeMultiplier">Amplitudenmultiplikator</param>
        public void SetAmplitude(float amplitudeMultiplier)
        {
            amplitude = (int)(amplitudeMultiplier);
        }

        /// <summary>
        /// Setzt die Farbe des Partikelpfads.
        /// </summary>
        /// <param name="color">Neue Farbe</param>
        public void SetColor(Color color)
        {
            ParticlePath.ParticleColor = color;
        }

        /// <summary>
        /// Setzt den Wellenlängenmultiplikator des Laserstrahls.
        /// </summary>
        /// <param name="wavelengthMultiplier">Wellenlängenmultiplikator</param>
        public void SetWaveLengthMultiplier(float wavelengthMultiplier)
        {
            wavelength = wavelengthMultiplier;
        }

        /// <summary>
        /// Normalisiert die Strahlrichtung auf eine der sechs Hauptrichtungen.
        /// </summary>
        /// <param name="inVector">Eingabevektor</param>
        /// <returns>Normalisierter Richtungsvektor</returns>
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

        /// <summary>
        /// Initialisiert einen Laserstrahl mit Quell-, Ziel-Transform, Amplitude und Wellenlänge.
        /// </summary>
        /// <param name="lb">Zu initialisierender Laserstrahl</param>
        /// <param name="sourceTransform">Quell-Transform</param>
        /// <param name="targetTransform">Ziel-Transform</param>
        /// <param name="_amplitude">Amplitude</param>
        /// <param name="_wavelength">Wellenlänge</param>
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

        /// <summary>
        /// Leitet den Laserstrahl zu einer neuen Position um.
        /// </summary>
        /// <param name="position">Neue Zielposition</param>
        public void RedirectBeamToPosition(Vector3 position)
        {
            transform.position = position;
            InitializeLaserBeam(this, this.from, transform, this.amplitude, this.wavelength);
        }

        /// <summary>
        /// Erstellt einen Klon des Laserstrahls zwischen zwei Bausteinen mit angegebenen Parametern.
        /// </summary>
        /// <param name="source">Quell-Baustein</param>
        /// <param name="target">Ziel-Baustein</param>
        /// <param name="_amplitude">Amplitude</param>
        /// <param name="_wavelength">Wellenlänge</param>
        /// <returns>Geklonter Laserstrahl</returns>
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

        /// <summary>
        /// Erstellt einen Klon des Laserstrahls mit aktuellen Amplitude- und Wellenlängenwerten.
        /// </summary>
        /// <param name="source">Quell-Baustein</param>
        /// <param name="target">Ziel-Baustein</param>
        /// <returns>Geklonter Laserstrahl</returns>
        public LaserBeam Clone(Brick source, Brick target)
        {
            return Clone(source, target, amplitude, wavelength);
        }

        /// <summary>
        /// Führt die Interferenzberechnung mit einem anderen Laserstrahl durch.
        /// </summary>
        /// <param name="other">Anderer Laserstrahl für Interferenz</param>
        public void PerformInterference(LaserBeam other)
        {
            //NewParticlePath pp = GetComponentInChildren<NewParticlePath>();
            ParticlePath.Add(other.GetComponentInChildren<ParticlePath>());
        }

        /// <summary>
        /// Zeichnet den Laserstrahl, indem der Partikelpfad zwischen Quelle und Ziel gesetzt wird.
        /// </summary>
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

        /// <summary>
        /// Zerstört den Laserstrahl, nachdem die Trail-Lebensdauer abgelaufen ist.
        /// </summary>
        public void DestroyWhenDone()
        {
            StartCoroutine(DestroyWhenDoneCoroutine());
        }

        /// <summary>
        /// Coroutine, die nach Ablauf der Trail-Lebensdauer den Laserstrahl zerstört.
        /// </summary>
        private IEnumerator DestroyWhenDoneCoroutine()
        {
            yield return new WaitForSeconds(ParticlePath.TrailLifetime);
            Destroy(gameObject);
            AMI.Util.Console.Log("Destroyed trail.");
        }
    }
}