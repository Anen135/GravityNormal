using UnityEngine;
using System;

[Serializable]
public class ItemInstance
{
    [Header("Data")]
    public ItemDef Defination;
    public int Quantity = 1;
    [Header("Runtime")]
    public string uniqueId;
    public event Action<ItemInstance, object> OnUsed;
    public string UniqueId => uniqueId;

    public ItemInstance (ItemDef data, int quantity = 1)
    {
        Defination = data;
        Quantity = quantity;
        uniqueId = Guid.NewGuid().ToString();
    }

    public ItemInstance Clone() => new(Defination, Quantity) { uniqueId = uniqueId };
}
