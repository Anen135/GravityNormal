using UnityEngine;

public enum FalloffMode { Constant, InverseSquare }

public sealed class PlanetaryGravityProvider : GravityProvider
{
    [Header("Planetary Settings")]
    public Transform gravityCenter;
    public FalloffMode falloff = FalloffMode.Constant;

    [Min(0f)] public float constantAcceleration = 9.81f;

    [Header("Inverse square")]
    [Min(0f)] public float maximumAcceleration = 0f;
    [Min(0f)] public float acceleration = 9.81f;
    [Min(0.01f)] public float referenceDistance = 10f;

    [Header("Debug")]
    public bool gizmosEnabled = true;
    public Color minimumDistanceColor = Color.green;
    public Color referenceDistanceColor = Color.yellow;

    #region Properties
    public Vector3 CenterPosition => gravityCenter != null ? gravityCenter.position : transform.position;
    public float MinDistance => maximumAcceleration > 0f ? referenceDistance * Mathf.Sqrt(acceleration / maximumAcceleration) : 0f;
    #endregion
    #if UNITY_EDITOR
    void OnValidate()
    {
        gravityCenter ??= transform;
    }
    #endif
    public override Vector3 GetAcceleration(Rigidbody body)
    {
        Vector3 toCenter = CenterPosition - body.worldCenterOfMass;
        float sqrDistance = toCenter.sqrMagnitude;

        if (sqrDistance < 0.0001f) return Vector3.zero;

        float distance = Mathf.Sqrt(sqrDistance);
        Vector3 direction = toCenter / distance;

        float acceleration = falloff switch
        {
            FalloffMode.Constant => constantAcceleration,
            FalloffMode.InverseSquare => CalculateInverseSquareAcceleration(distance),
            _ => constantAcceleration
        };

        return direction * acceleration;
    }

    float CalculateInverseSquareAcceleration(float distance)
    {
        distance = Mathf.Max(distance, MinDistance);
        float ratio = referenceDistance / distance;
        return acceleration * ratio * ratio;
    }

    void OnDrawGizmosSelected()
    {
        if (!gizmosEnabled) return;

        Gizmos.color = minimumDistanceColor;
        Gizmos.DrawWireSphere(CenterPosition, MinDistance);

        Gizmos.color = referenceDistanceColor;
        Gizmos.DrawWireSphere(CenterPosition, referenceDistance);
    }
}