using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PatrolBehaviaour : StateMachineBehaviour
{
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    float timer;
    List<Transform> points = new List<Transform>();
    NavMeshAgent agent;
    Transform player;
    Enemy enemy;
    float chaseRange = 30;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        timer = 0;

        player = GameObject.FindGameObjectWithTag("Player").transform;
        enemy = animator.gameObject.GetComponent<Enemy>();

        foreach (GameObject t in enemy.points)
        {
            points.Add(t.transform);
        }

        agent = animator.GetComponent<NavMeshAgent>();
        agent.SetDestination(points[0].position);
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (enemy.IsDead.Value)
        {
            animator.SetBool("isPotroling", false);
            animator.SetBool("isChasing", false);
            return;
        }

        if (agent.remainingDistance <= 1f){
            agent.SetDestination(points[Random.Range(0, points.Count)].position);
        }

        timer += Time.deltaTime;
        if (timer > 10){
            animator.SetBool("isPotroling", false);
        }

        float distance = Vector3.Distance(animator.transform.position, player.position);
        if (distance < chaseRange){
            animator.SetBool("isChasing", true);
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (agent.enabled)
        {
            agent.SetDestination(enemy.transform.position);
        }
    }
}
