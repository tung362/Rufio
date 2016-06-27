using UnityEngine;
using System.Collections;

public class TransitionToLayer : StateMachineBehaviour
{
    public bool OneWayTrip = false;
    public string ParameterName = "";
    public string[] TargetLayerName;
    public float TransitionSpeed = 1;

    [Header("Only check box 1 type")]
    public bool IsBool = false;
    public bool IsTrueFalse = false;
    [Space(10)]
    public bool IsFloat = false;
    public float IsFloatValue = 0;
    [Space(10)]
    public bool IsInt = false;
    public int IsIntValue = 0;

    [Header("Force setting to 1 or 0 instantly")]
    public bool ForceInstantTransition = false;

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

        for (int i = 0; i < TargetLayerName.Length; ++i)
        {
            if (ForceInstantTransition == false)
            {
                if (IsBool == true)
                {
                    if (animator.GetBool(ParameterName) == IsTrueFalse)
                    {
                        if (animator.GetLayerWeight(animator.GetLayerIndex(TargetLayerName[i])) < 1) animator.SetLayerWeight(animator.GetLayerIndex(TargetLayerName[i]), animator.GetLayerWeight(animator.GetLayerIndex(TargetLayerName[i])) + TransitionSpeed * Time.deltaTime);
                    }
                    else
                    {
                        if (animator.GetLayerWeight(animator.GetLayerIndex(TargetLayerName[i])) > 0 && OneWayTrip == false) animator.SetLayerWeight(animator.GetLayerIndex(TargetLayerName[i]), animator.GetLayerWeight(animator.GetLayerIndex(TargetLayerName[i])) - TransitionSpeed * Time.deltaTime);
                    }
                }
                else if (IsFloat == true)
                {
                    if (animator.GetFloat(ParameterName) == IsFloatValue)
                    {
                        if (animator.GetLayerWeight(animator.GetLayerIndex(TargetLayerName[i])) < 1) animator.SetLayerWeight(animator.GetLayerIndex(TargetLayerName[i]), animator.GetLayerWeight(animator.GetLayerIndex(TargetLayerName[i])) + TransitionSpeed * Time.deltaTime);
                    }
                    else
                    {
                        if (animator.GetLayerWeight(animator.GetLayerIndex(TargetLayerName[i])) > 0 && OneWayTrip == false) animator.SetLayerWeight(animator.GetLayerIndex(TargetLayerName[i]), animator.GetLayerWeight(animator.GetLayerIndex(TargetLayerName[i])) - TransitionSpeed * Time.deltaTime);
                    }
                }
                else if (IsInt == true)
                {
                    if (animator.GetInteger(ParameterName) == IsIntValue)
                    {
                        if (animator.GetLayerWeight(animator.GetLayerIndex(TargetLayerName[i])) < 1) animator.SetLayerWeight(animator.GetLayerIndex(TargetLayerName[i]), animator.GetLayerWeight(animator.GetLayerIndex(TargetLayerName[i])) + TransitionSpeed * Time.deltaTime);
                    }
                    else
                    {
                        if (animator.GetLayerWeight(animator.GetLayerIndex(TargetLayerName[i])) > 0 && OneWayTrip == false) animator.SetLayerWeight(animator.GetLayerIndex(TargetLayerName[i]), animator.GetLayerWeight(animator.GetLayerIndex(TargetLayerName[i])) - TransitionSpeed * Time.deltaTime);
                    }
                }
            }
            else
            {
                if (IsBool == true)
                {
                    if (animator.GetBool(ParameterName) == IsTrueFalse)
                    {
                        if (animator.GetLayerWeight(animator.GetLayerIndex(TargetLayerName[i])) < 1) animator.SetLayerWeight(animator.GetLayerIndex(TargetLayerName[i]), 1);
                    }
                    else
                    {
                        if (animator.GetLayerWeight(animator.GetLayerIndex(TargetLayerName[i])) > 0 && OneWayTrip == false) animator.SetLayerWeight(animator.GetLayerIndex(TargetLayerName[i]), 0);
                    }
                }
                else if (IsFloat == true)
                {
                    if (animator.GetFloat(ParameterName) == IsFloatValue)
                    {
                        if (animator.GetLayerWeight(animator.GetLayerIndex(TargetLayerName[i])) < 1) animator.SetLayerWeight(animator.GetLayerIndex(TargetLayerName[i]), 1);
                    }
                    else
                    {
                        if (animator.GetLayerWeight(animator.GetLayerIndex(TargetLayerName[i])) > 0 && OneWayTrip == false) animator.SetLayerWeight(animator.GetLayerIndex(TargetLayerName[i]), 0);
                    }
                }
                else if (IsInt == true)
                {
                    if (animator.GetInteger(ParameterName) == IsIntValue)
                    {
                        if (animator.GetLayerWeight(animator.GetLayerIndex(TargetLayerName[i])) < 1) animator.SetLayerWeight(animator.GetLayerIndex(TargetLayerName[i]), 1);
                    }
                    else
                    {
                        if (animator.GetLayerWeight(animator.GetLayerIndex(TargetLayerName[i])) > 0 && OneWayTrip == false) animator.SetLayerWeight(animator.GetLayerIndex(TargetLayerName[i]), 0);
                    }
                }
            }
        }
    }
}
