using System;
using UnityEngine;

namespace Lucy.Features.SpaceSectors.Runtime
{
    [Serializable]
    public sealed class SectorSpawnRule
    {
        public string prefabId;
        public GameObject prefab;

        [Range(0f, 1f)] public float sectorChance = 1f;
        [Min(0)] public int minCount;
        [Min(0)] public int maxCount = 1;
        [Min(0f)] public float borderPadding = 100f;
        [Min(0f)] public float minimumSpacing;
        [Min(0.001f)] public float minScale = 1f;
        [Min(0.001f)] public float maxScale = 1f;
        public bool randomRotation = true;
        public bool generatePlanetName;
    }
}
