using System;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.AI;

public class Player : MonoBehaviour
{
    public PlayerInventory inventory;
    public NavMeshAgent agent;

    private Animator anim;


    private static Subject<Vector3> nextDestinationSubject = new Subject<Vector3>();
    private static Subject<Unit> navCompleteSubject;

    public void setNextPosition(Vector3 pos)
    {
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

    void OnTriggerExit(Collider other)
    {
        // Debug.Log(other.transform.name);
    }

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();

        this.UpdateAsObservable()
            .Select(_ => agent.remainingDistance <= 1f)
            .DistinctUntilChanged()
            .Where(isFinishing => isFinishing)
            .Subscribe(x => navCompleteSubject.OnNext(Unit.Default));

        nextDestination()
            .Do(_ => Debug.Log("nav start"))
            .Subscribe(dest => moveToDest(dest));

        navComplete()
            .Do(_ => Debug.Log("nav end"))
            .Subscribe(_ => anim.SetBool("Run", false));

    }

    public void moveToDest(Vector3 dest)
    {        
        agent.destination = dest;
        anim.SetBool("Run", true);
    }

    // Update is called once per frame
    void Update()
    {
    }
}
