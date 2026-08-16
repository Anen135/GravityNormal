using UnityEngine;

namespace Lucy.Features.SpaceSectors.Runtime
{
    public abstract class SectorContentGenerator : MonoBehaviour
    {
        public abstract SectorData Generate(Vector3Int coordinate, float sectorSize, int worldSeed);
        public abstract bool TryGetPrefab(string prefabId, out GameObject prefab);

        public virtual bool PrepareLoadedData(SectorData data, int worldSeed)
        {
            return false;
        }
    }
}
