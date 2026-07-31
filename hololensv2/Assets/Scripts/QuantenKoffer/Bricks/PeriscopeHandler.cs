/// <summary>
/// Verwaltet die Erstellung, Positionierung und Steuerung von Periskopen auf dem Spielfeld.
/// Enthält Referenzen auf die vier Periskop-Positionen (oben links, oben rechts, unten links, unten rechts).
/// </summary>
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace QuantenKoffer.Bricks
{
    /// <summary>
    /// Verwaltet die Erstellung, Positionierung und Steuerung von Periskopen auf dem Spielfeld.
    /// </summary>
    public class PeriscopeHandler : MonoBehaviour
    {
        [SerializeField] private GameObject factory;
        [SerializeField] public float periscopeScaleFactor = 1;
        [SerializeField] public float laserScaleFactor = 1;
        public GameObject LaserBeamPrefab;
        [SerializeField] private Transform TopLeftSpawn;
        [SerializeField] private Transform TopRightSpawn;
        [SerializeField] private Transform BottomLeftSpawn;
        [SerializeField] private Transform BottomRightSpawn;
        [SerializeField] private Transform LaserParent;
        [SerializeField] private UnityEvent OnPeriscopeChange;
        private float SpeedMultiplier = 1.0f;

        private Dictionary<Transform, Periscope> spawnedPeriscopes = new Dictionary<Transform, Periscope>();

        /// <summary>
        /// Initialisiert das Dictionary für die vier Periskop-Positionen.
        /// </summary>
        private void Start()
        {
            spawnedPeriscopes[TopLeftSpawn] = null;
            spawnedPeriscopes[TopRightSpawn] = null;
            spawnedPeriscopes[BottomLeftSpawn] = null;
            spawnedPeriscopes[BottomRightSpawn] = null;
        }

        /// <summary>
        /// Erstellt alle vier Periskope auf dem Spielfeld.
        /// </summary>
        public void SpawnAllPeriscopes()
        {
            CreatePeriscope(0);
            CreatePeriscope(1);
            CreatePeriscope(2);
            CreatePeriscope(3);
        }

        /// <summary>
        /// Erstellt ein Periskop an der angegebenen Position, falls dort noch keines existiert.
        /// </summary>
        /// <param name="src">Transform der Position</param>
        public void SpawnPeriscopeVoid(Transform src)
        {
            if (spawnedPeriscopes[src] == null)
                CreatePeriscope(src);
        }

        /// <summary>
        /// Erstellt ein Periskop am angegebenen Transform und konfiguriert es.
        /// </summary>
        /// <param name="src">Transform-Position für das Periskop</param>
        /// <returns>Das erstellte Periskop</returns>
        public Brick CreatePeriscope(Transform src)
        {
            GameObject brick = factory.GetComponent<BrickFactory>().CreatePeriscopeAtPosition(src);


            spawnedPeriscopes[src] = brick.GetComponentInChildren<Periscope>();
            spawnedPeriscopes[src].transform.Rotate(Vector3.up, 90);
            spawnedPeriscopes[src].transform.localScale *= periscopeScaleFactor;
            if (LaserBeamPrefab != null)
            {
                spawnedPeriscopes[src].SetLaserBeam(LaserBeamPrefab);
            }

            OnPeriscopeChange.Invoke();
            return spawnedPeriscopes[src];
        }

        /// <summary>
        /// Gibt die Position des linken oberen Periskops zurück.
        /// </summary>
        /// <returns>Position des linken Periskops</returns>
        public Vector3 GetLeftPeriscopePosition()
        {
            return TopLeftSpawn.position;
        }

        /// <summary>
        /// Gibt die Position des rechten oberen Periskops zurück.
        /// </summary>
        /// <returns>Position des rechten Periskops</returns>
        public Vector3 GetRightPeriscopePosition()
        {
            return TopRightSpawn.position;
        }

        /// <summary>
        /// Erstellt ein Periskop anhand des Index (0=oben links, 1=oben rechts, 2=unten rechts, 3=unten links).
        /// </summary>
        /// <param name="index">Index der Periskop-Position</param>
        /// <returns>Das erstellte Periskop</returns>
        public Brick CreatePeriscope(int index)
        {
            Transform target = null;
            switch (index)
            {
                case 0:
                    target = TopLeftSpawn;
                    break;
                case 1:
                    target = TopRightSpawn;
                    break;
                case 2:
                    target = BottomRightSpawn;
                    break;
                case 3:
                    target = BottomLeftSpawn;
                    break;
                default:
                    AMI.Util.Console.LogError("PeriscopeHandler",
                        "Someone tried to TogglePeriscope with unusable index");
                    return null;
            }

            return CreatePeriscope(target);
        }

        /// <summary>
        /// Gibt das Periskop anhand des Index zurück.
        /// </summary>
        /// <param name="index">Index der Periskop-Position</param>
        /// <returns>Das Periskop oder null</returns>
        public Brick GetPeriscopeByIndex(int index)
        {
            switch (index)
            {
                case 0:
                    return spawnedPeriscopes[TopLeftSpawn];
                case 1:
                    return spawnedPeriscopes[TopRightSpawn];
                case 2:
                    return spawnedPeriscopes[BottomRightSpawn];
                case 3:
                    return spawnedPeriscopes[BottomLeftSpawn];
            }

            return null;
        }

        /// <summary>
        /// Prüft, ob ein Periskop am angegebenen Index aktiv ist.
        /// </summary>
        /// <param name="index">Index der Periskop-Position</param>
        /// <returns>True, wenn das Periskop aktiv ist</returns>
        public bool IsPeriscopeActive(int index)
        {
            switch (index)
            {
                case 0:
                    return spawnedPeriscopes[TopLeftSpawn] != null;
                case 1:
                    return spawnedPeriscopes[TopRightSpawn] != null;
                case 2:
                    return spawnedPeriscopes[BottomRightSpawn] != null;
                case 3:
                    return spawnedPeriscopes[BottomLeftSpawn] != null;
            }

            return false;
        }

        /// <summary>
        /// Startet die Laserstrahlen der aktiven linken Periskope.
        /// </summary>
        [ContextMenu("StartBeams")]
        public void StartBeams()
        {
            if (IsPeriscopeActive(0))
                spawnedPeriscopes[TopLeftSpawn]?.StartBeam(LaserParent, SpeedMultiplier * laserScaleFactor);
            if (IsPeriscopeActive(3))
                spawnedPeriscopes[BottomLeftSpawn]?.StartBeam(LaserParent, SpeedMultiplier * laserScaleFactor);
        }

        /// <summary>
        /// Setzt den Geschwindigkeitsmultiplikator für Laserstrahlen.
        /// </summary>
        /// <param name="speed">Geschwindigkeitswert</param>
        public void SetSpeed(float speed)
        {
            SpeedMultiplier = speed;
        }

        /// <summary>
        /// Entfernt ein Periskop anhand des Index.
        /// </summary>
        /// <param name="index">Index des zu entfernenden Periskops</param>
        public void ClearPeriscope(int index)
        {
            if (!IsPeriscopeActive(index)) return;
            switch (index)
            {
                case 0:
                    spawnedPeriscopes[TopLeftSpawn].Destroy();
                    return;
                case 1:
                    spawnedPeriscopes[TopRightSpawn].Destroy();
                    return;
                case 2:
                    spawnedPeriscopes[BottomRightSpawn].Destroy();
                    return;
                case 3:
                    spawnedPeriscopes[BottomLeftSpawn].Destroy();
                    return;
            }
        }

        /// <summary>
        /// Entfernt alle Periskope und löst das OnPeriscopeChange-Event aus.
        /// </summary>
        public void ClearPeriscopes()
        {
            foreach (var brick in spawnedPeriscopes)
            {
                if (brick.Value != null)
                    brick.Value.Destroy();
            }

            OnPeriscopeChange.Invoke();
        }
    }
}