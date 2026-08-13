using TMPro;
using UnityEngine;

[DisallowMultipleComponent]
public sealed class RadarTextUI : SpaceShipRadarUI
{
    [Header("Dependencies")]
    public TMP_Text text;

    void OnValidate()
    {
        text = text != null ? text : GetComponentInChildren<TMP_Text>(true);
    }

    public override void Bind(SpaceShipRadar target)
    {
        base.Bind(target);

        if (radar == null) return;

        radar.OnRadarDataChanged += Refresh;
        radar.OnRadarEnabledChanged += HandleRadarEnabledChanged;
        Refresh();
    }

    public override void Unbind()
    {
        if (radar != null)
        {
            radar.OnRadarDataChanged -= Refresh;
            radar.OnRadarEnabledChanged -= HandleRadarEnabledChanged;
        }

        base.Unbind();
    }

    void HandleRadarEnabledChanged(bool enabled) => Refresh();

    public void Refresh()
    {
        if (text == null) return;
        text.text = radar != null ? radar.GetDisplayText() : string.Empty;
    }
}
