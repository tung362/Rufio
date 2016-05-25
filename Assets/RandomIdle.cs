using UnityEngine;
using System.Collections;

public class RandomIdle : StateMachineBehaviour
{
    public float TriggerOnTime = 1;
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (stateInfo.normalizedTime >= TriggerOnTime)
        {
            animator.SetInteger("RandomIdle", Random.Range(0, 4));
        }
    }
}
