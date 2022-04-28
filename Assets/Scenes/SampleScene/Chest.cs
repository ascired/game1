using UnityEngine;
using UnityEngine.UI;
using UniRx.Triggers;
using UniRx;

public class Chest : MonoBehaviour {

    public GameObject ChestPanel;
    private Animator anim;
    private Player Player;
    private Text Title;
    private Transform Container;
    private GameObject Glow;
    private Item[] ItemList;

    void Start()
    {
        anim = this.GetComponent<Animator>();

        Player = MainManager.Instance.player;
        Title = ChestPanel.transform.GetComponentInChildren<Text>();
        Container = ChestPanel.transform.Find("Container");
        Glow = gameObject.transform.Find("chest_glow")?.gameObject;

        int count = Random.Range(1, 4);
        ItemList = new Item[count];

        for (int i = 0; i < count; i++)
        {
            ItemList[i] = ItemsManager.Instance.GenerateItem();
        }

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

        foreach (Item item in ItemList)
        {
            Instantiate(item.IconItem, Container);
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
