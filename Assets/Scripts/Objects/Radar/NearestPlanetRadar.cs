using UnityEngine;

public abstract class NearestPlanetRadar : SpaceShipRadar
{
    [Header("Settings Nearest Planet Radar")]
    [Min(0f)] public float detectionRadius = 2000f;
    [Min(0f)] public float scanInterval = 0.1f;

    [Header("Debug")]
    public RadarPlanet currentPlanet;
    public float currentDistance;
    public Vector3 currentDirection;

    float nextScanTime;

    protected override void OnRadarEnabled() => Scan();

    protected override void OnRadarDisabled()
    {
        currentPlanet = null;
        currentDistance = 0f;
        currentDirection = Vector3.zero;
        OnTargetCleared();
        NotifyDataChanged();
    }

    protected override void UpdateRadar()
    {
        if (Time.time < nextScanTime) return;

        Scan();

        nextScanTime = scanInterval <= 0f
            ? Time.time
            : Time.time + scanInterval;
    }

    public void Scan()
    {
        RadarPlanet previousPlanet = currentPlanet;
        float previousDistance = currentDistance;

        currentPlanet = FindNearestPlanet();

        if (currentPlanet == null)
        {
            currentDistance = 0f;
            currentDirection = Vector3.zero;
            OnTargetCleared();

            if (previousPlanet != null) NotifyDataChanged();
            return;
        }

        Vector3 offset = currentPlanet.CenterPosition - OriginPosition;

        currentDistance = offset.magnitude;
        currentDirection = currentDistance > 0.0001f
            ? offset / currentDistance
            : Vector3.zero;

        OnTargetUpdated(currentPlanet);

        if (previousPlanet != currentPlanet || !Mathf.Approximately(previousDistance, currentDistance))
            NotifyDataChanged();
    }

    RadarPlanet FindNearestPlanet()
    {
        RadarPlanet nearest = null;
        float nearestDistanceSqr = float.MaxValue;
        float radiusSqr = detectionRadius * detectionRadius;
        var planets = RadarPlanet.ActivePlanets;

        for (int i = 0; i < planets.Count; i++)
        {
            RadarPlanet planet = planets[i];

            if (planet == null || !planet.isActiveAndEnabled) continue;

            float distanceSqr = (planet.CenterPosition - OriginPosition).sqrMagnitude;

            if (distanceSqr > radiusSqr || distanceSqr >= nearestDistanceSqr) continue;

            nearestDistanceSqr = distanceSqr;
            nearest = planet;
        }

        return nearest;
    }

    protected virtual void OnTargetUpdated(RadarPlanet planet) { }
    protected virtual void OnTargetCleared() { }
}
