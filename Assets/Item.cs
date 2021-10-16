using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using UnityEngine.UI;

public enum ItemType
{
    Trash,
    Weapon,
    Armor,
    Accessory,
    Consumable,
    Usable,
}

public enum ItemRarity
{
    Poor,
    Common,
    Uncommon,
    Rare,
    Epic,
    Legendary,
}

public class Stats
{
    public int Stamina = 0;
    public int Strength = 0;
    public int Dexterity = 0;
    public int Intellect = 0;
}

public class Item
{
    public string Name;
    public ItemRarity Rarity;
    public ItemType Type;
    public Stats Stats;
    public bool Craftable;
    public bool Usable;
    public GameObject IconItem;


    public static Dictionary<ItemRarity, Color32> RARITY_TO_COLOR_MAP
        = new Dictionary<ItemRarity, Color32>()
        {
            { ItemRarity.Poor, new Color32(131, 131, 131, 255) },
            { ItemRarity.Common, new Color32(241, 241, 241, 255) },
            { ItemRarity.Uncommon, new Color32(69, 161, 81, 255) },
            { ItemRarity.Rare, new Color32(49, 95, 174, 255) },
            { ItemRarity.Epic, new Color32(142, 48, 173, 255) },
            { ItemRarity.Legendary, new Color32(215, 117, 0, 255) },
        };

    public Item(
        string name,
        ItemRarity rarity,
        ItemType type,
        Stats stats,
        bool craftable = true,
        bool usable = false
    )
    {
        Name = name;
        Rarity = rarity;
        Type = type;
        Stats = stats;
        Craftable = craftable;
        Usable = usable;

        IconItem = GenerateIconItem();
    }

    public Item(ItemRarity rarity, ItemType type)
    {
        Name = GenerateName(type);
        Rarity = rarity;
        Type = type;
        Stats = GenerateStats(rarity, type);
        IconItem = GenerateIconItem();
    }

    private string GenerateName(ItemType? type)
    {
        return "___item___";
    }

    private Stats GenerateStats(ItemRarity rarity, ItemType type)
    {
        Stats stats = new Stats();
        stats.Stamina = 1;

        return stats;
    }

    private GameObject GenerateIconItem()
    {
        GameObject icon = GameObject.Instantiate(ItemsManager.Instance.itemIcon);
        icon.GetComponent<Image>().color = RARITY_TO_COLOR_MAP[Rarity];

        return icon;
    }
}
