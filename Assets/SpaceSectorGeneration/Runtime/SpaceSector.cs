using System.Collections.Generic;
using UnityEngine;

namespace Lucy.Features.SpaceSectors.Runtime
{
    public sealed class SpaceSector : MonoBehaviour
    {
        #region Inspector

        [Header("Debug")]
        public SectorData data;
        public List<SectorPersistentObject> spawnedObjects = new List<SectorPersistentObject>();

        #endregion

        SpaceSectorManager manager;
        bool unloading;

        public SectorData Data => data;

        public void Initialize(SpaceSectorManager sectorManager, SectorData sectorData)
        {
            manager = sectorManager;
            data = sectorData;
            name = $"Sector [{data.coordinate.x}, {data.coordinate.y}, {data.coordinate.z}]";
            SpawnSavedObjects();
        }

        public void CaptureState()
        {
            if (data == null)
                return;

            for (int i = spawnedObjects.Count - 1; i >= 0; i--)
            {
                SectorPersistentObject persistentObject = spawnedObjects[i];
                if (persistentObject == null)
                {
                    spawnedObjects.RemoveAt(i);
                    continue;
                }

                SectorObjectData objectData = FindObjectData(persistentObject.objectId);
                if (objectData == null || objectData.destroyed)
                    continue;

                Transform objectTransform = persistentObject.transform;
                objectData.position = objectTransform.position;
                objectData.rotation = objectTransform.rotation;
                objectData.scale = objectTransform.localScale;
                objectData.active = persistentObject.gameObject.activeSelf;
            }
        }

        public void SaveNow()
        {
            if (manager != null)
                manager.SaveSector(this);
        }

        public void NotifyObjectDestroyed(string objectId)
        {
            if (unloading || data == null)
                return;

            SectorObjectData objectData = FindObjectData(objectId);
            if (objectData == null)
                return;

            objectData.destroyed = true;
            manager?.SaveSector(this);
        }

        public void PrepareForUnload()
        {
            unloading = true;
            CaptureState();

            for (int i = 0; i < spawnedObjects.Count; i++)
            {
                if (spawnedObjects[i] != null)
                    spawnedObjects[i].PrepareForSectorUnload();
            }
        }

        void SpawnSavedObjects()
        {
            for (int i = 0; i < data.objects.Count; i++)
            {
                SectorObjectData objectData = data.objects[i];
                if (objectData == null || objectData.destroyed)
                    continue;

                if (!manager.ContentGenerator.TryGetPrefab(objectData.prefabId, out GameObject prefab))
                {
                    manager.ReportMissingPrefab(objectData.prefabId, data.coordinate);
                    continue;
                }

                GameObject instance = Instantiate(
                    prefab,
                    objectData.position,
                    objectData.rotation,
                    transform);

                instance.transform.localScale = objectData.scale;

                if (!string.IsNullOrWhiteSpace(objectData.displayName))
                    instance.name = objectData.displayName;

                SectorPersistentObject persistentObject = instance.GetComponent<SectorPersistentObject>();
                if (persistentObject == null)
                    persistentObject = instance.AddComponent<SectorPersistentObject>();

                persistentObject.Initialize(this, objectData);
                spawnedObjects.Add(persistentObject);
                instance.SetActive(objectData.active);
            }
        }

        SectorObjectData FindObjectData(string objectId)
        {
            for (int i = 0; i < data.objects.Count; i++)
            {
                SectorObjectData objectData = data.objects[i];
                if (objectData != null && objectData.objectId == objectId)
                    return objectData;
            }

            return null;
        }

        void OnDestroy()
        {
            if (unloading)
                return;

            for (int i = 0; i < spawnedObjects.Count; i++)
            {
                if (spawnedObjects[i] != null)
                    spawnedObjects[i].PrepareForSectorUnload();
            }
        }
    }
}
