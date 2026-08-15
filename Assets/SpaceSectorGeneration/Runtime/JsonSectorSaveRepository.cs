using System.IO;
using UnityEngine;

namespace Lucy.Features.SpaceSectors.Runtime
{
    public sealed class JsonSectorSaveRepository : SectorSaveRepository
    {
        readonly string saveDirectory;
        readonly string metadataPath;

        public override string SaveDirectory => saveDirectory;

        public JsonSectorSaveRepository(string saveSlot)
        {
            string safeSlot = SanitizeFileName(string.IsNullOrWhiteSpace(saveSlot) ? "Default" : saveSlot);
            saveDirectory = Path.Combine(Application.persistentDataPath, "SpaceSectors", safeSlot);
            metadataPath = Path.Combine(saveDirectory, "world.json");
        }

        public override int InitializeWorld(int configuredWorldSeed)
        {
            Directory.CreateDirectory(saveDirectory);

            if (File.Exists(metadataPath))
            {
                string json = File.ReadAllText(metadataPath);
                SectorWorldMetadata metadata = JsonUtility.FromJson<SectorWorldMetadata>(json);
                if (metadata != null)
                    return metadata.worldSeed;
            }

            SectorWorldMetadata newMetadata = new SectorWorldMetadata
            {
                worldSeed = configuredWorldSeed
            };

            WriteJson(metadataPath, JsonUtility.ToJson(newMetadata, true));
            return configuredWorldSeed;
        }

        public override bool TryLoadSector(Vector3Int coordinate, out SectorData data)
        {
            string path = GetSectorPath(coordinate);
            if (!File.Exists(path))
            {
                data = null;
                return false;
            }

            string json = File.ReadAllText(path);
            data = JsonUtility.FromJson<SectorData>(json);
            return data != null;
        }

        public override void SaveSector(SectorData data)
        {
            if (data == null)
                return;

            Directory.CreateDirectory(saveDirectory);
            WriteJson(GetSectorPath(data.coordinate), JsonUtility.ToJson(data, true));
        }

        public override void DeleteWorld()
        {
            if (Directory.Exists(saveDirectory))
                Directory.Delete(saveDirectory, true);
        }

        string GetSectorPath(Vector3Int coordinate)
        {
            return Path.Combine(saveDirectory, $"sector_{coordinate.x}_{coordinate.y}_{coordinate.z}.json");
        }

        void WriteJson(string path, string json)
        {
            string temporaryPath = path + ".tmp";
            File.WriteAllText(temporaryPath, json);
            File.Copy(temporaryPath, path, true);
            File.Delete(temporaryPath);
        }

        string SanitizeFileName(string value)
        {
            foreach (char invalidChar in Path.GetInvalidFileNameChars())
                value = value.Replace(invalidChar, '_');

            return value;
        }
    }
}
