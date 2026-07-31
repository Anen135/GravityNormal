using System;
using UnityEngine;

namespace Lucy.Core
{
    public class Interactable : MonoBehaviour
    {
        [Header("Interaction")]
        public string interactionPrompt;
        public bool canInteract = true;

        [Header("debugEnabled")]
        public bool debugEnabled = false;

        public event Action<Interactable, Interaction> OnInteract;
        public event Action<Interactable, Interaction> OnDetect;
        public event Action<Interactable, Interaction> OnLost;

        public virtual void Detect(Interaction interactor)
        {
            if (!debugEnabled) Debug.Log($"Detect {gameObject.name}");
            OnDetect?.Invoke(this, interactor);
        }

        public virtual void Lost(Interaction interactor)
        {
            OnLost?.Invoke(this, interactor);
        }

        public virtual void Interact(Interaction interactor)
        {
            if (!canInteract) return;
            OnInteract?.Invoke(this, interactor);
        }

        public void SetInteractable(bool value) => canInteract = value;
    }
}