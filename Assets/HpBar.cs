using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HpBar : MonoBehaviour
{
    public Image hpBarImage;
    public Enemy enemy;


    void Update()
    {
        hpBarImage.fillAmount = Mathf.Clamp(enemy.CurrentHp.Value / enemy.maxHp, 0, 1f);
    }
}
