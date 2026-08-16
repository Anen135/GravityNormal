using System;
using UnityEngine;

namespace Lucy.Features.SpaceSectors.Runtime
{
    [Serializable]
    public sealed class SectorObjectData
    {
        public string objectId;
        public string prefabId;
        public string displayName;
        public Vector3 position;
        public Quaternion rotation = Quaternion.identity;
        public Vector3 scale = Vector3.one;
        public bool active = true;
        public bool destroyed;
    }
}
