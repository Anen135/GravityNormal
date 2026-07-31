using UnityEngine;

public sealed class DirectionalGravityProvider : GravityProvider
{
    public Vector3 direction = Vector3.down;

    public bool useLocalSpace;

    [SerializeField, Min(0f)]
    float acceleration = 9.81f;

    public Vector3 WorldDirection
    {
        get
        {
            Vector3 worldDirection = useLocalSpace
                ? transform.TransformDirection(direction)
                : direction;

            if (worldDirection.sqrMagnitude < 0.0001f)
                return Vector3.zero;

            return worldDirection.normalized;
        }
    }

    public override Vector3 GetAcceleration(Rigidbody body)
    {
        return WorldDirection * acceleration;
    }

    public void SetDirection(Vector3 newDirection)
    {
        if (newDirection.sqrMagnitude < 0.0001f)
            return;

        direction = newDirection.normalized;
    }

    public void SetWorldDirection(Vector3 worldDirection)
    {
        if (worldDirection.sqrMagnitude < 0.0001f)
            return;

        worldDirection.Normalize();

        direction = useLocalSpace
            ? transform.InverseTransformDirection(worldDirection)
            : worldDirection;
    }

    public void SetAcceleration(float newAcceleration)
    {
        acceleration = Mathf.Max(0f, newAcceleration);
    }

    void OnValidate()
    {
        acceleration = Mathf.Max(0f, acceleration);
    }

    void OnDrawGizmosSelected()
    {
        Vector3 worldDirection = WorldDirection;

        if (worldDirection.sqrMagnitude < 0.0001f)
            return;

        Vector3 start = transform.position;
        Vector3 end = start + worldDirection * 3f;

        Gizmos.DrawLine(start, end);
        Gizmos.DrawSphere(end, 0.1f);
    }
}