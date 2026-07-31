using UnityEngine;
using Lucy.Core;
using Lucy.Shared.Extensions;

namespace Lucy.Features.Triggers.Runtime
{
    [RequireComponent(typeof(Collider))]
    public class TeleportTrigger : MonoBehaviour
    {
        [Tooltip("Where to teleport objects")]
        [SerializeField] Transform spawnPoint;
        [Tooltip("Show spawnPoint in editor")]
        [SerializeField] bool showSpawnPoint = false;
        [Tooltip("Effects to apply on teleport")]
        [SerializeField] Effect[] effects;
        [Tooltip("Layers to trigger on")]
        [SerializeField] LayerMask layerMask;

        private void OnTriggerEnter(Collider other)
        {
            if (!layerMask.Contains(other.gameObject.layer)) return;
            if (!other.TryGetComponent<CharacterController>(out var cc) && other.attachedRigidbody == null) return;
            if (cc) cc.enabled = false;
            if (other.attachedRigidbody is not null and var rb) rb.linearVelocity = rb.angularVelocity = Vector3.zero;
            other.transform.position = spawnPoint != null ? spawnPoint.position : Vector3.zero;
            if (cc) cc.enabled = true;
            Execute();
        }

        void Execute()
        {
            if (effects.Length == 0) return;
            foreach (var effect in effects) effect.Execute();
        }

        void OnDrawGizmosSelected()
        {
            if (showSpawnPoint && spawnPoint != null)
            {
                Gizmos.color = Color.cyan;
                Gizmos.DrawWireSphere(spawnPoint.position, 0.5f);
            }
        }
    }
}