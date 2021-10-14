using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

public class Chest : MonoBehaviour {

    public GameObject itemIcon;
    public GameObject chestPanel;

    void Start()
    {
        int count = Random.Range(1, 6);

        Transform panel = chestPanel.transform.Find("Panel");

        for (int i = 0; i <= count; i++)
        {
            Instantiate(itemIcon, panel);
        }
    }

    public void Open()
    {
        chestPanel.SetActive(true);
    }

}
