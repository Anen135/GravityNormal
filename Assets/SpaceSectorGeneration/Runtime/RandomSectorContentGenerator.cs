using System;
using System.Collections.Generic;
using UnityEngine;

namespace Lucy.Features.SpaceSectors.Runtime
{
    public sealed class RandomSectorContentGenerator : SectorContentGenerator
    {
        #region Inspector

        [Header("Settings Random Sector Content Generator")]
        public List<SectorSpawnRule> spawnRules = new List<SectorSpawnRule>();
        [Min(1)] public int placementAttempts = 32;

        [Header("Debug")]
        public bool debugEnabled;

        #endregion

        public override SectorData Generate(Vector3Int coordinate, float sectorSize, int worldSeed)
        {
            SectorData data = new SectorData
            {
                coordinate = coordinate,
                generated = true
            };

            System.Random random = new System.Random(GetSectorSeed(worldSeed, coordinate));
            List<Vector3> occupiedPositions = new List<Vector3>();
            Vector3 sectorOrigin = SectorCoordinateUtility.SectorToOrigin(coordinate, sectorSize);

            for (int ruleIndex = 0; ruleIndex < spawnRules.Count; ruleIndex++)
            {
                SectorSpawnRule rule = spawnRules[ruleIndex];
                if (rule == null || rule.prefab == null || string.IsNullOrWhiteSpace(rule.prefabId))
                    continue;

                if (random.NextDouble() > rule.sectorChance)
                    continue;

                int minCount = Mathf.Max(0, rule.minCount);
                int maxCount = Mathf.Max(minCount, rule.maxCount);
                int count = random.Next(minCount, maxCount + 1);

                for (int objectIndex = 0; objectIndex < count; objectIndex++)
                {
                    if (!TryFindPosition(random, sectorOrigin, sectorSize, rule, occupiedPositions, out Vector3 position))
                    {
                        if (debugEnabled)
                            Debug.LogWarning($"Failed to place '{rule.prefabId}' in sector {coordinate}.", this);

                        continue;
                    }

                    float minScale = Mathf.Min(rule.minScale, rule.maxScale);
                    float maxScale = Mathf.Max(rule.minScale, rule.maxScale);
                    float scale = Mathf.Lerp(minScale, maxScale, (float)random.NextDouble());

                    Quaternion rotation = rule.randomRotation
                        ? Quaternion.Euler(
                            (float)random.NextDouble() * 360f,
                            (float)random.NextDouble() * 360f,
                            (float)random.NextDouble() * 360f)
                        : Quaternion.identity;

                    string objectId = $"{coordinate.x}_{coordinate.y}_{coordinate.z}:{ruleIndex}:{objectIndex}";
                    string displayName = rule.generatePlanetName
                        ? PlanetNameGenerator.Generate(GetObjectNameSeed(worldSeed, objectId))
                        : string.Empty;

                    data.objects.Add(new SectorObjectData
                    {
                        objectId = objectId,
                        prefabId = rule.prefabId,
                        displayName = displayName,
                        position = position,
                        rotation = rotation,
                        scale = Vector3.one * scale,
                        active = true,
                        destroyed = false
                    });

                    occupiedPositions.Add(position);
                }
            }

            return data;
        }

        public override bool TryGetPrefab(string prefabId, out GameObject prefab)
        {
            for (int i = 0; i < spawnRules.Count; i++)
            {
                SectorSpawnRule rule = spawnRules[i];
                if (rule == null || rule.prefabId != prefabId || rule.prefab == null)
                    continue;

                prefab = rule.prefab;
                return true;
            }

            prefab = null;
            return false;
        }

        public override bool PrepareLoadedData(SectorData data, int worldSeed)
        {
            if (data == null || data.objects == null)
                return false;

            bool changed = false;

            for (int i = 0; i < data.objects.Count; i++)
            {
                SectorObjectData objectData = data.objects[i];
                if (objectData == null || !string.IsNullOrWhiteSpace(objectData.displayName))
                    continue;

                if (!TryGetSpawnRule(objectData.prefabId, out SectorSpawnRule rule) || !rule.generatePlanetName)
                    continue;

                objectData.displayName = PlanetNameGenerator.Generate(
                    GetObjectNameSeed(worldSeed, objectData.objectId));

                changed = true;
            }

            return changed;
        }

        void OnValidate()
        {
            for (int i = 0; i < spawnRules.Count; i++)
            {
                SectorSpawnRule rule = spawnRules[i];
                if (rule == null)
                    continue;

                if (string.IsNullOrWhiteSpace(rule.prefabId) && rule.prefab != null)
                    rule.prefabId = rule.prefab.name;

                rule.maxCount = Mathf.Max(rule.minCount, rule.maxCount);
                rule.maxScale = Mathf.Max(rule.minScale, rule.maxScale);
            }
        }

        bool TryFindPosition(
            System.Random random,
            Vector3 sectorOrigin,
            float sectorSize,
            SectorSpawnRule rule,
            List<Vector3> occupiedPositions,
            out Vector3 position)
        {
            float padding = Mathf.Clamp(rule.borderPadding, 0f, sectorSize * 0.49f);
            float min = padding;
            float max = sectorSize - padding;

            for (int attempt = 0; attempt < placementAttempts; attempt++)
            {
                position = sectorOrigin + new Vector3(
                    Mathf.Lerp(min, max, (float)random.NextDouble()),
                    Mathf.Lerp(min, max, (float)random.NextDouble()),
                    Mathf.Lerp(min, max, (float)random.NextDouble()));

                if (rule.minimumSpacing <= 0f || IsFarEnough(position, occupiedPositions, rule.minimumSpacing))
                    return true;
            }

            position = default;
            return false;
        }

        bool IsFarEnough(Vector3 position, List<Vector3> occupiedPositions, float minimumSpacing)
        {
            float minimumSpacingSqr = minimumSpacing * minimumSpacing;

            for (int i = 0; i < occupiedPositions.Count; i++)
            {
                if ((occupiedPositions[i] - position).sqrMagnitude < minimumSpacingSqr)
                    return false;
            }

            return true;
        }

        int GetSectorSeed(int worldSeed, Vector3Int coordinate)
        {
            unchecked
            {
                int hash = worldSeed;
                hash = hash * 397 ^ coordinate.x;
                hash = hash * 397 ^ coordinate.y;
                hash = hash * 397 ^ coordinate.z;
                return hash;
            }
        }

        bool TryGetSpawnRule(string prefabId, out SectorSpawnRule result)
        {
            for (int i = 0; i < spawnRules.Count; i++)
            {
                SectorSpawnRule rule = spawnRules[i];
                if (rule == null || rule.prefabId != prefabId)
                    continue;

                result = rule;
                return true;
            }

            result = null;
            return false;
        }

        int GetObjectNameSeed(int worldSeed, string objectId)
        {
            unchecked
            {
                int hash = worldSeed;

                if (!string.IsNullOrEmpty(objectId))
                {
                    for (int i = 0; i < objectId.Length; i++)
                        hash = hash * 31 + objectId[i];
                }

                hash = hash * 397 ^ 0x4E414D45;
                return hash;
            }
        }
    }
}
