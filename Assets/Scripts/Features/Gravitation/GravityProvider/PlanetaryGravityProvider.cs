using UnityEngine;

public enum PlanetaryGravityFalloff
{
    Constant,
    InverseSquare
}

public sealed class PlanetaryGravityProvider : GravityProvider
{
    public Transform gravityCenter;

    public PlanetaryGravityFalloff falloff =
        PlanetaryGravityFalloff.Constant;

    [Header("Constant")]
    [SerializeField, Min(0f)]
    float constantAcceleration = 9.81f;

    [Header("Inverse square")]
    [SerializeField, Min(0f)]
    float accelerationAtReferenceDistance = 9.81f;

    [SerializeField, Min(0.01f)]
    float referenceDistance = 10f;

    [SerializeField, Min(0.01f)]
    float minimumDistance = 1f;

    public Vector3 CenterPosition =>
        gravityCenter != null
            ? gravityCenter.position
            : transform.position;

    public override Vector3 GetAcceleration(Rigidbody body)
    {
        Vector3 toCenter =
            CenterPosition - body.worldCenterOfMass;

        float sqrDistance = toCenter.sqrMagnitude;

        if (sqrDistance < 0.0001f)
            return Vector3.zero;

        float distance = Mathf.Sqrt(sqrDistance);
        Vector3 direction = toCenter / distance;

        float acceleration = falloff switch
        {
            PlanetaryGravityFalloff.Constant =>
                constantAcceleration,

            PlanetaryGravityFalloff.InverseSquare =>
                CalculateInverseSquareAcceleration(distance),

            _ => constantAcceleration
        };

        return direction * acceleration;
    }

    float CalculateInverseSquareAcceleration(float distance)
    {
        distance = Mathf.Max(distance, minimumDistance);

        float ratio = referenceDistance / distance;

        return
            accelerationAtReferenceDistance *
            ratio *
            ratio;
    }

    void OnValidate()
    {
        constantAcceleration =
            Mathf.Max(0f, constantAcceleration);

        accelerationAtReferenceDistance =
            Mathf.Max(0f, accelerationAtReferenceDistance);

        referenceDistance =
            Mathf.Max(0.01f, referenceDistance);

        minimumDistance =
            Mathf.Max(0.01f, minimumDistance);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(CenterPosition, minimumDistance);
    }
}