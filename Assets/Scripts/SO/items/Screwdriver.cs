using Lucy.Core;
using UnityEngine;

[CreateAssetMenu(menuName = "Lucy/Items/Screwdriver", fileName = "Screwdriver")]
public class Screwdriver : ItemDef
{
    public override void Use(object user)
    {
        if (user is not MonoBehaviour mb) return;

        var interaction = mb.GetComponent<Interaction>();

        if (interaction == null)
        {
            Debug.Log("No Interaction component");
            return;
        }

        if (interaction.CurrentTarget == null)
        {
            Debug.Log("No CurrentTarget");
            return;
        }

        if (interaction.CurrentTarget.TryGetComponent<Screw>(out var screw))
        {
            Debug.Log("Has Screw " + interaction.CurrentTarget.name);
            screw.DoScrew();
        }
        else
        {
            Debug.Log("Target has no Screw component");
        }
    }
}