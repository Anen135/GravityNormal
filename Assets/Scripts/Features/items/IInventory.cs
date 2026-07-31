using System;
using System.Collections.Generic;

public interface IInventory
{
    event Action<ItemInstance> OnItemAdded;
    event Action<ItemInstance> OnItemRemoved;
    event Action<ItemInstance> OnItemUsed;

    bool AddItem(ItemInstance item);
    bool RemoveItem(ItemInstance item);
    IEnumerable<ItemInstance> GetAllItems();
    bool UseItem(string itemId, object user);
}
