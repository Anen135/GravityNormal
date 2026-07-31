using UnityEngine;
namespace Lucy.Shared.Extensions
{
    public static class Vector3Extensions
    {
        public static bool IsFinite(this Vector3 v) => float.IsFinite(v.x) && float.IsFinite(v.y) && float.IsFinite(v.z);
    }
}