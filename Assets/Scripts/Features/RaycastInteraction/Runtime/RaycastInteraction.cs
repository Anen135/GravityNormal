// Game layer
// Variant component
using System;
using UnityEngine;
using UnityEngine.Events;
using Lucy.Core;

namespace Lucy.Features.Interactables.Runtime
{
    public class RaycastInteraction : Interaction
    {
        #region Settings
        [Header("Raycast Settings")]
        public float interactDistance = 3f;
        public LayerMask interactMask;

        [Header("References")]
        public Camera playerCamera;
        #endregion

        #region Unity Methods
        void Awake()
        {
            if (playerCamera == null)
            {
                Debug.LogWarning($"[{nameof(RaycastInteraction)}] No camera assigned. Using MainCamera.", this);
                playerCamera = Camera.main;
            }
        }

        void Update()
        {
            var detected = Detect();

            if (detected != CurrentTarget)
            {
                if (CurrentTarget != null)
                    CurrentTarget.Lost(this);

                CurrentTarget = detected;

                if (CurrentTarget != null)
                    CurrentTarget.Detect(this);

                OnTargetChanged?.Invoke(CurrentTarget);
            }
        }
        #endregion

        #region Interaction

        public override Interactable Detect()
        {
            if (playerCamera == null)
                return null;

            Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);

            if (Physics.Raycast(ray, out RaycastHit hit, interactDistance, interactMask))
            {
                // Пытаемся взять напрямую
                if (hit.collider.TryGetComponent<Interactable>(out var interactable))
                {
                    // Если у тебя нет CanInteract — просто возвращаем
                    return interactable;
                }

                // Если объект вложенный
                var parent = hit.collider.GetComponentInParent<Interactable>();
                if (parent != null)
                    return parent;
            }

            return null;
        }

        #endregion

        #region Gizmos
        void OnDrawGizmos()
        {
            if (!gizmosEnabled || playerCamera == null) return;

            Gizmos.color = Color.cyan;
            Gizmos.DrawRay(playerCamera.transform.position, playerCamera.transform.forward * interactDistance);
        }
        #endregion
    }
}