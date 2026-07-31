using System;
using UnityEngine;
using Lucy.Core;
using Lucy.Shared.Extensions;

namespace Lucy.Features.Triggers.Runtime
{
    [RequireComponent(typeof(Collider))]
    public class DestroyTrigger : MonoBehaviour
    {
        [Tooltip("Effects to apply on trigger")]
        [SerializeField] Effect[] effects;
        [Tooltip("Layers to trigger on")]
        [SerializeField] LayerMask layerMask;
        public Action<GameObject> OnTriggered;

        private void OnTriggerEnter(Collider other)
        {
            if (!layerMask.Contains(other.gameObject.layer)) return;
            Destroy(gameObject);
        }

        void OnDestroy()
        {
            OnTriggered?.Invoke(gameObject);
            Execute();
        }

        void Execute()
        {
            if (effects.Length == 0) return;
            foreach (var effect in effects) effect.Execute();
        }
    }
}