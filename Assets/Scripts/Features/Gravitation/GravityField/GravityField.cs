using UnityEngine;

public abstract class GravityField : MonoBehaviour
{
    [Header("Gravity provider")]
    public GravityProvider provider;

    [Header("Composition")]
    public GravityBlendMode blendMode = GravityBlendMode.Override;

    public int priority;

    public GravityProvider Provider => provider;

    public GravityBlendMode BlendMode => blendMode;

    public int Priority => priority;


    public void SetProvider(GravityProvider newProvider)
    {
        provider = newProvider;
    }

    public bool TryGetAcceleration(
        Rigidbody body,
        out Vector3 acceleration)
    {
        acceleration = Vector3.zero;

        if (!isActiveAndEnabled)
            return false;

        if (provider == null || !provider.isActiveAndEnabled)
            return false;

        acceleration = provider.GetAcceleration(body);

        return IsFinite(acceleration);
    }

    static bool IsFinite(Vector3 value)
    {
        return
            !float.IsNaN(value.x) &&
            !float.IsNaN(value.y) &&
            !float.IsNaN(value.z) &&
            !float.IsInfinity(value.x) &&
            !float.IsInfinity(value.y) &&
            !float.IsInfinity(value.z);
    }
}