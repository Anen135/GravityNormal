using System;
using System.Collections.Generic;
using UnityEngine;

namespace Lucy.Features.SpaceSectors.Runtime
{
    [Serializable]
    public sealed class SectorData
    {
        public Vector3Int coordinate;
        public bool generated;
        public List<SectorObjectData> objects = new List<SectorObjectData>();
    }
}
