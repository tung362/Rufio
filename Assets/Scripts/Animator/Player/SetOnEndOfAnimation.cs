using UnityEngine;
using System.Collections;

public class SetOnEndOfAnimation : StateMachineBehaviour
{
    public float TriggerOnTime = 1;

    [Space(10)]
    public bool ModifyRandomIdle = false;
    public int RandomIdleValue = 0;

    [Space(10)]
    public bool ModifyMoveForward = false;
    public float MoveForwardValue = 0;

    [Space(10)]
    public bool ModifyMoveSide = false;
    public float MoveSideValue = 0;

    [Space(10)]
    public bool ModifyAttackID = false;
    public int AttackIDValue = 0;

    [Space(10)]
    public bool ModifyIsIdle = false;
    public bool IsIdleValue = false;

    [Space(10)]
    public bool ModifyIsStrafe = false;
    public bool IsStrafeValue = false;

    [Space(10)]
    public bool ModifyFall = false;
    public bool FallValue = false;

    [Space(10)]
    public bool ModifyHurt = false;
    public int HurtValue = 0;

    [Space(10)]
    public bool ModifyIsBlock = false;
    public bool IsBlockValue = false;

    [Space(10)]
    public bool ModifyBlockHit = false;
    public bool BlockHitValue = false;

    [Space(10)]
    public bool ModifyChangingWeapons = false;
    public bool ChangingWeaponsValue = false;

    [Space(10)]
    public bool ModifyWeaponID = false;
    public int WeaponIDValue = 1;

    [Space(10)]
    public bool ModifyIsSword = false;
    public bool IsSwordValue = false;

    [Space(10)]
    public bool ModifyIsGun = false;
    public bool IsGunValue = false;

    [Space(10)]
    public bool ModifyShootForward = false;
    public float ShootForwardValue = 0;

    [Space(10)]
    public bool ModifyShootSide = false;
    public float ShootSideValue = 0;

    //[Space(10)]
    [Header("Used by player controller")]
    public bool EnableCanTeleport = false;
    public bool CanTeleport = true;
    public float CanTeleportTriggerOnTime = 1;
    public int id = 0;

    [Header("Used by attacks")]
    public bool EnableCanIncreaseCurrentAttackChain = false;

    //Prevents ticking more than once
    private bool RunOnce = true;

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (stateInfo.normalizedTime >= TriggerOnTime)
        {
            //Teleport
            if (CanTeleportTriggerOnTime == TriggerOnTime && EnableCanTeleport == true && CanTeleport == true)
            {
                animator.transform.root.GetComponent<PlayerController>().CanTeleport = true;
                animator.transform.root.GetComponent<PlayerController>().TeleportID = id;
                CanTeleport = false;
            }

            if (ModifyRandomIdle == true) animator.SetInteger("RandomIdle", RandomIdleValue);
            if (ModifyMoveForward == true) animator.SetFloat("MoveForward", MoveForwardValue);
            if (ModifyMoveSide == true) animator.SetFloat("MoveSide", MoveSideValue);
            if (ModifyAttackID == true) animator.SetInteger("AttackID", AttackIDValue);
            if (ModifyIsIdle == true) animator.SetBool("IsIdle", IsIdleValue);
            if (ModifyIsStrafe == true) animator.SetBool("IsStrafe", IsStrafeValue);
            if (ModifyFall == true) animator.SetBool("Fall", FallValue);
            if (ModifyHurt == true) animator.SetInteger("HurtID", HurtValue);
            if (ModifyIsBlock == true) animator.SetBool("IsBlock", IsBlockValue);
            if (ModifyBlockHit == true) animator.SetBool("BlockHit", BlockHitValue);
            if (ModifyChangingWeapons == true) animator.SetBool("ChangingWeapons", ChangingWeaponsValue);
            if (ModifyWeaponID == true) animator.SetInteger("WeaponID", WeaponIDValue);
            if (ModifyIsSword == true) animator.SetBool("IsSword", IsSwordValue);
            if (ModifyIsGun == true) animator.SetBool("IsGun", IsGunValue);
            if (ModifyShootForward == true) animator.SetFloat("ShootForward", ShootForwardValue);
            if (ModifyShootSide == true) animator.SetFloat("ShootSide", ShootSideValue);

            RunOnce = false;
        }
        else
        {
            if (CanTeleportTriggerOnTime == TriggerOnTime && EnableCanTeleport == true && CanTeleport == false) CanTeleport = true;
        }

        //Attack switch
        if (stateInfo.normalizedTime >= 0.9f)
        {
            //Prevents ticking more than once
            if (RunOnce == true)
            {
                if (EnableCanIncreaseCurrentAttackChain == true)
                {
                    animator.SetInteger("CurrentAttackChain", animator.GetInteger("CurrentAttackChain") + 1);
                    //Set to 3 since strafe dont have an idle attack
                    if (animator.GetBool("IsStrafe") == true && animator.GetInteger("CurrentAttackChain") < 3) animator.SetInteger("CurrentAttackChain", 3);
                    if (animator.GetInteger("CurrentAttackChain") > 5)
                    {
                        animator.transform.root.GetComponent<PlayerAnimation>().CanAttack = false;
                        animator.SetInteger("CurrentAttackChain", 2);
                    }
                }
                RunOnce = false;
            }
        }
        else
        {
            if (RunOnce == false) RunOnce = true;
        }

        //Teleport
        if (CanTeleportTriggerOnTime != TriggerOnTime && EnableCanTeleport == true)
        {
            if (stateInfo.normalizedTime >= CanTeleportTriggerOnTime)
            {
                if(CanTeleport == true)
                {
                    animator.transform.root.GetComponent<PlayerController>().CanTeleport = true;
                    animator.transform.root.GetComponent<PlayerController>().TeleportID = id;
                    CanTeleport = false;
                }
            }
            else CanTeleport = true;
        }
    }
}
