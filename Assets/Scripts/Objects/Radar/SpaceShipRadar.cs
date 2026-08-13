using System;
using UnityEngine;

public abstract class SpaceShipRadar : MonoBehaviour
{
    [Header("Dependencies")]
    public SpaceShipRadarUI uiPrefab;

    [Header("Settings Space Ship Radar")]
    public string displayName = "Radar";

    [Header("Debug")]
    public SpaceShipRadarSlot slot;
    public GravityBody gravityBody;
    public Transform origin;
    public bool isRadarEnabled;
    public bool isEquipped;

    public Action<bool> OnRadarEnabledChanged;
    public Action OnRadarDataChanged;

    public string DisplayName => string.IsNullOrWhiteSpace(displayName) ? GetType().Name : displayName;

    public Vector3 OriginPosition
    {
        get
        {
            if (origin != null) return origin.position;
            if (gravityBody != null && gravityBody.rb != null) return gravityBody.rb.worldCenterOfMass;
            return transform.position;
        }
    }

    protected virtual void Update()
    {
        if (!isEquipped || !isRadarEnabled) return;
        UpdateRadar();
    }

    protected virtual void OnDisable()
    {
        if (isRadarEnabled) SetRadarEnabled(false);
    }

    public void Equip(SpaceShipRadarSlot owner)
    {
        if (owner == null) return;
        if (slot == owner && isEquipped) return;

        if (isEquipped) Unequip();

        slot = owner;
        gravityBody = owner.gravityBody;
        origin = owner.origin;
        isEquipped = true;

        OnEquipped();
        NotifyDataChanged();
    }

    public void Unequip()
    {
        if (!isEquipped) return;

        SetRadarEnabled(false);
        OnUnequipped();

        slot = null;
        gravityBody = null;
        origin = null;
        isEquipped = false;

        NotifyDataChanged();
    }

    public void Toggle() => SetRadarEnabled(!isRadarEnabled);

    public void SetRadarEnabled(bool value)
    {
        if (!isEquipped && value) return;
        if (isRadarEnabled == value) return;

        isRadarEnabled = value;

        if (value) OnRadarEnabled();
        else OnRadarDisabled();

        OnRadarEnabledChanged?.Invoke(value);
        NotifyDataChanged();
    }

    protected virtual void OnEquipped() { }
    protected virtual void OnUnequipped() { }
    protected virtual void OnRadarEnabled() { }
    protected virtual void OnRadarDisabled() { }

    protected abstract void UpdateRadar();

    protected void NotifyDataChanged() => OnRadarDataChanged?.Invoke();

    public virtual string GetDisplayText() => DisplayName;
}
