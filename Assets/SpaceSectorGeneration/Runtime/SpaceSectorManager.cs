using System.Collections.Generic;
using UnityEngine;

namespace Lucy.Features.SpaceSectors.Runtime
{
    public sealed class SpaceSectorManager : MonoBehaviour
    {
        #region Inspector

        [Header("References Space Sector Manager")]
        public Transform target;
        public SectorContentGenerator contentGenerator;

        [Header("Settings Space Sector Manager")]
        [Min(1f)] public float sectorSize = 5000f;
        public Vector3Int loadRadius = new Vector3Int(1, 1, 1);
        public int worldSeed = 12345;
        public string saveSlot = "Default";
        [Min(0f)] public float autoSaveInterval = 30f;

        [Header("Debug")]
        public bool debugEnabled;
        public bool gizmosEnabled = true;
        public Vector3Int currentSector;
        public int activeSectorCount;
        public string saveDirectory;

        #endregion

        readonly Dictionary<Vector3Int, SpaceSector> activeSectors = new Dictionary<Vector3Int, SpaceSector>();
        SectorSaveRepository saveRepository;
        Vector3Int previousSector;
        float nextAutoSaveTime;
        bool initialized;
        bool quitting;

        public SectorContentGenerator ContentGenerator => contentGenerator;

        void Awake()
        {
            Initialize();
        }

        void Start()
        {
            if (!initialized)
                return;

            RefreshLoadedSectors();
        }

        void Update()
        {
            if (!initialized || target == null)
                return;

            currentSector = SectorCoordinateUtility.WorldToSector(target.position, sectorSize);

            if (currentSector != previousSector)
            {
                previousSector = currentSector;
                RefreshLoadedSectors();
            }

            if (autoSaveInterval > 0f && Time.unscaledTime >= nextAutoSaveTime)
            {
                SaveAllLoadedSectors();
                nextAutoSaveTime = Time.unscaledTime + autoSaveInterval;
            }

            activeSectorCount = activeSectors.Count;
        }

        void OnApplicationQuit()
        {
            quitting = true;
            SaveAllLoadedSectors();
            PrepareAllForUnload();
        }

        void OnDestroy()
        {
            if (!initialized || quitting)
                return;

            SaveAllLoadedSectors();
            PrepareAllForUnload();
        }

        public void RefreshLoadedSectors()
        {
            if (!initialized || target == null)
                return;

            currentSector = SectorCoordinateUtility.WorldToSector(target.position, sectorSize);
            previousSector = currentSector;

            HashSet<Vector3Int> desiredSectors = BuildDesiredSectorSet(currentSector);

            foreach (Vector3Int coordinate in desiredSectors)
            {
                if (!activeSectors.ContainsKey(coordinate))
                    LoadSector(coordinate);
            }

            List<Vector3Int> sectorsToUnload = new List<Vector3Int>();
            foreach (KeyValuePair<Vector3Int, SpaceSector> pair in activeSectors)
            {
                if (!desiredSectors.Contains(pair.Key))
                    sectorsToUnload.Add(pair.Key);
            }

            for (int i = 0; i < sectorsToUnload.Count; i++)
                UnloadSector(sectorsToUnload[i]);

            activeSectorCount = activeSectors.Count;
        }

        public void SaveSector(SpaceSector sector)
        {
            if (!initialized || sector == null || sector.Data == null)
                return;

            sector.CaptureState();
            saveRepository.SaveSector(sector.Data);
        }

        public void SaveAllLoadedSectors()
        {
            if (!initialized)
                return;

            foreach (SpaceSector sector in activeSectors.Values)
                SaveSector(sector);
        }

        public void DeleteCurrentWorldSave()
        {
            if (saveRepository == null)
                saveRepository = new JsonSectorSaveRepository(saveSlot);

            saveRepository.DeleteWorld();

            if (debugEnabled)
                Debug.Log($"Deleted sector world save: {saveRepository.SaveDirectory}", this);
        }

