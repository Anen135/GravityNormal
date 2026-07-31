using Lucy.Core;
using UnityEngine;

public class ProtectedInteractable : Interactable
{
    public string key;
    public void Interact(Interaction interactor, string key)
    {
        if (this.key == key) base.Interact(interactor);
    }
    public override void Interact(Interaction interactor) { }
}