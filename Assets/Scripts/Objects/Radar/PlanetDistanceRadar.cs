using UnityEngine;

[DisallowMultipleComponent]
public sealed class PlanetDistanceRadar : NearestPlanetRadar
{
    [Header("Settings Planet Distance Radar")]
    public string distanceFormat = "0";
    public string distanceUnit = "m";

    public override string GetDisplayText()
    {
        if (!isRadarEnabled) return string.Empty;
        if (currentPlanet == null) return $"--- {distanceUnit}";

        return $"{currentDistance.ToString(distanceFormat)} {distanceUnit}";
    }
}
