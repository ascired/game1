using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : ScriptableObject
{

    #region Fields

    private List<InventoryItem> _items = new List<InventoryItem>();

    #endregion

    #region Methods

    public void AddItem(InventoryItem item)
    {
        _items.Add(item);
    }

    public void RemoveItem(InventoryItem item)
    {
        _items.Remove(item);
    }

    //so on so forth

    #endregion

}

public class InventoryItem : ScriptableObject
{

    //fields and stuff

}