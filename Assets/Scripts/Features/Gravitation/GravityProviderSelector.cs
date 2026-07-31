using UnityEngine;

public sealed class GravityProviderSelector : MonoBehaviour
{
    public GravityField targetField;

    public GravityProvider directionalGravity;

    public GravityProvider planetaryGravity;

    public void UseDirectionalGravity()
    {
        SetProvider(directionalGravity);
    }

    public void UsePlanetaryGravity()
    {
        SetProvider(planetaryGravity);
    }

    public void SetProvider(GravityProvider provider)
    {
        if (targetField == null)
        {
            Debug.LogError(
                "GravityProviderSelector: Target Field is not assigned.",
                this);

            return;
        }

        if (provider == null)
        {
            Debug.LogError(
                "GravityProviderSelector: Gravity Provider is not assigned.",
                this);

            return;
        }

        targetField.SetProvider(provider);
    }
}