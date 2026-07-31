/// <summary>
/// Bewegt ein Partikel entlang eines Pfads unter Berücksichtigung der Sinuswellen-Gleichung.
/// Der Partikelpfad rendert die zurückgelegte Strecke und kann Interferenz mehrerer Partikelpfade verarbeiten.
/// </summary>
using System.Collections;
using System.Collections.Generic;
using AMI.Util;
using UnityEngine;

namespace QuantenKoffer.Laser
{
    /// <summary>
    ///     Moves a particle along a path adhering to the sinewave equation
    ///     Particle has a trail which renders the covered path
    ///     Also able to handle interference of multiple ParticlePath objects
    /// </summary>
    public class ParticlePath : MonoBehaviour
    {
        /// <summary>
        ///     Prefab of the particle moving along the path
        /// </summary>
        [Tooltip("Prefab of the particle moving along the path")] [SerializeField]
        private TrailRenderer particlePrefab;

        /// <summary>
        ///     Color of the particle
        /// </summary>
        [Tooltip("Color of the particle")] public Color ParticleColor = Color.red;

        /// <summary>
        ///     Multiply the thickness of the visible trail by this value
        /// </summary>
        [Tooltip("Multiply the thickness of the visible trail by this value")]
        public float TrailWidthMultiplier = .01f;

        /// <summary>
        ///     How long is the trail visible until it fades
        /// </summary>
        [Tooltip("How long is the trail visible until it fades")] [SerializeField]
        private float trailLifetime = .5f;

        /// <summary>
        ///     How fast is the particle moving
        /// </summary>
        [Tooltip("How fast is the particle moving")] [SerializeField]
        private float speed = 1f;

        [Header("Sinewave")]
        /// <summary>
        /// Sinewave amplitude
        /// </summary> 
        [Tooltip("Sinewave amplitude")]
        [SerializeField]
        private float amplitude = 1f;

        [SerializeField] private List<float> amplitudes = new();

        /// <summary>
        ///     Sinewave frequency
        /// </summary>
        [Tooltip("Sinewave frequency")] [SerializeField]
        private float frequency = 1f;

        [SerializeField] private List<float> frequencies = new();

        /// <summary>
        ///     Sinewave offset
        /// </summary>
        [Tooltip("Sinewave offset")] [SerializeField]
        private float amplitudeOffset;

        [SerializeField] private List<float> amplitudeOffsets = new();


        [Header("Positioning")]
        /// <summary>
        /// Path 1 for when the path is from a position to a position
        /// </summary> 
        [Tooltip("Path 1 for when the path is from a position to a position")]
        [SerializeField]
        private Vector3 startPosition;

        /// <summary>
        ///     Path 2 for when the path is from a position to a position
        /// </summary>
        [Tooltip("Path 2 for when the path is from a position to a position")] [SerializeField]
        private Vector3 targetPosition;

        [Tooltip("Total Distance traveled in Fields")] [SerializeField]
        private float distance;

        public float Distance
        {
            get => distance;
            set => distance = value;
        }

        public Vector3 TargetPosition => targetPosition;

        public Vector3 StartPosition => startPosition;

        public TrailRenderer ParticlePrefab
        {
            get => particlePrefab;
            set => particlePrefab = value;
        }

        public float TrailLifetime
        {
            get => trailLifetime;
            set => trailLifetime = value;
        }

        public float Speed
        {
            get => speed;
            set => speed = value;
        }

        public float Amplitude
        {
            get => amplitude;
            set => amplitude = value;
        }

        public float Frequency
        {
            get => frequency;
            set => frequency = value;
        }

        public float AmplitudeOffset
        {
            get => amplitudeOffset;
            set => amplitudeOffset = value;
        }

        private float speedUp = 1;

        private Coroutine coroutine, signalCoroutine;

        /// <summary>
        ///     List of all particles currently on the path
        /// </summary>
        private readonly List<TrailRenderer> particles = new();
        // List of all ParticlePath Objects spawned showing related Lasers for Interference

        public override string ToString()
        {
            return $"{name}: {AmplitudeOffset}";
        }

        /// <summary>
        /// Setzt den Geschwindigkeitsmultiplikator für den Partikelpfad.
        /// </summary>
        /// <param name="value">Geschwindigkeitsmultiplikator</param>
        public void SetSpeedup(float value)
        {
            speedUp = value;
        }

        /// <summary>
        /// Fügt einen weiteren Partikelpfad für die Interferenzberechnung hinzu.
        /// </summary>
        /// <param name="other">Anderer Partikelpfad</param>
        public void Add(ParticlePath other)
        {
            amplitudes.Add(amplitude);
            amplitudes.Add(other.amplitude);
            frequencies.Add(frequency);
            frequencies.Add(other.frequency);
            var settingDifference = Distance - other.Distance;
            amplitudeOffsets.Add(-1f);
            amplitudeOffsets.Add(settingDifference);
        }

