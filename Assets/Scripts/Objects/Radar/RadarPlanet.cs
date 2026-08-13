using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public sealed class RadarPlanet : MonoBehaviour
{
    static readonly List<RadarPlanet> activePlanets = new();

    public static IReadOnlyList<RadarPlanet> ActivePlanets => activePlanets;

    [Header("Dependencies")]
    public GravityField gravityField;
    public Transform center;

    [Header("Settings Radar Planet")]
    public string displayName;

    [Header("Debug")]
    public bool isRegistered;

    public Vector3 CenterPosition => center != null ? center.position : transform.position;
    public string DisplayName => string.IsNullOrWhiteSpace(displayName) ? gameObject.name : displayName;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void ResetRegistry() => activePlanets.Clear();

    public static RadarPlanet FindByGravityField(GravityField field)
    {
        if (field == null) return null;

        for (int i = 0; i < activePlanets.Count; i++)
        {
            RadarPlanet planet = activePlanets[i];
            if (planet != null && planet.gravityField == field) return planet;
        }

        return null;
    }

    void OnEnable()
    {
        if (!activePlanets.Contains(this)) activePlanets.Add(this);
        isRegistered = true;
    }

    void OnDisable()
    {
        activePlanets.Remove(this);
        isRegistered = false;
    }

    void OnValidate()
    {
        gravityField = gravityField != null ? gravityField : GetComponent<GravityField>();
        center = center != null ? center : transform;
    }
}
