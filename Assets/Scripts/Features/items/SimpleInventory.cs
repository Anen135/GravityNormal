// Game layer
// Variant component
using System;
using System.Collections.Generic;
using UnityEngine;

public class SimpleInventory : MonoBehaviour, IInventory
{
    [Header("Inventory Settings")]
    public List<ItemInstance> items = new();

    public event Action<ItemInstance> OnItemAdded;
    public event Action<ItemInstance> OnItemRemoved;
    public event Action<ItemInstance> OnItemUsed;

    public bool AddItem(ItemInstance item)
    {
        if (item == null) return false;
        items.Add(item);
        OnItemAdded?.Invoke(item);
        Debug.Log($"[{gameObject.name}] Added item: {item.Defination.id}");
        return true;
    }

    public bool RemoveItem(ItemInstance item)
    {
        if (item == null || !items.Contains(item)) return false;

        items.Remove(item);
        OnItemRemoved?.Invoke(item);
        Debug.Log($"[{gameObject.name}] Removed item: {item.Defination.id}");
        return true;
    }

    public bool Contains(ItemInstance item)
    {
        return item != null && items.Contains(item);
    }

    public IEnumerable<ItemInstance> GetAllItems()
    {
        return items;
    }

    public bool UseItem(string itemId, object user)
    {
        var item = items.Find(i => i.Defination.id == itemId);
        if (item == null) return false;

        item.Defination.Use(user);
        OnItemUsed?.Invoke(item);
        Debug.Log($"[{gameObject.name}] Used item: {item.Defination.id}");
        return true;
    }
}
