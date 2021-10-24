using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : ScriptableObject
{
    private List<InventoryItem> _items = new List<InventoryItem>();

    public void AddItem(InventoryItem item)
    {
        _items.Add(item);
    }

    public void RemoveItem(InventoryItem item)
    {
        _items.Remove(item);
    }
}

public class InventoryItem : ScriptableObject
{

}