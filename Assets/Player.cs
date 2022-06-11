using System;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.AI;

public class Player : MonoBehaviour
{
    public enum AnimType
    {
        Run,
        Charge,
        Attack,
        Die,
    }

    public NavMeshAgent agent;
    private Animator anim;

    public int Level = 1;
    public int maxHealth = 100;
    
    protected ReactiveProperty<int> CurrentHp { get; private set; } = new ReactiveProperty<int>(0);
    protected ReactiveProperty<int> CurrentArmor { get; private set; } = new ReactiveProperty<int>(0);
    public int Health
    {
        get => CurrentHp.Value;//последнее знаечение health в потоке
    }
    public int Armor
    {
        get => CurrentArmor.Value;
    }

    private float _ad = 25f;
    public float AttackDamage
    {
        get => _ad;// инкапсуляция
    }

    private int _as = 1000;
    public float AttackSpeed
    {
        get => _as;
    }

    private static Subject<Vector3> nextDestinationSubject = new Subject<Vector3>();
    private static Subject<Unit> navCompleteSubject;
    private static BehaviorSubject<Enemy> targetEnemySubject = new BehaviorSubject<Enemy>(null);

    public void setNextPosition(Vector3 pos)//получает от click to move
    {
        targetEnemySubject.OnNext(null);
        stopAttackAnim();
        agent.updateRotation = true;
        nextDestinationSubject.OnNext(pos);
    }
    public IObservable<Vector3> nextDestination()
    {
        if (nextDestinationSubject == null)
        {
            nextDestinationSubject = new Subject<Vector3>();
        }

        return nextDestinationSubject.AsObservable();
    }

    public IObservable<Unit> navComplete()
    {
        if (navCompleteSubject == null)
        {
            navCompleteSubject = new Subject<Unit>();
        }

        return navCompleteSubject.AsObservable();
    }

    public IObservable<Enemy> getTarget()
    {
        if (targetEnemySubject == null)
        {
            targetEnemySubject = new BehaviorSubject<Enemy>(null);
        }

        return targetEnemySubject.AsObservable();
    }

    public void setAttackTarget(Enemy enemy, Vector3? newDest = null)
    {
        targetEnemySubject.OnNext(enemy);
        if (newDest.HasValue) 
        {
            nextDestinationSubject.OnNext((Vector3)newDest);
        }
        else 
        {
            startAttack(enemy);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        if (PlayerPrefs.HasKey("currentLevel"))
        {
            Level = PlayerPrefs.GetInt("currentLevel");
        }

        if (PlayerPrefs.HasKey("currentHealth"))
        {
            CurrentHp.Value = PlayerPrefs.GetInt("currentHealth");
        }
        else
        {
            CurrentHp.Value = maxHealth;
        }

        if (PlayerPrefs.HasKey("currentArmor"))
        {
            CurrentArmor.Value = PlayerPrefs.GetInt("currentArmor");
        }
        else
        {
            CurrentArmor.Value = 50;
        }


        MainManager.Instance.IsPortalsReady 
            .Take(1)
            .Subscribe(_ => 
            {
                Vector3 pos;

                if (MainManager.Instance.portalLocations.TryGetValue(Level, out pos))
                {
                    Debug.Log(pos);
                    agent.Warp(pos);// телепортирует персонажа на уровень на котором он сохранился
                }
            })
            .AddTo(this);

        anim = GetComponent<Animator>();
        DieMenu dieMenu = FindObjectOfType<DieMenu>();

        this.UpdateAsObservable()
            .Select(_ => agent.remainingDistance <= 2.5f)
            .DistinctUntilChanged()
            .Where(isFinishing => isFinishing)
            .Subscribe(x => navCompleteSubject.OnNext(Unit.Default))
            .AddTo(this);

        nextDestination()
            .Do(_ => Debug.Log("nav start"))
            .Subscribe(dest => moveToDest(dest))
            .AddTo(this);

        navComplete()
            .Do(_ => Debug.Log("nav end"))
            .Subscribe(_ => {
                anim.SetBool(AnimType.Run.ToString(), false);
            })
            .AddTo(this);

        getTarget()
            .Where(enemy => enemy)
            .Select((Enemy enemy) => enemy.IsDead)
            .Switch()
            .Subscribe(_ => stopAttackAnim())
            .AddTo(this);

        CurrentHp
            .Where((int hp) => hp <= 0)
            .Take(1)
            .Do(_ => Die())
            .Delay(TimeSpan.FromMilliseconds(1100))
            .Do(_ => dieMenu.DiePause())
            .Subscribe()
            .AddTo(this);
    }

    public void moveToDest(Vector3 dest)// ускорение передвижения при атаке
    {
        agent.updateRotation = true;
        agent.destination = dest;
        anim.SetBool(AnimType.Attack.ToString(), false);
        anim.SetBool(AnimType.Charge.ToString(), false);

        if (targetEnemySubject.Value)
        {
            agent.speed = 42;
            anim.SetBool(AnimType.Run.ToString(), false);
            anim.SetBool(AnimType.Charge.ToString(), true);
        } else
        {
            agent.speed = 27;
            anim.SetBool(AnimType.Charge.ToString(), false);
            anim.SetBool(AnimType.Run.ToString(), true);
        }
    }

    public void TakeDamage(int ad)// отнимает здоровье если броня пустая
    {
        if (CurrentArmor.Value <= 0)
        {
            CurrentHp.Value -= ad;
        }
        else
        {
            CurrentArmor.Value -= ad;
        }
    }

    public void Heal(int hp)
    {
        if (CurrentHp.Value + hp > maxHealth)
        {
            CurrentHp.Value = maxHealth;
        }
        else
        {
            CurrentHp.Value += hp;
        }
    }

    public void AddArmor(int ap)
    {
        if (CurrentArmor.Value + ap > maxHealth)
        {
            CurrentArmor.Value = maxHealth;
        }
        else
        {
            CurrentArmor.Value += ap;
        }
    }

    public void startAttack(Enemy enemy)
    {
        if (targetEnemySubject.Value)
        {
            var rotation = Quaternion.LookRotation(targetEnemySubject.Value.transform.position - transform.position);
            rotation.y = 0;

            agent.updateRotation = false;

            Vector3 look = enemy.transform.position - Vector3.Scale(
                enemy.transform.forward,
                new Vector3(enemy.transform.localScale.x, 0, enemy.transform.localScale.z)
            );

            transform.LookAt(look);
        }

        enemy.IsUnderAttack.Value = true;
        anim.SetBool(AnimType.Attack.ToString(), true);
    }
    
    public void stopAttackAnim()
    {
        anim.SetBool(AnimType.Attack.ToString(), false);
        anim.SetBool(AnimType.Charge.ToString(), false);
        targetEnemySubject.OnNext(null);
    }
    public void Die()
    {
        // delete all saved data on player death
        PlayerPrefs.DeleteAll();
        anim.SetBool(AnimType.Die.ToString(), true);
    }
}
