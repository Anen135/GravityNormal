// Core layer
// Base component
using UnityEngine;
using UnityEngine.Events;
namespace Lucy.Core
{
    public abstract class Interaction : MonoBehaviour {
        [Header("Debug")]
        public bool debugEnabled = false;
        public bool gizmosEnabled = false;

        [Header("Runtime")]
        public Interactable CurrentTarget;
        public UnityEvent<Interactable> OnTargetChanged;

        public abstract Interactable Detect();
        public virtual void Interact()
        {
            if (CurrentTarget == null || !CurrentTarget.canInteract) return;
            if (debugEnabled) Debug.Log($"Interacting with {CurrentTarget.name}");
            CurrentTarget?.Interact(this);
        }
    }
}
