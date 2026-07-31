using UnityEngine;
using System;
using Lucy.Core;

namespace Lucy.Features.Items
{
    [RequireComponent(typeof(Collider))]
    public class PickableItem : Interactable
    {
        public ItemInstance itemInstance;
        public event Action<Interactable, object> OnInteracted;

        public override void Interact(Interaction inter)
        {
            var inventory = inter.GetComponent<IInventory>();
            if (inventory != null && inventory.AddItem(itemInstance))
            {
                OnInteracted?.Invoke(this, inter);
                Destroy(gameObject);
            } else
            {
                if (debugEnabled) Debug.Log("Cannot add item to inventory");
            }
        }
        public ItemInstance Item => itemInstance;
        public void Initialize(ItemInstance instance) => itemInstance = instance.Clone();
    }
}
