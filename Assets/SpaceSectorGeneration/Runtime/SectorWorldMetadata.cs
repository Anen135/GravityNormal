using System;

namespace Lucy.Features.SpaceSectors.Runtime
{
    [Serializable]
    public sealed class SectorWorldMetadata
    {
        public int version = 1;
        public int worldSeed;
    }
}
