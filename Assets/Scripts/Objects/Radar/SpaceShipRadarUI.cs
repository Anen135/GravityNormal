using UnityEngine;

public abstract class SpaceShipRadarUI : MonoBehaviour
{
    [Header("Debug")]
    public SpaceShipRadar radar;

    public virtual void Bind(SpaceShipRadar target)
    {
        Unbind();
        radar = target;
    }

    public virtual void Unbind()
    {
        radar = null;
    }
}
