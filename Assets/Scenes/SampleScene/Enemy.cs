using UnityEngine;
using UnityEngine.UI;
using UniRx;
using System;
using UniRx.Triggers;
using UnityEngine.AI;
using System.Collections.Generic;

public class Enemy : MonoBehaviour {

    private Animator anim;
    private Player Player;
    private Text Title;
    public GameObject AttackEffect = null;
    public int maxHp = 100;
    public int attackDamage = 3;
    public int attackSpeed = 1090;
    public bool melee = false;
    public List<GameObject> points = null;
    private float attackDistance;

    public ReactiveProperty<float> CurrentHp { get; private set; }
    public ReactiveProperty<bool> IsUnderAttack { get; private set; }
    public ReadOnlyReactiveProperty<bool> IsDead { get; private set; }
    public ReactiveProperty<bool> IsAttacking { get; private set; }

    void Start()
    {
        anim = this.GetComponent<Animator>();

        Player = MainManager.Instance.Player;
        attackDistance = melee ? MainManager.Instance.meleeAttackDistanceThreshold : MainManager.Instance.rangeAttackDistanceThreshold;

        CurrentHp = new ReactiveProperty<float>(maxHp);
        IsUnderAttack = new ReactiveProperty<bool>(false);
        IsAttacking = new ReactiveProperty<bool>(false);

        IsDead = CurrentHp
            .Select(x => x <= 0)
            .Where((bool isDead) => isDead)
            .ToReadOnlyReactiveProperty();

        Player.getTarget()
            .Where((Enemy enemy) => !enemy | enemy?.GetInstanceID() != this.GetInstanceID())
            .Subscribe(_ => {
                IsUnderAttack.Value = false;
            })
            .AddTo(this);

        IsDead
            .Do(_ => Die())
            .Do(_ => {
                Collider collider = gameObject.GetComponent<Collider>();
                if (collider)
                {
                    collider.enabled = false;
                }

                NavMeshAgent agent = gameObject.GetComponent<NavMeshAgent>();
                if (agent)
                {
                    agent.speed = 0;
                    agent.isStopped = true;
                    agent.enabled = false;
                }

                IsUnderAttack.Value = false;
                IsAttacking.Value = false;
            })
            .Do(v => Debug.Log("dead"))
            .Throttle(TimeSpan.FromMilliseconds(2000))
            .Subscribe(_ => this.gameObject.SetActive(false))
            .AddTo(this);

        IsUnderAttack
            .Where((bool isAttacked) => isAttacked)
            .Select(_ => Observable.Interval(TimeSpan.FromMilliseconds(Player.AttackSpeed)).TakeUntil(IsUnderAttack.Where((bool flag) => !flag)))
            .Switch()
            .Subscribe(_ => TakeDamage(Player.AttackDamage))
            .AddTo(this);

        this.UpdateAsObservable()
            .Select(_ => Vector3.Distance(new Vector3(x: gameObject.transform.position.x, y: 0, z: gameObject.transform.position.z), new Vector3(x: Player.agent.transform.position.x, y: 0, z: Player.agent.transform.position.z)))
            .TakeUntil(IsDead)
            .Subscribe((float dist) => {
                IsAttacking.Value = dist <= attackDistance;
            })
            .AddTo(this);

        IsAttacking
            .DistinctUntilChanged()
            .TakeUntil(IsDead)
            .Do(v => Debug.Log(v))
            .Do((bool isAttacking) => Attack(isAttacking))
            .Where((bool isAttacking) => isAttacking)
            .Select(_ => Observable.Interval(TimeSpan.FromMilliseconds(attackSpeed)).TakeUntil(IsAttacking.Where((bool flag) => !flag)))
            .Switch()
            .Do(_ =>
            {
                this.transform.LookAt(Player.transform.position);
                if (AttackEffect)
                {
                    Instantiate(AttackEffect, Player.transform.position, Quaternion.identity);
                }
            })
            .Subscribe(_ => Player.TakeDamage(attackDamage))
            .AddTo(this);

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
