using UnityEngine;
using UnityEngine.UI;
using UniRx.Triggers;
using UniRx;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class Chest : MonoBehaviour
{
    public GameObject ChestPanel;
    private Animator anim;
    private Player Player;
    private Text Title;
    private Transform Container;
    private GameObject Glow;
    private int Id;

    void Start()
    {
        anim = this.GetComponent<Animator>();

        Player = MainManager.Instance.player;
        Title = ChestPanel.transform.GetComponentInChildren<Text>();
        Container = ChestPanel.transform.Find("Container");
        Glow = gameObject.transform.Find("chest_glow")?.gameObject;

        int count = Random.Range(1, 4);
        Id = ItemsManager.Instance.createNewChest(1);

        this.OnTriggerExitAsObservable()
            .Subscribe(_ => Close());

        this.UpdateAsObservable()
            .Select(_ => ChestPanel.activeSelf)
            .DistinctUntilChanged()
            .Where(isActive => !isActive)
            .Subscribe(_ => ToggleLidAnimation(false));
    }

    private void OnMouseEnter()
    {
        if (Glow)
        {
            Glow.SetActive(true);
        }
    }

    private void OnMouseExit()
    {
        if (Glow)
        {
            Glow.SetActive(false);
        }
    }

    private void ToggleLidAnimation(bool isOpen)
    {
        if (anim)
        {
            anim.SetBool("IsOpen", isOpen);
        }
    }

    public void Open()
    {
        ClearChestPanel();
        Title.text = name;
        List<int> ItemIds = ItemsManager.Instance.getChest(Id);
        Dictionary<int, Item> itemList = ItemsManager.Instance.itemList;

        foreach (int itemId in ItemIds)
        {
            if (itemList.ContainsKey(itemId))
            {
                Instantiate(itemList[itemId].IconItem, Container);
            }
        }

        ChestPanel.SetActive(true);
        ToggleLidAnimation(true);
    }

    public void Close()
    {
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
