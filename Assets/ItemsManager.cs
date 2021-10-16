using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

public class ItemsManager : MonoBehaviour
{
    public GameObject itemIcon;
    public static ItemsManager Instance { get; private set; }
    private Dictionary<string, Item> chestList;

    void Awake()
    {
        if (Instance == null) { Instance = this; }
        else { Destroy(gameObject); }

    }

    public Item GenerateItem()
    {
        int t = System.Enum.GetValues(typeof(ItemRarity)).Length;
        int c = Random.Range(0, t);
        ItemRarity rarity = (ItemRarity)c;
        ItemType type = ItemType.Weapon;

        return new Item(rarity, type);

    }
}
