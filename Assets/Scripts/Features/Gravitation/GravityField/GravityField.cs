// Base class
using UnityEngine;

public class GravityField : MonoBehaviour
{
    [Header("Settings")]
    public LayerMask layerMask = ~0;
    public GravityBlendMode blendMode = GravityBlendMode.Override;
    [Tooltip("Higher priority fields will override lower priority fields when using the Override blend mode.")]
    public int priority;

    [Header("Dependencies")]
    public GravityProvider provider;

    void OnValidate()
    {
        provider = provider != null ? provider : GetComponent<GravityProvider>();
    }

    public bool TryGetAcceleration(Rigidbody body, out Vector3 acceleration)
    {
        acceleration = Vector3.zero;
        if (!isActiveAndEnabled || provider == null || !provider.isActiveAndEnabled) return false;
        acceleration = provider.GetAcceleration(body);
        return (acceleration * 0) == Vector3.zero;
    }
}