using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class ItemsManager : MonoBehaviour
{
    public GameObject itemIcon;
    public static ItemsManager Instance { get; private set; }
    private Dictionary<string, Item> chestList;

    private static float[] RarityProbability = { 0.25f, 0.6f, 0.75f, 0.97f, 0.99f, 1f };

    void Awake()
    {
        if (Instance == null) { Instance = this; }
        else { Destroy(gameObject); }
    }

    public Item GenerateItem()
    {
        float rand = Random.Range(0f, 1f);
        int i = System.Array.FindIndex(RarityProbability, ((float prob) => rand < prob));
        ItemRarity rarity = i >= 0 ? (ItemRarity)i : ItemRarity.Poor;
        ItemType type = ItemType.Weapon;

        return new Item(rarity, type);

    }
}
