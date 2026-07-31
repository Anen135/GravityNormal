using System;
using System.Collections.Generic;
using UnityEngine;

namespace Lucy.Features.Minigames
{
    public class MiniGame : MonoBehaviour
    {
        public enum Result { Success, Failure, Cancelled, Timeout }

        [Header("Settings")]
        public string miniGameName = "Unnamed Minigame";
        public string miniGameDescription = "No description";

        [Header("References")]
        public List<Component> toggleTargets;

        [Header("Runtime")]
        public bool isActive = false;
        public bool isCompleted = false;

        public Action OnStarted;
        public Action<Result> OnFinished;

        public void ToggleComponents()
        {
            toggleTargets.ForEach(t =>
            {
                if (t is MonoBehaviour mb) mb.enabled = !mb.enabled;
                else if (t is Collider col) col.enabled = !col.enabled;
                else if (t is Renderer rend) rend.enabled = !rend.enabled;
            });
        }

        public virtual void StartMinigame()
        {
            isActive = true;
            ToggleComponents();
            OnStarted?.Invoke();
        }

        public virtual void FinishMinigame(Result result = Result.Success)
        {
            isActive = false;
            ToggleComponents();
            OnFinished?.Invoke(result);
        }


    }
}