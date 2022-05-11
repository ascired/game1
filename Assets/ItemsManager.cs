using System.Collections.Generic;
using UnityEngine;
using UniRx.Triggers;
using UniRx;
using Random = UnityEngine.Random;

public class ItemsManager : MonoBehaviour
{
    public GameObject healthIcon;
    public GameObject armorIcon;
    public static ItemsManager Instance { get; private set; }
    public Dictionary<int, Item> itemList = new Dictionary<int, Item>();
    public Dictionary<int, Item> playerItems = new Dictionary<int, Item>();
    private Dictionary<int, List<int>> chests = new Dictionary<int, List<int>>();

    private int chestId = 0;
    private int itemId = 0;

    private static float[] RarityProbability = { 0.25f, 0.6f, 0.75f, 0.97f, 0.99f, 1f };

    void Awake()
    {
        if (Instance == null) { Instance = this; }
        else { Destroy(gameObject); }
    }

    void Start()
    {
        this.UpdateAsObservable()
            .Select(_ => playerItems.Count)
            .DistinctUntilChanged()
            .Do(_ => renderInventoryPanel(playerItems))
            .Subscribe()
            .AddTo(this);

    }

    private void renderInventoryPanel(Dictionary<int, Item> items)
    {
        Debug.Log(items);
    }

    public int createNewChest(int count)
    {
        chestId++;
        chests.Add(chestId, new List<int>());

        for (int i = 0; i < count; i++)
        {
            Item newItem = ItemsManager.Instance.GenerateItem(chestId);

            EvtTrigger trigger = newItem.IconItem.GetComponent<EvtTrigger>();
            trigger.id = newItem.Id;

            chests[chestId].Add(newItem.Id);
        }

        return chestId;
    }

    public List<int> getChest(int id)
    {
        return chests[id];
    }

    public List<int> removeFromChest(int chestId, int itemId)
    {
        chests[chestId].Remove(itemId);
        return chests[chestId];
    }

    public Item GenerateItem(int id)
    {
        itemId++;
        float rand = Random.Range(0f, 1f);
        int i = System.Array.FindIndex(RarityProbability, ((float prob) => rand < prob));
        ItemRarity rarity = i >= 0 ? (ItemRarity)i : ItemRarity.Poor;
        ItemType type = id % 2 == 0 ? ItemType.Potion : ItemType.Armor;
        Item newItem = new Item(itemId, rarity, type);
        itemList.Add(itemId, newItem);

        return newItem;

    }

    public void addItemToPlayer(int id)
    {
        playerItems.Add(id, itemList[id]);
        itemList.Remove(id);
    }
}
