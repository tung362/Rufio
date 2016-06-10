using UnityEngine;
using System.Collections;

public class SetOnStartOfAnimation : StateMachineBehaviour
{
    public int HurtValue = 0;
    private int PreviousValue = 0;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    //override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    animator.SetInteger("HurtID", HurtValue);
    //}

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Debug.Log("Previous: " + PreviousValue);
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (PreviousValue > 0 && PreviousValue != HurtValue)
        {
            animator.SetInteger("HurtID", PreviousValue);
        }
        PreviousValue = 0;
    }

    // OnStateMove is called right after Animator.OnAnimatorMove(). Code that processes and affects root motion should be implemented here
    override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (animator.GetInteger("HurtID") > 0 && animator.GetInteger("HurtID") != HurtValue)
        {
            PreviousValue = animator.GetInteger("HurtID");
        }
        //Debug.Log("Previous: " + PreviousValue);
        //Debug.Log("ID: " + animator.GetInteger("HurtID"));
    }

    // OnStateIK is called right after Animator.OnAnimatorIK(). Code that sets up animation IK (inverse kinematics) should be implemented here.
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
    //
    //}
}
