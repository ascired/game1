using System;
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
    }

    public PlayerInventory inventory;
    public NavMeshAgent agent;
    public float health = 100f;
    private Animator anim;

    private float _ad = 10f;
    public float AttackDamage
    {
        get => _ad;
    }

    private static Subject<Vector3> nextDestinationSubject = new Subject<Vector3>();
    private static Subject<Unit> navCompleteSubject;
    private static BehaviorSubject<Enemy> targetEnemySubject = new BehaviorSubject<Enemy>(null);

    public void setNextPosition(Vector3 pos)
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

    void OnTriggerExit(Collider other)
    {
        // Debug.Log(other.transform.name);
    }

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();

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
    }

    public void moveToDest(Vector3 dest)
    {        
        agent.destination = dest;

        anim.SetBool(AnimType.Attack.ToString(), false);

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

    public void startAttack(Enemy enemy)
    {
        if (targetEnemySubject.Value)
        {
            var rotation = Quaternion.LookRotation(targetEnemySubject.Value.transform.position - transform.position);
            rotation.y = 0;

            agent.updateRotation = false;

            Vector3 look = enemy.transform.position
                    - Vector3.Scale(
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
    }
}
