using UnityEngine;
namespace Lucy.Core.Extensions
{
    public static class TransformExtensions
    {
        public static bool ClearChildren(this Transform transform)
        {
            for (int i = transform.childCount - 1; i >= 0; i--)
            {
                Transform child = transform.GetChild(i);
                if (child == null) continue;
                if (Application.isPlaying) Object.Destroy(child.gameObject);
                else Object.DestroyImmediate(child.gameObject);
            }
            return transform.childCount == 0;
        }
    }
}