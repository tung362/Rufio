using UnityEngine;
using System.Collections;

//Fixes bug where hurt animation instantly turns back to idle when instead should be playing the next hurt animation
public class SetOnStartOfAnimation : StateMachineBehaviour
{
    public int HurtValue = 0;
    private int PreviousValue = 0;
    private bool Reset = false;


    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //Play next hurt animation on end of current animation
        if (PreviousValue > 0 && PreviousValue != HurtValue) animator.SetInteger("HurtID", PreviousValue);
        Reset = true;
    }

    override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //If there was any hurt animation transitions record it
        if (animator.GetInteger("HurtID") > 0 && animator.GetInteger("HurtID") != HurtValue)
        {
            PreviousValue = animator.GetInteger("HurtID");
        }

        //Resets values and enforces animation change
        if (Reset == true)
        {
            animator.SetInteger("HurtID", PreviousValue);
            PreviousValue = 0;
            Reset = false;
        }
    }
}