        public void ReportMissingPrefab(string prefabId, Vector3Int coordinate)
        {
            if (debugEnabled)
                Debug.LogError($"Prefab id '{prefabId}' is not registered. Sector: {coordinate}.", this);
        }

        void Initialize()
        {
            if (target == null)
            {
                if (debugEnabled)
                    Debug.LogError("SpaceSectorManager target is not assigned.", this);
                return;
            }

            if (contentGenerator == null)
            {
                if (debugEnabled)
                    Debug.LogError("SpaceSectorManager contentGenerator is not assigned.", this);
                return;
            }

            sectorSize = Mathf.Max(1f, sectorSize);
            loadRadius.x = Mathf.Max(0, loadRadius.x);
            loadRadius.y = Mathf.Max(0, loadRadius.y);
            loadRadius.z = Mathf.Max(0, loadRadius.z);

            saveRepository = new JsonSectorSaveRepository(saveSlot);
            worldSeed = saveRepository.InitializeWorld(worldSeed);
            saveDirectory = saveRepository.SaveDirectory;

            currentSector = SectorCoordinateUtility.WorldToSector(target.position, sectorSize);
            previousSector = currentSector;
            nextAutoSaveTime = Time.unscaledTime + autoSaveInterval;
            initialized = true;
        }

        HashSet<Vector3Int> BuildDesiredSectorSet(Vector3Int center)
        {
            HashSet<Vector3Int> result = new HashSet<Vector3Int>();

            for (int x = -loadRadius.x; x <= loadRadius.x; x++)
            for (int y = -loadRadius.y; y <= loadRadius.y; y++)
            for (int z = -loadRadius.z; z <= loadRadius.z; z++)
                result.Add(center + new Vector3Int(x, y, z));

            return result;
        }

        void LoadSector(Vector3Int coordinate)
        {
            SectorData data;

            if (!saveRepository.TryLoadSector(coordinate, out data))
            {
                data = contentGenerator.Generate(coordinate, sectorSize, worldSeed);
                saveRepository.SaveSector(data);
            }
            else if (contentGenerator.PrepareLoadedData(data, worldSeed))
            {
                saveRepository.SaveSector(data);
            }

            GameObject sectorObject = new GameObject();
            sectorObject.transform.SetParent(transform, false);

            SpaceSector sector = sectorObject.AddComponent<SpaceSector>();
            sector.Initialize(this, data);
            activeSectors.Add(coordinate, sector);

            if (debugEnabled)
                Debug.Log($"Loaded sector {coordinate}.", this);
        }

        void UnloadSector(Vector3Int coordinate)
        {
            if (!activeSectors.TryGetValue(coordinate, out SpaceSector sector))
                return;

            sector.PrepareForUnload();
            saveRepository.SaveSector(sector.Data);
            activeSectors.Remove(coordinate);
            Destroy(sector.gameObject);

            if (debugEnabled)
                Debug.Log($"Unloaded sector {coordinate}.", this);
        }

        void PrepareAllForUnload()
        {
            foreach (SpaceSector sector in activeSectors.Values)
            {
                if (sector != null)
                    sector.PrepareForUnload();
            }
        }

        void OnDrawGizmosSelected()
        {
            if (!gizmosEnabled || target == null || sectorSize <= 0f)
                return;

            Vector3Int center = SectorCoordinateUtility.WorldToSector(target.position, sectorSize);

            for (int x = -loadRadius.x; x <= loadRadius.x; x++)
            for (int y = -loadRadius.y; y <= loadRadius.y; y++)
            for (int z = -loadRadius.z; z <= loadRadius.z; z++)
            {
                Vector3Int coordinate = center + new Vector3Int(x, y, z);
                Gizmos.DrawWireCube(
                    SectorCoordinateUtility.SectorToCenter(coordinate, sectorSize),
                    Vector3.one * sectorSize);
            }
        }
    }
}
