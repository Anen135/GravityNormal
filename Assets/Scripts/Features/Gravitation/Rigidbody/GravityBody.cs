using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Rigidbody))]
public sealed class GravityBody : MonoBehaviour
{
    struct ActiveField
    {
        public GravityField Field;
        public ulong Sequence;
    }

    public Rigidbody rb;

    List<ActiveField> activeFields = new();

    ulong registrationSequence;

    bool useRigidBodyGravity = false;

    public Vector3 currentAcceleration;
    public Vector3 CurrentGravityDirection => currentAcceleration.sqrMagnitude < 0.0001f ? Vector3.zero : currentAcceleration.normalized;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void OnDisable()
    {
        currentAcceleration = Vector3.zero;
    }

    void FixedUpdate()
    {
        if (useRigidBodyGravity) rb.useGravity = false;

        Vector3 additiveAcceleration = Vector3.zero;
        Vector3 overrideAcceleration = Vector3.zero;

        GravityField selectedOverride = null;

        int selectedPriority = int.MinValue;
        ulong selectedSequence = 0;

        for (int i = activeFields.Count - 1; i >= 0; i--)
        {
            ActiveField entry = activeFields[i];

            if (entry.Field == null)
            {
                activeFields.RemoveAt(i);
                continue;
            }

            if (!entry.Field.TryGetAcceleration( rb, out Vector3 acceleration))
            {
                continue;
            }

            if (entry.Field.BlendMode == GravityBlendMode.Additive)
            {
                additiveAcceleration += acceleration;
                continue;
            }

            bool higherPriority =
                entry.Field.Priority > selectedPriority;

            bool newerWithSamePriority =
                entry.Field.Priority == selectedPriority &&
                entry.Sequence > selectedSequence;

            if (!higherPriority && !newerWithSamePriority)
                continue;

            selectedOverride = entry.Field;
            selectedPriority = entry.Field.Priority;
            selectedSequence = entry.Sequence;
            overrideAcceleration = acceleration;
        }

        currentAcceleration = additiveAcceleration;

        if (selectedOverride != null)
            currentAcceleration += overrideAcceleration;

        if (rb.isKinematic)
            return;

        if (currentAcceleration.sqrMagnitude < 0.0001f)
            return;

        rb.AddForce( currentAcceleration, ForceMode.Acceleration);
    }

    public void AddField(GravityField field)
    {
        if (field == null)
            return;

        for (int i = 0; i < activeFields.Count; i++)
        {
            if (activeFields[i].Field == field)
                return;
        }

        activeFields.Add( new ActiveField(){ Field = field, Sequence = ++registrationSequence });
    }

    public void RemoveField(GravityField field)
    {
        if (field == null)
            return;

        for (int i = activeFields.Count - 1; i >= 0; i--)
        {
            if (activeFields[i].Field == field)
                activeFields.RemoveAt(i);
        }
    }

    public void DisableDefaultGravity()
    {
        useRigidBodyGravity = true;
        rb.useGravity = false;
    }

    public static GravityBody Ensure(Rigidbody body)
    {
        if (body == null) return null;
        if (body.TryGetComponent<GravityBody>(out var gravityBody)) return gravityBody;
        return body.gameObject.AddComponent<GravityBody>();
    }
}