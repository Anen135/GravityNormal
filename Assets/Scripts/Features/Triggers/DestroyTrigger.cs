using System;
using UnityEngine;
using Lucy.Core;
using Lucy.Core.Extensions;
using UnityEngine.Events;

namespace Lucy.Features.Triggers.Runtime
{
    [RequireComponent(typeof(Collider))]
    public class DestroyTrigger : MonoBehaviour
    {
        [Header("Settings")]
        [Tooltip("Layers to trigger on")]
        public LayerMask layerMask;
        public UnityEvent<GameObject> OnTriggered;

        void OnTriggerEnter(Collider other)
        {
            if (!layerMask.Contains(other.gameObject.layer)) return;
            Destroy(gameObject);
        }

        void OnDestroy()
        {
            OnTriggered?.Invoke(gameObject);
        }
    }
}