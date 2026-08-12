using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public sealed class DisplayRadarVisualizations : MonoBehaviour
{
    #region Inspector

    [Header("Dependencies")]
    public PlanetRadar radar;
    public Transform content;
    public RadarVisualizationToggle togglePrefab;

    [Header("Settings Display Radar Visualizations")]
    public bool rebuildOnEnable = true;

    [Header("Debug")]
    public List<RadarVisualization> visualizations = new();
    public List<RadarVisualizationToggle> toggles = new();

    #endregion

    #region Unity Messages

    void Awake()
    {
        FindDependencies();

        if (!rebuildOnEnable)
            Rebuild();
    }

    void OnEnable()
    {
        if (rebuildOnEnable)
            Rebuild();
    }

    void OnValidate()
    {
        FindDependencies();
    }

    #endregion

    #region Initialization

    void FindDependencies()
    {
        radar = radar != null
            ? radar
            : FindAnyObjectByType<PlanetRadar>();

        content = content != null
            ? content
            : transform;

        togglePrefab = togglePrefab != null
            ? togglePrefab
            : GetComponentInChildren<RadarVisualizationToggle>(true);
    }

    #endregion

    #region UI

    public void Rebuild()
    {
        Clear();

        if (radar == null)
            return;

        RadarVisualization[] found =
            radar.GetComponents<RadarVisualization>();

        for (int i = 0; i < found.Length; i++)
        {
            RadarVisualization visualization = found[i];

            if (visualization == null)
                continue;

            visualizations.Add(visualization);
            CreateToggle(visualization);
        }
    }

    void CreateToggle(RadarVisualization visualization)
    {
        if (togglePrefab == null)
            return;

        RadarVisualizationToggle instance =
            Instantiate(
                togglePrefab,
                content
            );

        instance.gameObject.SetActive(true);
        instance.Bind(visualization);

        toggles.Add(instance);
    }

    public void Clear()
    {
        visualizations.Clear();

        for (int i = toggles.Count - 1; i >= 0; i--)
        {
            RadarVisualizationToggle toggle = toggles[i];

            if (toggle != null)
                Destroy(toggle.gameObject);
        }

        toggles.Clear();
    }

    #endregion

    #region Radar

    public void SetRadar(PlanetRadar target)
    {
        if (radar == target)
            return;

        radar = target;
        Rebuild();
    }

    public void Refresh()
    {
        for (int i = 0; i < toggles.Count; i++)
        {
            RadarVisualizationToggle toggle = toggles[i];

            if (
                toggle == null ||
                toggle.visualization == null
            )
                continue;

            toggle.Bind(toggle.visualization);
        }
    }

    #endregion
}
