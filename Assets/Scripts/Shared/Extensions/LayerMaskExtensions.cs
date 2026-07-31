using UnityEngine;
namespace Lucy.Shared.Extensions
{
    public static class LayerMaskExtensions
    {
        public static bool Contains(this LayerMask mask, GameObject go)
        {
            if (go == null) return false;
            return (mask.value & (1 << go.layer)) != 0;
        }

        public static bool Contains(this LayerMask mask, Collider collider)
        {
            if (collider == null) return false;
            return (mask.value & (1 << collider.gameObject.layer)) != 0;
        }
        public static bool Contains(this LayerMask mask, Collider2D collider)
        {
            if (collider == null) return false;
            return (mask.value & (1 << collider.gameObject.layer)) != 0;
        }

        public static bool Contains(this LayerMask mask, int layer)
        {
            return (mask.value & (1 << layer)) != 0;
        }

        public static bool ContainsLayerName(this LayerMask mask, string layerName)
        {
            int layer = LayerMask.NameToLayer(layerName);
            if (layer == -1) return false;
            return (mask.value & (1 << layer)) != 0;
        }

        public static bool DoesNotContain(this LayerMask mask, GameObject go)
        {
            return !Contains(mask, go);
        }

        public static bool DoesNotContain(this LayerMask mask, Collider collider)
        {
            return !Contains(mask, collider);
        }
    }
}