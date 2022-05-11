using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArmorBar : MonoBehaviour
{
    public Image armorBarImage;
    public Player player;

    public void Start()
    {
        player = MainManager.Instance.player;
    }

    void Update()
    {
        armorBarImage.fillAmount = Mathf.Clamp(player.Armor / player.maxHealth, 0, 1f);
    }
}
