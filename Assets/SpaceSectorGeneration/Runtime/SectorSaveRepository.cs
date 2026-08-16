using UnityEngine;

namespace Lucy.Features.SpaceSectors.Runtime
{
    public abstract class SectorSaveRepository
    {
        public abstract string SaveDirectory { get; }
        public abstract int InitializeWorld(int configuredWorldSeed);
        public abstract bool TryLoadSector(Vector3Int coordinate, out SectorData data);
        public abstract void SaveSector(SectorData data);
        public abstract void DeleteWorld();
    }
}
