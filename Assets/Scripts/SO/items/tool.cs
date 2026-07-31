using Lucy.Core;
using UnityEngine;

[CreateAssetMenu(menuName = "Lucy/Items/Tool", fileName = "New Tool")]
public class Tool : ItemDef
{
    public string targetTag = "Interactable";
    public override void Use(object user)
    {
        if (user is MonoBehaviour mb)
        {
            if (mb.gameObject.GetComponent<Interaction>() is not null and Interaction interaction && interaction.CurrentTarget != null && interaction.CurrentTarget.CompareTag(targetTag))
            {
                interaction.Interact();
            } 
        }
    }
}