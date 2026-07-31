// Game layer
// Variant component
using System;
using System.Diagnostics;
using Lucy.Core;
using UnityEngine;

public class RadiusInteraction : Interaction
{
    [Header("Interaction Settings")]
    public float interactRadius = 3f;
    public LayerMask interactMask;

    private void Update()
    {
        var detected = Detect();

        if (detected != CurrentTarget)
        {
            CurrentTarget = detected;
            OnTargetChanged?.Invoke(CurrentTarget);
        }
    }

    public override Interactable Detect()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, interactRadius, interactMask);
        Interactable nearest = null;
        float minDist = float.MaxValue;

        foreach (var hit in hits)
        {
            if (hit.TryGetComponent<Interactable>(out var interactable) && interactable.canInteract)
            {
                float dist = Vector3.Distance(transform.position, hit.transform.position);
                if (dist < minDist)
                {
                    minDist = dist;
                    nearest = interactable;
                }
            }
        }

        return nearest;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactRadius);
    }
}
