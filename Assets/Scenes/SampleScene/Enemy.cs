using UnityEngine;
using UnityEngine.UI;
using UniRx;
using System;
using UniRx.Triggers;

public class Enemy : MonoBehaviour {

    private Animator anim;
    private Player Player;
    private Text Title;
    private GameObject Glow;

    public float maxHp = 100;

    public ReactiveProperty<float> CurrentHp { get; private set; }

    public ReactiveProperty<bool> IsUnderAttack { get; private set; }
    public ReadOnlyReactiveProperty<bool> IsDead { get; private set; }


    void Start()
    {
        anim = this.GetComponent<Animator>();

        Player = MainManager.Instance.player;
        Glow = gameObject.transform.Find("enemy_glow")?.gameObject;

        CurrentHp = new ReactiveProperty<float>(maxHp);
        IsUnderAttack = new ReactiveProperty<bool>(false);
        IsDead = CurrentHp
            .Select(x => x <= 0)
            .Where((bool isDead) => isDead)
            .ToReadOnlyReactiveProperty();

        this.OnTriggerExitAsObservable()
            .Do(v => Debug.Log("untouch"))
            .Subscribe(_ => IsUnderAttack.Value = false);

        CurrentHp
            .Subscribe(v => Debug.Log(v));

        IsDead
            .Do(_ => Die())
            .Do(_ => IsUnderAttack.Value = false)
            .Do(v => Debug.Log("dead"))
            .Throttle(TimeSpan.FromMilliseconds(2000))
            .Subscribe(_ => this.gameObject.SetActive(false));

        IsUnderAttack
            .Do(v => Debug.Log(v))
            .Where((bool isAttacked) => isAttacked)
            .Select(_ => Observable.Interval(TimeSpan.FromMilliseconds(1000)).TakeUntil(IsUnderAttack.Where((bool flag) => !flag)))
            .Switch()
            .Do(v => Debug.Log(IsUnderAttack.Value))
            .Subscribe(_ => TakeDamage(Player.AttackDamage));
    }

    public void TakeDamage(float ad)
    {
        CurrentHp.Value -= ad;
    }
    public void Heal(float hp)
    {
        CurrentHp.Value += hp;
    }

    public void Wait()
    {
        if (anim)
        {
            anim.SetBool("Attack", false);
        }
    }

    public void Attack()
    {
        if (anim)
        {
            anim.SetBool("Attack", true);
        }
    }
    public void Die()
    {
        if (anim)
        {
            anim.SetBool("Die", true);
        }
    }
}
