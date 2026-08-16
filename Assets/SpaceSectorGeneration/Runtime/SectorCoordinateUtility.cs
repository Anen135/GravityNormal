using UnityEngine;

namespace Lucy.Features.SpaceSectors.Runtime
{
    public static class SectorCoordinateUtility
    {
        public static Vector3Int WorldToSector(Vector3 worldPosition, float sectorSize)
        {
            return new Vector3Int(
                Mathf.FloorToInt(worldPosition.x / sectorSize),
                Mathf.FloorToInt(worldPosition.y / sectorSize),
                Mathf.FloorToInt(worldPosition.z / sectorSize));
        }

        public static Vector3 SectorToOrigin(Vector3Int coordinate, float sectorSize)
        {
            return new Vector3(
                coordinate.x * sectorSize,
                coordinate.y * sectorSize,
                coordinate.z * sectorSize);
        }

        public static Vector3 SectorToCenter(Vector3Int coordinate, float sectorSize)
        {
            return SectorToOrigin(coordinate, sectorSize) + Vector3.one * (sectorSize * 0.5f);
        }
    }
}
