using System.Collections.Generic;
using UnityEngine;

public abstract class RadarVisualization : MonoBehaviour
{
    #region Inspector

    [Header("Dependencies")]
    public PlanetRadar radar;

    [Header("Settings Radar Visualization")]
    public string displayName;
    public bool visualizationEnabled = true;

    [Header("Debug")]
    public bool isVisible;

    #endregion

    #region Properties

    public string DisplayName =>
        string.IsNullOrWhiteSpace(displayName)
            ? GetType().Name.Replace("Visualization", string.Empty)
            : displayName;

    #endregion

    #region Unity Messages

    protected virtual void Awake()
    {
        radar = radar != null ? radar : GetComponent<PlanetRadar>();
    }

    protected virtual void OnDisable() => Hide();

    #endregion

    #region Visualization

    public void Render(IReadOnlyList<RadarContact> contacts)
    {
        if (!visualizationEnabled)
        {
            Hide();
            return;
        }

        RenderVisualization(contacts);
    }

    public void Hide() => SetVisible(false);

    protected abstract void RenderVisualization(IReadOnlyList<RadarContact> contacts);

    protected virtual void SetVisible(bool visible)
    {
        isVisible = visible;
    }

    #endregion
}
