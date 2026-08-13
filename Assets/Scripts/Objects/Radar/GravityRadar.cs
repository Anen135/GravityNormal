using UnityEngine;

[DisallowMultipleComponent]
public sealed class GravityRadar : SpaceShipRadar
{
    [Header("Dependencies Gravity Radar")]
    public GravityBeam beam;

    [Header("Settings Gravity Radar")]
    [Min(0f)] public float minimumAcceleration = 0.001f;

    [Header("Debug")]
    public RadarPlanet currentPlanet;
    public Vector3 currentDirection;
    public float currentDistance;
    public float currentAcceleration;

    void OnValidate()
    {
        beam = beam != null ? beam : GetComponent<GravityBeam>();
    }

    protected override void OnRadarEnabled() => Scan();
    protected override void OnRadarDisabled() => ClearTarget();
    protected override void UpdateRadar() => Scan();

    void Scan()
    {
        RadarPlanet previousPlanet = currentPlanet;

        currentPlanet = FindNearestActivePlanet();

        if (currentPlanet == null)
        {
            ClearTarget();

            if (previousPlanet != null) NotifyDataChanged();
            return;
        }

        Vector3 offset = currentPlanet.CenterPosition - OriginPosition;

        currentDistance = offset.magnitude;
        currentDirection = currentDistance > 0.0001f
            ? offset / currentDistance
            : Vector3.zero;

        currentAcceleration = 0f;

        if (
            gravityBody != null &&
            gravityBody.rb != null &&
            currentPlanet.gravityField != null &&
            currentPlanet.gravityField.TryGetAcceleration(
                gravityBody.rb,
                out Vector3 acceleration
            )
        )
        {
            currentAcceleration = acceleration.magnitude;
        }

        if (currentAcceleration < minimumAcceleration)
        {
            ClearTarget();
            return;
        }

        if (beam != null) beam.Draw(OriginPosition, currentPlanet.CenterPosition);

        if (previousPlanet != currentPlanet) NotifyDataChanged();
    }

    RadarPlanet FindNearestActivePlanet()
    {
        if (gravityBody == null) return null;

        RadarPlanet nearest = null;
        float nearestDistanceSqr = float.MaxValue;

        for (int i = 0; i < gravityBody.activeFields.Count; i++)
        {
            GravityField field = gravityBody.activeFields[i];

            if (field == null) continue;

            RadarPlanet planet = RadarPlanet.FindByGravityField(field);

            if (planet == null) continue;

            float distanceSqr = (planet.CenterPosition - OriginPosition).sqrMagnitude;

            if (distanceSqr >= nearestDistanceSqr) continue;

            nearestDistanceSqr = distanceSqr;
            nearest = planet;
        }

        return nearest;
    }

    void ClearTarget()
    {
        currentPlanet = null;
        currentDirection = Vector3.zero;
        currentDistance = 0f;
        currentAcceleration = 0f;

        if (beam != null) beam.SetVisible(false);
    }

    public override string GetDisplayText()
    {
        return isRadarEnabled ? "GRAVITY VECTOR" : string.Empty;
    }
}
