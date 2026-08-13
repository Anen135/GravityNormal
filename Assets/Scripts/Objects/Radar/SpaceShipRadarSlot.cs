using System;
using UnityEngine;

[DisallowMultipleComponent]
public sealed class SpaceShipRadarSlot : MonoBehaviour
{
    [Header("Dependencies")]
    public GravityBody gravityBody;
    public Transform origin;
    public SpaceShipRadar currentRadar;

    [Header("Debug")]
    public bool radarEnabled;

    public Action<SpaceShipRadar> OnRadarChanged;
    public Action<bool> OnRadarEnabledChanged;

    void Awake()
    {
        FindDependencies();
        if (currentRadar != null) EquipRadar(currentRadar);
    }

    void OnValidate() => FindDependencies();

    void OnDisable()
    {
        if (currentRadar != null) currentRadar.SetRadarEnabled(false);
        radarEnabled = false;
    }

    void FindDependencies()
    {
        gravityBody = gravityBody != null ? gravityBody : GetComponentInParent<GravityBody>();
        origin = origin != null ? origin : gravityBody != null ? gravityBody.transform : transform;
    }

    public void EquipRadar(SpaceShipRadar radar)
    {
        if (currentRadar == radar && radar != null && radar.isEquipped) return;

        if (currentRadar != null) currentRadar.Unequip();

        currentRadar = radar;
        radarEnabled = false;

        if (currentRadar != null) currentRadar.Equip(this);

        OnRadarChanged?.Invoke(currentRadar);
        OnRadarEnabledChanged?.Invoke(false);
    }

    public SpaceShipRadar UnequipRadar()
    {
        SpaceShipRadar previous = currentRadar;

        if (previous != null) previous.Unequip();

        currentRadar = null;
        radarEnabled = false;

        OnRadarChanged?.Invoke(null);
        OnRadarEnabledChanged?.Invoke(false);

        return previous;
    }

    public void ToggleRadar()
    {
        if (currentRadar == null) return;
        SetRadarEnabled(!radarEnabled);
    }

    public void SetRadarEnabled(bool value)
    {
        if (currentRadar == null)
        {
            radarEnabled = false;
            return;
        }

        radarEnabled = value;
        currentRadar.SetRadarEnabled(value);

        OnRadarEnabledChanged?.Invoke(radarEnabled);
    }

    public void EnableRadar() => SetRadarEnabled(true);
    public void DisableRadar() => SetRadarEnabled(false);
}
