using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public sealed class RadarPlanet : MonoBehaviour
{
    #region Static

    static readonly List<RadarPlanet> activePlanets = new();

    public static IReadOnlyList<RadarPlanet> ActivePlanets => activePlanets;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void ResetRegistry() => activePlanets.Clear();

    #endregion

    #region Inspector

    [Header("Dependencies")]
    public GravityField gravityField;
    public Transform center;

    [Header("Settings Radar Planet")]
    public string displayName;

    [Header("Debug")]
    public bool isRegistered;

    #endregion

    #region Properties

    public Vector3 CenterPosition => center != null ? center.position : transform.position;

    public string DisplayName =>
        string.IsNullOrWhiteSpace(displayName)
            ? gameObject.name
            : displayName;

    #endregion

    #region Unity Messages

    void OnEnable()
    {
        if (!activePlanets.Contains(this))
            activePlanets.Add(this);

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

    #endregion
}
