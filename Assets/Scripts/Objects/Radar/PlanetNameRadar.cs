using UnityEngine;

[DisallowMultipleComponent]
public sealed class PlanetNameRadar : NearestPlanetRadar
{
    public override string GetDisplayText()
    {
        if (!isRadarEnabled) return string.Empty;
        return currentPlanet == null ? "NO PLANET" : "PLANET: " + currentPlanet.DisplayName;
    }
}
