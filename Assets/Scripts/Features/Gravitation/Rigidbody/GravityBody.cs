using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Rigidbody))]
public class GravityBody : MonoBehaviour
{
    [Header("Debug")]
    public Vector3 currentAcceleration;
    public Rigidbody rb;
    List<GravityField> activeFields = new();

    public Vector3 CurrentGravityDirection => currentAcceleration.sqrMagnitude < 0.0001f ? Vector3.zero : currentAcceleration.normalized;
    
    void Awake() => rb = GetComponent<Rigidbody>();
    void OnDisable() => currentAcceleration = Vector3.zero;

    void FixedUpdate()
    {
        Vector3 additiveAcceleration = Vector3.zero;
        Vector3 overrideAcceleration = Vector3.zero;
        GravityField selectedOverride = null;
        int selectedPriority = int.MinValue;
        int index = activeFields.Count;

        while (index-- > 0)
        {
            GravityField field = activeFields[index];

            if (field == null)
            {
                activeFields.RemoveAt(index);
                continue;
            }

            if (!field.TryGetAcceleration(rb, out Vector3 acceleration)) continue;

            if (field.blendMode == GravityBlendMode.Additive)
            {
                additiveAcceleration += acceleration;
                continue;
            }

            if (selectedOverride != null && field.priority <= selectedPriority) continue;

            selectedOverride = field;
            selectedPriority = field.priority;
            overrideAcceleration = acceleration;
        }

        currentAcceleration = additiveAcceleration + overrideAcceleration;

        if (rb.isKinematic || currentAcceleration.sqrMagnitude < 0.0001f) return;
        rb.AddForce(currentAcceleration, ForceMode.Acceleration);
    }

    public void AddField(GravityField field)
    {
        if (field == null) return;
        for (int i = 0; i < activeFields.Count; i++)
        {
            if (activeFields[i] == field) return;
        }
        activeFields.Add(field);
    }

    public void RemoveField(GravityField field)
    {
        if (field == null) return;
        for (int i = activeFields.Count - 1; i >= 0; i--)
        {
            if (activeFields[i] == field) activeFields.RemoveAt(i);
        }
    }
}