using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using UnityEngine.UI;

public enum ItemType
{
    Armor,
    Potion,
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
    public int Health = 0;
    public int Armor = 0;
    public int Stamina = 0;
    public int Strength = 0;
    public int Dexterity = 0;
    public int Intellect = 0;
}

public class Item
{
    public int Id;
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

    public Item(int id, ItemRarity rarity, ItemType type)
    {
        Id = id;
        Name = GenerateName(type);
        Rarity = ItemRarity.Common;
        Type = type;
        Stats = GenerateStats(rarity, type);
        IconItem = GenerateIconItem(type);
    }

    private string GenerateName(ItemType? type)
    {
        return type == ItemType.Potion ? "Здоровье" : "Броня";
    }

    private Stats GenerateStats(ItemRarity rarity, ItemType type)
    {
        Stats stats = new Stats();
        if (type == ItemType.Potion)
        {
            stats.Health = 50;
        }
        else
        {
            stats.Armor = 30;
        }
        

        return stats;
    }

    private GameObject GenerateIconItem(ItemType? type)
    {
        GameObject icon = GameObject.Instantiate(type == ItemType.Potion ? ItemsManager.Instance.healthIcon : ItemsManager.Instance.armorIcon);
        icon.GetComponent<Image>().color = RARITY_TO_COLOR_MAP[Rarity];

        return icon;
    }
}
