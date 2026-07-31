using System.Collections.Generic;
using UnityEngine;
[DisallowMultipleComponent]
[RequireComponent(typeof(Collider))]
public sealed class GravityZone : GravityField
{
    
	[Header("Filtering")]
    public LayerMask affectedLayers = ~0;
    public bool autoAddGravityBody = true;
    
	[Header("Enter settings")]
    public bool wakeUpOnEnter = true;
    public bool clearLinearVelocityOnEnter;
    public bool clearAngularVelocityOnEnter;
    
	[Header("Rigidbody settings")]
    public bool useDefaultGravity = false;
    Collider triggerCollider;
    Dictionary<GravityBody, int> overlaps = new();
    
	void OnValidate()
    {
        if (triggerCollider == null) {Debug.LogWarning("Trigger collider is not assigned."); return;}
        if (!triggerCollider.isTrigger) {Debug.LogWarning("Trigger collider is not a trigger."); return;}
    }
    
	void OnTriggerEnter(Collider other)
    {
        if (!IsAffectedLayer(other.gameObject.layer)) return;
        Rigidbody body = other.attachedRigidbody;
        if (body == null) return;
        if (!TryGetComponent<GravityBody>(out var gravityBody) && autoAddGravityBody) gravityBody = GravityBody.Ensure(body);
        if (gravityBody == null) return;
        if (overlaps.TryGetValue(gravityBody, out int overlapCount))
        {
            overlaps[gravityBody] = overlapCount + 1;
            return;
        }
        overlaps.Add(gravityBody, 1);
        gravityBody.AddField(this);
        if (useDefaultGravity) gravityBody.DisableDefaultGravity();
        ApplyEnterSettings(body, gravityBody);
    }
    
	void OnTriggerExit(Collider other)
    {
        Rigidbody body = other.attachedRigidbody;
        if (body == null) return;
        if (!body.TryGetComponent<GravityBody>(out var gravityBody)) return;
        if (!overlaps.TryGetValue(gravityBody, out int overlapCount)) return;
        overlapCount--;
        if (overlapCount > 0)
        {
            overlaps[gravityBody] = overlapCount;
            return;
        }
        overlaps.Remove(gravityBody);
        gravityBody.RemoveField(this);
    }
    
	void OnDisable()
    {
        foreach (var pair in overlaps)
        {
            if (pair.Key != null) pair.Key.RemoveField(this);
        }
        overlaps.Clear();
    }
    
	void ApplyEnterSettings( Rigidbody body, GravityBody gravityBody)
    {
        if (!useDefaultGravity) gravityBody.DisableDefaultGravity();
        if (body.isKinematic) return;
        if (clearLinearVelocityOnEnter) body.linearVelocity = Vector3.zero;
        if (clearAngularVelocityOnEnter) body.angularVelocity = Vector3.zero;
        if (wakeUpOnEnter) body.WakeUp();
    }
    bool IsAffectedLayer(int layer) => (affectedLayers.value & (1 << layer)) != 0;
}