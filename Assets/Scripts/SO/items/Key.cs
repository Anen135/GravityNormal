using Lucy.Core;
using UnityEngine;

[CreateAssetMenu(menuName = "Lucy/Items/Key", fileName = "New Key")]
public class Key : ItemDef
{
    public string value;
    public override void Use(object user)
    {
        if (user is not MonoBehaviour mb) return;
        var interaction = mb.GetComponent<Interaction>();
        if (interaction == null || interaction.CurrentTarget == null) return; 
        if (interaction.CurrentTarget.TryGetComponent<ProtectedInteractable>(out var screw)) screw.Interact(interaction, value);
    }
}