        /// <summary>
        ///     Sets this paths start and endposition from Vector3
        /// </summary>
        /// <param name="startPosition">Startposition of the path</param>
        /// <param name="targetPosition">Endposition of the path</param>
        public void SetPath(Vector3 startPosition, Vector3 targetPosition)
        {
            this.startPosition = startPosition;
            this.targetPosition = targetPosition;
        }

        /// <summary>
        ///     Start the ShootParticles Coroutine
        /// </summary>
        [ContextMenu("Show Path")]
        public void ShowPath()
        {
            if (coroutine != null) StopCoroutine(coroutine);

            DestroyParticles();
            coroutine = StartCoroutine(ShootParticles(int.MaxValue));
        }

        /// <summary>
        ///     Sets this paths start and endposition from Transform.position
        /// </summary>
        /// <param name="startPosition">Startposition of the path</param>
        /// <param name="targetPosition">Endposition of the path</param>
        public void SetPath(Transform startPosition, Transform targetPosition)
        {
            SetPath(startPosition.position, targetPosition.position);
            if (transform.parent.name == "4 -> 7")
                Console.Log("4-7Found");
        }


        /// <summary>
        ///     Spawn particles coroutine which spawns and sets the startvalues of a particle then calls the MoveCoroutine for it.
        ///     Then repeats with a delay(delayBetweenParticles)
        /// </summary>
        /// <param name="amount">how many particles should be spawned in total</param>
        private IEnumerator ShootParticles(int amount)
        {
            var particle = Instantiate(particlePrefab);
            particle.transform.SetParent(transform);

            particle.transform.position = startPosition;
            particle.transform.rotation = Quaternion.LookRotation(particle.transform.position - transform.position);
            particle.GetComponentInChildren<SpriteRenderer>().color = ParticleColor;
            particle.startColor = ParticleColor;
            particle.endColor = ParticleColor;
            particle.widthMultiplier = TrailWidthMultiplier;
            particle.time = trailLifetime;
            var theoreticalPosition = particle.transform.position;

            float time = 0; // Time elapsed since start

            Vector3 newPosition;
            float yPosition = 0;

            while (Vector3.Distance(theoreticalPosition, targetPosition) > 0 && particle != null)
            {
                /** TODO: We do have a little hickup in the moment we change the speed of the particle.
             * I thought it had to do with changing speedUp Values while the coroutine runs
             * (theoretical position and y position speedup value differs)
             * but this seems not to be the case.  Maybe we need to precompute the next position
             * and use it on speed change. to buffer the change?
            **/

                theoreticalPosition =
                    Vector3.MoveTowards(theoreticalPosition, targetPosition, Time.deltaTime * speed * speedUp);
                time += Time.deltaTime;

                newPosition = theoreticalPosition;
                yPosition = 0;
                if (amplitudes.Count == 0)
                    yPosition += amplitude * Mathf.Sin(2f * Mathf.PI * frequency * speedUp *
                                                       (time / speedUp + distance));
                else
                    for (var i = 0; i < amplitudes.Count; i++)
                    {
                        float step = 0.25f / 3;
                        float multiplier = 1;
                        if (amplitudeOffsets[i] == -1f) multiplier = 0;
                        else if (amplitudeOffsets[i] == 0f) multiplier = 2;
                        else if (amplitudeOffsets[i] == 0.25f) multiplier = 0.0f;
                        else if (amplitudeOffsets[i] == 0.5f) multiplier = 2;
                        else if (Mathf.Abs(amplitudeOffsets[i] - (0.25f - 2 * step)) < step / 2) multiplier = 1.3f;
                        else if (Mathf.Abs(amplitudeOffsets[i] - (0.25f - step)) < step / 2) multiplier = 0.6f;
                        else if (Mathf.Abs(amplitudeOffsets[i] - (0.5f - 2 * step)) < step / 2) multiplier = 0.6f;
                        else if (Mathf.Abs(amplitudeOffsets[i] - (0.5f - step)) < step / 2) multiplier = 1.3f;

                        if (multiplier != 0)
                            yPosition += amplitudes[i] * multiplier * Mathf.Sin(2f * Mathf.PI * frequencies[i] *
                                speedUp *
                                (time / speedUp + distance));
                    }

                newPosition.y += yPosition;
                particle.time = trailLifetime * 1 / speedUp;
                particle.transform.position = newPosition;
                yield return null;
            }

            amplitudes.Clear();
            frequencies.Clear();
            amplitudeOffsets.Clear();
            yield return new WaitForSeconds(particle.time);

            Destroy(gameObject.transform.parent.gameObject);
        }

        private void OnDisable()
        {
            DestroyParticles();
            StopAllCoroutines();
        }

        private void OnDestroy()
        {
            DestroyParticles();
            StopAllCoroutines();
        }

        /// <summary>
        ///     Destroys and removes all particles from particles list
        /// </summary>
        [ContextMenu("Destroy particles")]
        private void DestroyParticles()
        {
            for (var i = 0; i < particles.Count; i++)
                if (particles[i] != null && particles[i].gameObject != null)
                {
                    Destroy(particles[i].gameObject);
                    particles.RemoveAt(i);
                }
        }
    }
}