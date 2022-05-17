using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Image healthBarImage;
    public Player player;

    public void Start()
    {
        //Debug.Log("start: " + healthBarImage);
        player = MainManager.Instance.Player;
    }
    
    void Update()
    {
        //Debug.Log("update: " + healthBarImage);
        healthBarImage.fillAmount = Mathf.Clamp((float)player.Health / (float)player.maxHealth, 0, 1f);
    }
}
