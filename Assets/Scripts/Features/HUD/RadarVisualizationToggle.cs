using TMPro;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public sealed class RadarVisualizationToggle : MonoBehaviour
{
    #region Inspector

    [Header("Dependencies")]
    public TMP_Text label;
    public Toggle toggle;

    [Header("Debug")]
    public RadarVisualization visualization;

    #endregion

    #region Unity Messages

    void Awake()
    {
        label = label != null
            ? label
            : GetComponentInChildren<TMP_Text>(true);

        toggle = toggle != null
            ? toggle
            : GetComponentInChildren<Toggle>(true);
    }

    void OnDisable()
    {
        if (toggle != null)
            toggle.onValueChanged.RemoveListener(HandleValueChanged);
    }

    #endregion

    #region Initialization

    public void Bind(RadarVisualization target)
    {
        visualization = target;

        if (visualization == null)
            return;

        if (label != null)
            label.text = visualization.DisplayName;

        if (toggle != null)
        {
            toggle.onValueChanged.RemoveListener(HandleValueChanged);

            toggle.SetIsOnWithoutNotify(
                visualization.visualizationEnabled
            );

            toggle.onValueChanged.AddListener(HandleValueChanged);
        }
    }

    #endregion

    #region UI

    void HandleValueChanged(bool value)
    {
        if (visualization == null)
            return;

        visualization.visualizationEnabled = value;

        if (!value)
            visualization.Hide();
    }

    #endregion
}
