using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.AI;

public class Player : MonoBehaviour
{
    public PlayerInventory inventory;
    public NavMeshAgent agent;

    private Animator anim;

    private static Subject<Unit> navCompleteSubject;
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
            .Select(_ => agent.hasPath & agent.remainingDistance < 1f)
            .DistinctUntilChanged()
            .Where(isFinishing => isFinishing)
            .Subscribe(x => navCompleteSubject.OnNext(Unit.Default));

        this.UpdateAsObservable()
            .Select(_ => Vector3.SqrMagnitude(agent.velocity) > 0.5)
            .Subscribe((bool running) => anim.SetBool("Run", running));

    }

    public bool V3Equal(Vector3 a, Vector3 b)
    {
        return Vector3.SqrMagnitude(a - b) < 0.1;
    }

    // Update is called once per frame
    void Update()
    {
    }
}
