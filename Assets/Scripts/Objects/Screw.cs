using Lucy.Core;
using UnityEngine;
using UnityEngine.Events;

public class Screw : Interactable
{
    [Header("Settings")]
    public int screwTarget = 3;
    public float rotationAmount = 45f;
    public float positionAmount = 0.5f;
    public Vector3 screwDirection = Vector3.up;
    public Vector3 screwAxis = Vector3.right;
    [Header("Event")]
    public UnityEvent OnUnscrew;
    [Header("Runtime")]
    public int screwCount = 0;
    public void DoScrew()
    {
        base.Interact(null);
        transform.Rotate(0, rotationAmount, 0);
        transform.position += transform.forward * positionAmount;
        if (screwCount >= screwTarget) OnUnscrew.Invoke();
        screwCount++;
    }
}