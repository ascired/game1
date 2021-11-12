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
            .Select(_ => agent.velocity > Vector3.zero)
            .Subscribe((bool running) => anim.SetBool("Run", running));

    }

    // Update is called once per frame
    void Update()
    {
    }
}
