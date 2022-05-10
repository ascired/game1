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
    public float maxHp = 100f;
    private float attackDamage = 1;
    public int attackSpeed = 500;

    public ReactiveProperty<float> CurrentHp { get; private set; }
    public ReactiveProperty<bool> IsUnderAttack { get; private set; }
    public ReadOnlyReactiveProperty<bool> IsDead { get; private set; }
    public ReactiveProperty<bool> IsAttacking { get; private set; }

    void Start()
    {
        anim = this.GetComponent<Animator>();

        Player = MainManager.Instance.player;
        Glow = gameObject.transform.Find("enemy_glow")?.gameObject;

        CurrentHp = new ReactiveProperty<float>(maxHp);
        IsUnderAttack = new ReactiveProperty<bool>(false);
        IsAttacking = new ReactiveProperty<bool>(false);
        IsDead = CurrentHp
            .Select(x => x <= 0)
            .Where((bool isDead) => isDead)
            .ToReadOnlyReactiveProperty();

        this.OnTriggerExitAsObservable()
            .Subscribe(_ => {
                IsUnderAttack.Value = false;
                IsAttacking.Value = false;
            });

        IsDead
            .Do(_ => Die())
            .Do(_ => {
                IsUnderAttack.Value = false;
                IsAttacking.Value = false;
            })
            .Do(v => Debug.Log("dead"))
            .Throttle(TimeSpan.FromMilliseconds(2000))
            .Subscribe(_ => this.gameObject.SetActive(false));

        IsUnderAttack
            .Where((bool isAttacked) => isAttacked)
            .Do(_ => TakeDamage(Player.AttackDamage))
            .Select(_ => Observable.Interval(TimeSpan.FromMilliseconds(Player.AttackSpeed)).TakeUntil(IsUnderAttack.Where((bool flag) => !flag)))
            .Switch()
            .Subscribe(_ => TakeDamage(Player.AttackDamage));

        this.UpdateAsObservable()
            .Select(_ => Vector3.Distance(new Vector3(x: gameObject.transform.position.x, y: 0, z: gameObject.transform.position.z), new Vector3(x: Player.agent.transform.position.x, y: 0, z: Player.agent.transform.position.z)))
            .TakeUntil(IsDead)
            .Subscribe((float dist) => {
                IsAttacking.Value = dist <= MainManager.Instance.attackDistanceThreshold;
            });

        IsAttacking
            .DistinctUntilChanged()
            .Do(v => Debug.Log(v))
            .Do(v => Debug.Log(this.gameObject.activeSelf))
            .Do((bool isAttacking) => Attack(isAttacking))
            .Where((bool isAttacking) => isAttacking)
            .Select(_ => Observable.Interval(TimeSpan.FromMilliseconds(attackSpeed)).TakeUntil(IsAttacking.Where((bool flag) => !flag)))
            .Switch()
            .Subscribe(_ => Player.TakeDamage(attackDamage));
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

    public void Attack(bool isAttacking)
    {
        if (anim)
        {
            anim.SetBool("Attack", isAttacking);
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
