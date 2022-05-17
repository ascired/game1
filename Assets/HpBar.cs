using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class HpBar : MonoBehaviour
{
    public Image hpBarImage;
    private Player player;
    private Enemy enemy;

    void Start()
    {
        gameObject.SetActive(false);
        player = MainManager.Instance.Player;
        player.getTarget()
            .Do((Enemy enemy) => 
            {
                this.enemy = enemy;
                gameObject.SetActive(enemy && enemy.gameObject.activeSelf);
            })
            .Subscribe()
            .AddTo(this);

    }
    void Update()
    {
        hpBarImage.fillAmount = Mathf.Clamp(enemy.CurrentHp.Value / enemy.maxHp, 0, 1f);
    }
}
