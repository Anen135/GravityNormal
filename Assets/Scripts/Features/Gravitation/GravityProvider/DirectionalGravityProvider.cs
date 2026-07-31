using UnityEngine;

public sealed class DirectionalGravityProvider : GravityProvider
{
    [Header("Settings")]
    public Vector3 direction = Vector3.down;
    public bool useLocalSpace;
    [Header("Debug")]
    public bool gizmosEnabled;
    public Color gizmosColor = Color.green;
    [Range(0f, 10f)] public float gizmosLength = 3f;
    [Range(0f, 1f)] public float gizmosRadius = 0.1f;

    [Min(0f)]
    public float acceleration = 9.81f;

    public Vector3 WorldDirection
    {
        get
        {
            Vector3 worldDirection = useLocalSpace ? transform.TransformDirection(direction) : direction;
            if (worldDirection.sqrMagnitude < 0.0001f) return Vector3.zero;
            return worldDirection.normalized;
        }
        set
        {
            direction = useLocalSpace ? transform.InverseTransformDirection(value) : value;
        }
    }

    public override Vector3 GetAcceleration(Rigidbody body)
    {
        return WorldDirection * acceleration;
    }

    void OnDrawGizmosSelected()
    {
        if (!gizmosEnabled || WorldDirection.sqrMagnitude < 0.0001f) return;
        Gizmos.color = gizmosColor;
        Gizmos.DrawLine(transform.position, transform.position + WorldDirection * gizmosLength);
        Gizmos.DrawSphere(transform.position + WorldDirection * gizmosLength, gizmosRadius);
    }
}