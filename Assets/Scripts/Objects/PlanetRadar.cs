using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public sealed class RadarContact
{
    public RadarPlanet planet;

    public float distance;
    public Vector3 direction;

    public bool hasActiveGravity;

    public Vector3 gravityAcceleration;
    public float gravityAccelerationMagnitude;
}

[DisallowMultipleComponent]
public sealed class PlanetRadar : MonoBehaviour
{
    #region Inspector

    [Header("Dependencies")]
    public GravityBody gravityBody;
    public Transform origin;
    public RadarVisualization[] visualizations;

    [Header("Settings Planet Radar")]
    [Min(0f)] public float detectionRadius = 1000f;
    [Min(0f)] public float scanInterval = 0.1f;
    [Min(0)] public int maxContacts;

    [Header("Debug")]
    public bool gizmosEnabled = true;
    public int detectedPlanetCount;
    public List<RadarContact> contacts = new();

    #endregion

    #region Events

    public Action OnContactsUpdated;

    #endregion

    #region Properties

    public Vector3 OriginPosition
    {
        get
        {
            if (origin != null)
                return origin.position;

            if (gravityBody != null && gravityBody.rb != null)
                return gravityBody.rb.worldCenterOfMass;

            return transform.position;
        }
    }

    #endregion

    #region State

    float nextScanTime;

    #endregion

    #region Unity Messages

    void Awake()
    {
        gravityBody = gravityBody != null ? gravityBody : GetComponent<GravityBody>();
        origin = origin != null ? origin : transform;

        CacheVisualizations();
    }

    void OnValidate()
    {
        gravityBody = gravityBody != null ? gravityBody : GetComponent<GravityBody>();
        origin = origin != null ? origin : transform;

        CacheVisualizations();
    }

    void LateUpdate()
    {
        if (Time.time >= nextScanTime)
        {
            Scan();

            nextScanTime = scanInterval <= 0f
                ? Time.time
                : Time.time + scanInterval;
        }

        RenderVisualizations();
    }

    void OnDisable()
    {
        if (visualizations == null)
            return;

        for (int i = 0; i < visualizations.Length; i++)
        {
            if (visualizations[i] != null)
                visualizations[i].Hide();
        }
    }

    void OnDrawGizmosSelected()
    {
        if (!gizmosEnabled)
            return;

        Gizmos.DrawWireSphere(OriginPosition, detectionRadius);
    }

    #endregion

    #region Scan

    public void Scan()
    {
        contacts.Clear();

        IReadOnlyList<RadarPlanet> planets = RadarPlanet.ActivePlanets;

        Vector3 radarPosition = OriginPosition;
        float radiusSqr = detectionRadius * detectionRadius;

        for (int i = 0; i < planets.Count; i++)
        {
            RadarPlanet planet = planets[i];

            if (planet == null || !planet.isActiveAndEnabled)
                continue;

            Vector3 offset = planet.CenterPosition - radarPosition;
            float distanceSqr = offset.sqrMagnitude;

            if (distanceSqr > radiusSqr)
                continue;

            float distance = Mathf.Sqrt(distanceSqr);
            bool hasActiveGravity = IsGravityFieldActive(planet.gravityField);

            Vector3 acceleration = Vector3.zero;

            if (
                hasActiveGravity &&
                gravityBody != null &&
                gravityBody.rb != null &&
                planet.gravityField != null
            )
            {
                planet.gravityField.TryGetAcceleration(
                    gravityBody.rb,
                    out acceleration
                );
            }

            contacts.Add(
                new RadarContact
                {
                    planet = planet,
                    distance = distance,
                    direction = distance > 0.0001f
                        ? offset / distance
                        : Vector3.zero,
                    hasActiveGravity = hasActiveGravity,
                    gravityAcceleration = acceleration,
                    gravityAccelerationMagnitude = acceleration.magnitude
                }
            );
        }

        contacts.Sort((a, b) => a.distance.CompareTo(b.distance));

        if (maxContacts > 0 && contacts.Count > maxContacts)
        {
            contacts.RemoveRange(
                maxContacts,
                contacts.Count - maxContacts
            );
        }

        detectedPlanetCount = contacts.Count;

        OnContactsUpdated?.Invoke();
    }

    public void ForceScan()
    {
        Scan();
        RenderVisualizations();
    }

    bool IsGravityFieldActive(GravityField field)
    {
        if (field == null || gravityBody == null)
            return false;

        for (int i = 0; i < gravityBody.activeFields.Count; i++)
        {
            if (gravityBody.activeFields[i] == field)
                return true;
        }

        return false;
    }

    #endregion

    #region Visualizations

    void CacheVisualizations()
    {
        visualizations = GetComponents<RadarVisualization>();

        for (int i = 0; i < visualizations.Length; i++)
        {
            if (visualizations[i] != null)
                visualizations[i].radar = this;
        }
    }

    void RenderVisualizations()
    {
        if (visualizations == null)
            return;

        for (int i = 0; i < visualizations.Length; i++)
        {
            RadarVisualization visualization = visualizations[i];

            if (
                visualization == null ||
                !visualization.isActiveAndEnabled
            )
                continue;

            visualization.Render(contacts);
        }
    }

    #endregion
}
