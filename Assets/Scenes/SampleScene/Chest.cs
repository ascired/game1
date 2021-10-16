using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

public class Chest : MonoBehaviour {

    public GameObject ChestPanel;
    private Transform Container;
    private Item[] ItemList;

    void Start()
    {
        Container = ChestPanel.transform.Find("Panel");

        int count = Random.Range(1, 6);
        ItemList = new Item[count];

        for (int i = 0; i < count; i++)
        {
            ItemList[i] = ItemsManager.Instance.GenerateItem();
        }
    }

    public void Open()
    {
        ClearContainer();
        foreach (Item item in ItemList)
        {
            Instantiate(item.IconItem, Container);
        }

        ChestPanel.SetActive(true);
    }

    private void ClearContainer()
    {
        foreach (Transform child in Container.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
    }
}
