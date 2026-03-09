using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace QuantenKoffer.Bricks
{
    public class PeriscopeHandler : MonoBehaviour
    {
        // Start is called before the first frame update
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

        private void Start()
        {
            spawnedPeriscopes[TopLeftSpawn] = null;
            spawnedPeriscopes[TopRightSpawn] = null;
            spawnedPeriscopes[BottomLeftSpawn] = null;
            spawnedPeriscopes[BottomRightSpawn] = null;
        }

        public void SpawnAllPeriscopes()
        {
            CreatePeriscope(0);
            CreatePeriscope(1);
            CreatePeriscope(2);
            CreatePeriscope(3);
        }

        public void SpawnPeriscopeVoid(Transform src)
        {
            if (spawnedPeriscopes[src] == null)
                CreatePeriscope(src);
        }

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

        public Vector3 GetLeftPeriscopePosition()
        {
            return TopLeftSpawn.position;
        }

        public Vector3 GetRightPeriscopePosition()
        {
            return TopRightSpawn.position;
        }

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

        [ContextMenu("StartBeams")]
        public void StartBeams()
        {
            if (IsPeriscopeActive(0))
                spawnedPeriscopes[TopLeftSpawn]?.StartBeam(LaserParent, SpeedMultiplier * laserScaleFactor);
            if (IsPeriscopeActive(3))
                spawnedPeriscopes[BottomLeftSpawn]?.StartBeam(LaserParent, SpeedMultiplier * laserScaleFactor);
        }

        public void SetSpeed(float speed)
        {
            SpeedMultiplier = speed;
        }

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