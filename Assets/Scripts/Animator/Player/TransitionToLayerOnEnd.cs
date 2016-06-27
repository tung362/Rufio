using UnityEngine;
using System.Collections;

public class TransitionToLayerOnEnd : StateMachineBehaviour
{
    public float TriggerOnTime = 1;
    public string TargetLayerName;
    public float TransitionSpeed = 1;

    [Header("Force setting to 0 instantly")]
    public bool ForceInstantTransition = false;

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (stateInfo.normalizedTime >= TriggerOnTime)
        {
            for (int i = 0; i < TargetLayerName.Length; ++i)
            {
                if (ForceInstantTransition == false)
                {
                    if (animator.GetLayerWeight(animator.GetLayerIndex(TargetLayerName)) > 0) animator.SetLayerWeight(animator.GetLayerIndex(TargetLayerName), animator.GetLayerWeight(animator.GetLayerIndex(TargetLayerName)) - TransitionSpeed * Time.deltaTime);
                }
                else
                {
                    if (animator.GetLayerWeight(animator.GetLayerIndex(TargetLayerName)) > 0) animator.SetLayerWeight(animator.GetLayerIndex(TargetLayerName), 0);
                }
            }
        }
    }
}
