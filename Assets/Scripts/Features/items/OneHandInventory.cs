using System;
using System.Collections.Generic;
using Lucy.Features.Items;
using UnityEngine;

public class OneHandInventory : MonoBehaviour, IInventory
{
    #region Settings

    [Header("Settings")]
    public int capacity = 1;
    public Transform handSocket;
    public Transform dropPoint;
    public Transform worldDropSpace;

    [Header("Debug")]
    public bool debugEnabled = false;

    [Header("Runtime")]
    public List<ItemInstance> items = new();
    public GameObject equippedItemObject;

    public IReadOnlyList<ItemInstance> Items => items;

    public event Action<ItemInstance> OnItemAdded;
    public event Action<ItemInstance> OnItemRemoved;
    public event Action<ItemInstance> OnItemUsed;

    #endregion

    #region Inventory API

    public bool AddItem(ItemInstance item)
    {
        if (debugEnabled) Debug.Log("Adding item: " + item.Defination.id);
        if (items.Count >= capacity) DropCurrentItem();

        items.Clear();
        items.Add(item);

        EquipItemInHand(item);

        OnItemAdded?.Invoke(item);

        return true;
    }

    public bool RemoveItem(ItemInstance item)
    {
        if (items.Contains(item))
        {
            items.Remove(item);
            OnItemRemoved?.Invoke(item);
            return true;
        }

        return false;
    }

    public bool UseItem(string itemId, object user)
    {
        var item = items.Find(i => i.Defination.id == itemId);
        if (item == null) return false;

        item.Defination.Use(this);

        OnItemUsed?.Invoke(item);

        return true;
    }

    public IEnumerable<ItemInstance> GetAllItems()
    {
        return items;
    }

    public void UseCurrentItem()
    {
        if (items.Count != 0) UseItem(items[0].Defination.id, this);
    }

    public void DropCurrentItem()
    {
        if (items.Count != 0) DropItem(items[0], dropPoint.position);
    }

    public void ClearInventory()
    {
        items.Clear();
        UnequipItemFromHand();
    }
    #endregion

    #region Internal logic

    public void DropItem(ItemInstance item, Vector3 dropPosition)
    {
        RemoveItem(item);

        UnequipItemFromHand();

        var prefab = item.Defination.prefab;

        if (prefab)
        {
            var worldObj = Instantiate(prefab, dropPosition, Quaternion.identity);
            if (worldDropSpace) worldObj.transform.SetParent(worldDropSpace);
            var pickable = worldObj.GetComponent<PickableItem>();


            if (pickable)
            {
                pickable.Initialize(item);
            }
        }
    }

    void EquipItemInHand(ItemInstance item)
    {
        // Удаляем старый предмет из руки
        UnequipItemFromHand();

        if (item?.Defination?.model == null)
            return;

        // Создаём копию предмета и крепим к руке
        equippedItemObject = Instantiate(item.Defination.model, handSocket);

        equippedItemObject.transform.localPosition = Vector3.zero;
        equippedItemObject.transform.localRotation = Quaternion.identity;

        // Можно отключить PickableItem, если он есть на модели
        var pickable = equippedItemObject.GetComponent<PickableItem>();

        if (pickable)
        {
            Destroy(pickable);
        }
    }

    void UnequipItemFromHand()
    {
        if (equippedItemObject)
        {
            Destroy(equippedItemObject);
            equippedItemObject = null;
        }
    }

    #endregion
}