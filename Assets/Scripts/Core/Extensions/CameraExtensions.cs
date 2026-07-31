using UnityEngine;
namespace Lucy.Shared.Extensions
{
    public static class CameraExtensions
    {
        public static T Raycast<T>(this Camera camera, float maxDistance, LayerMask layerMask) where T : Component
        { 
            if (camera == null) return null;
            else if (Physics.Raycast(new(camera.transform.position, camera.transform.forward), out RaycastHit hit, maxDistance, layerMask) && hit.collider.TryGetComponent<T>(out var interactable)) return interactable;
            else return null;
        }
    }
}