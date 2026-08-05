using System.Collections.Generic;
using Lucy.Core.Extensions;
using UnityEngine;
[DisallowMultipleComponent]
[RequireComponent(typeof(Collider))]
public sealed class GravityZone : GravityField
{    
    [System.Serializable]
    public struct EnterSettings
    {
        public bool useDefaultGravity;
        public bool useEnterVelocity;
        public bool wakeUpOnEnter;
        public Vector3 linearVelocityOnEnter;
        public Vector3 angularVelocityOnEnter;
    }
	[Header("Enter settings")]
    public EnterSettings enterSettings = new();    
    public Collider triggerCollider;
    Dictionary<GravityBody, int> overlaps = new();
    
	void OnValidate()
    {
        triggerCollider = GetComponent<Collider>();
        triggerCollider.isTrigger = true;
        
    }
    
	void OnTriggerEnter(Collider other)
    {
        if (!layerMask.Contains(other.gameObject.layer) || !other.attachedRigidbody) return;
        var gravityBody = other.attachedRigidbody.RequireComponent<GravityBody>();
        if (overlaps.TryGetValue(gravityBody, out int overlapCount))
        {
            overlaps[gravityBody] = overlapCount + 1;
            return;
        }
        overlaps.Add(gravityBody, 1);
        gravityBody.AddField(this);
        ApplyEnterSettings(other.attachedRigidbody);
    }
    
	void OnTriggerExit(Collider other)
    {
        if (other.attachedRigidbody == null) return;
        if (!other.attachedRigidbody.TryGetComponent<GravityBody>(out var gravityBody)) return;
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
    void OnEnable() => triggerCollider.enabled = true;
	void OnDisable()
    {
        triggerCollider.enabled = false;
        foreach (var pair in overlaps)
        {
            if (pair.Key != null) pair.Key.RemoveField(this);
        }
        overlaps.Clear();
    }
    
	void ApplyEnterSettings( Rigidbody rb)
    {
        if (rb.isKinematic) return;
        rb.useGravity = enterSettings.useDefaultGravity;
        if (enterSettings.useEnterVelocity)
        {
            rb.linearVelocity = enterSettings.linearVelocityOnEnter;
            rb.angularVelocity = enterSettings.angularVelocityOnEnter;
        }
        if (enterSettings.wakeUpOnEnter) rb.WakeUp();
    }
}