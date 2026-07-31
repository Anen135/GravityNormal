using UnityEngine;
using UnityEngine.Events;
using Lucy.Core;

namespace Lucy.Features.Interactables.Runtime
{
    [RequireComponent(typeof(Interactable))]
    public class CustomInteractable : MonoBehaviour
    {
        [Header("Runtime")]
        public Interactable interactable;

        [Header("Events")]
        public UnityEvent<Interactable, Interaction> OnInteractUnity;
        public UnityEvent<Interactable, Interaction> OnDetectUnity;
        public UnityEvent<Interactable, Interaction> OnLostUnity;

        void Awake()
        {
            if (interactable == null)
                interactable = GetComponent<Interactable>();
        }

        void OnValidate()
        {
            if (interactable == null)
                interactable = GetComponent<Interactable>();
        }

        void OnEnable()
        {
            if (interactable == null) return;

            interactable.OnInteract += HandleInteract;
            interactable.OnDetect += HandleDetect;
            interactable.OnLost += HandleLost;
        }

        void OnDisable()
        {
            if (interactable == null) return;

            interactable.OnInteract -= HandleInteract;
            interactable.OnDetect -= HandleDetect;
            interactable.OnLost -= HandleLost;
        }

        protected virtual void HandleInteract(Interactable self, Interaction interactor)
        {
            OnInteractUnity?.Invoke(self, interactor);
        }

        protected virtual void HandleDetect(Interactable self, Interaction interactor)
        {
            OnDetectUnity?.Invoke(self, interactor);
        }

        protected virtual void HandleLost(Interactable self, Interaction interactor)
        {
            OnLostUnity?.Invoke(self, interactor);
        }
    }
}