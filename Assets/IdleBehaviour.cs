using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleBehaviour : StateMachineBehaviour
{
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    float timer;
    Transform player;
    Enemy enemy;
    float chaseRange = 30;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        timer = 0;
        player = GameObject.FindGameObjectWithTag("Player").transform;
        enemy = animator.gameObject.GetComponent<Enemy>();
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

        timer += Time.deltaTime;
        if (timer > 5){
            animator.SetBool("isPotroling", true);
        }
        float distance = Vector3.Distance(animator.transform.position, player.position);
        if (distance < chaseRange){
            animator.SetBool("isChasing", true);
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        
    }
}
