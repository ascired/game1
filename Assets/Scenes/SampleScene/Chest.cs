using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using UnityEngine.UI;
using UniRx.Triggers;
using UniRx;

public class Chest : MonoBehaviour {

    public GameObject ChestPanel;
    private Animator anim;
    private Player Player;
    private Text Title;
    private Transform Container;
    private Item[] ItemList;

    void Start()
    {
        anim = GetComponent<Animator>();

        Player = SceneManager.Instance.player;
        Title = ChestPanel.transform.GetComponentInChildren<Text>();
        Container = ChestPanel.transform.Find("Container");

        int count = Random.Range(1, 4);
        ItemList = new Item[count];

        for (int i = 0; i < count; i++)
        {
            ItemList[i] = ItemsManager.Instance.GenerateItem();
        }

        this.OnTriggerExitAsObservable()
            .Subscribe(_ => Close());
    }

    public void Open()
    {
        if (anim)
        {
            anim.SetTrigger("Open");
        }
        ClearChestPanel();
        Title.text = name;

        foreach (Item item in ItemList)
        {
            Instantiate(item.IconItem, Container);
        }

        ChestPanel.SetActive(true);
    }

    public void Close()
    {
        if (anim)
        {
            anim.SetTrigger("Close");
        }
        ClearChestPanel();
        ChestPanel.SetActive(false);
    }

    private void ClearChestPanel()
    {
        foreach (Transform child in Container.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
    }
}
