using UnityEngine;

[DisallowMultipleComponent]
public sealed class SpaceShipRadarUIHost : MonoBehaviour
{
    [Header("Dependencies")]
    public SpaceShipRadarSlot radarSlot;
    public Transform content;

    [Header("Settings Space Ship Radar UI Host")]
    public bool hideWhenRadarDisabled = true;

    [Header("Debug")]
    public SpaceShipRadarUI currentUI;

    void Awake()
    {
        content = content != null ? content : transform;
    }

    void OnEnable()
    {
        if (radarSlot == null) return;

        radarSlot.OnRadarChanged += HandleRadarChanged;
        radarSlot.OnRadarEnabledChanged += HandleRadarEnabledChanged;

        Rebuild(radarSlot.currentRadar);
        ApplyVisibility(radarSlot.radarEnabled);
    }

    void OnDisable()
    {
        if (radarSlot != null)
        {
            radarSlot.OnRadarChanged -= HandleRadarChanged;
            radarSlot.OnRadarEnabledChanged -= HandleRadarEnabledChanged;
        }

        DestroyCurrentUI();
    }

    void OnValidate()
    {
        content = content != null ? content : transform;
    }

    void HandleRadarChanged(SpaceShipRadar radar)
    {
        Rebuild(radar);
        if (radarSlot != null) ApplyVisibility(radarSlot.radarEnabled);
    }

    void HandleRadarEnabledChanged(bool enabled) => ApplyVisibility(enabled);

    public void Rebuild(SpaceShipRadar radar)
    {
        DestroyCurrentUI();

        if (radar == null || radar.uiPrefab == null || content == null) return;

        currentUI = Instantiate(radar.uiPrefab, content);
        currentUI.Bind(radar);
    }

    void ApplyVisibility(bool radarEnabled)
    {
        if (currentUI == null) return;
        currentUI.gameObject.SetActive(!hideWhenRadarDisabled || radarEnabled);
    }

    void DestroyCurrentUI()
    {
        if (currentUI == null) return;

        currentUI.Unbind();
        Destroy(currentUI.gameObject);
        currentUI = null;
    }
}
