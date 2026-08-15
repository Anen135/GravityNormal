using UnityEngine;

namespace Lucy.Features.SpaceSectors.Runtime
{
    public sealed class SectorPersistentObject : MonoBehaviour
    {
        #region Inspector

        [Header("Debug")]
        public string objectId;
        public string prefabId;
        public string displayName;
        public Vector3Int sectorCoordinate;
        public bool suppressDestroyNotification;

        #endregion

        SpaceSector owner;
        bool initialized;

        public string DisplayName => displayName;

        public void Initialize(SpaceSector sector, SectorObjectData data)
        {
            owner = sector;
            objectId = data.objectId;
            prefabId = data.prefabId;
            displayName = data.displayName;
            sectorCoordinate = sector.Data.coordinate;
            suppressDestroyNotification = false;
            initialized = true;
        }

        public void PrepareForSectorUnload()
        {
            suppressDestroyNotification = true;
        }

        public void SaveNow()
        {
            if (initialized && owner != null)
                owner.SaveNow();
        }

        void OnDestroy()
        {
            if (!initialized || suppressDestroyNotification || owner == null)
                return;

            owner.NotifyObjectDestroyed(objectId);
        }
    }
}
