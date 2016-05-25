using UnityEngine;
using System.Collections;

public class PlayerAnimation : MonoBehaviour
{
    //Animation
    [Header("Animation")]
    private Animator TheAnimator;
    private bool SetRandomIdle = true;
    private float AttackResetTimer = 0;
    public float AttackResetDelay = 1f;

    private PlayerManager PM;

    void Start()
    {
        PM = GetComponent<PlayerManager>();
        TheAnimator = GetComponentInChildren<Animator>();
    }

    public void Attack()
    {
        //Attack (WIP)
        if (TheAnimator.GetInteger("CurrentAttackChain") >= 2)
        {
            AttackResetTimer += Time.fixedDeltaTime;
            if (AttackResetTimer >= AttackResetDelay)
            {
                TheAnimator.SetInteger("CurrentAttackChain", 2);
                AttackResetTimer = 0;
            }

            if (PM.ATTACK == true)
            {
                AttackResetTimer = 0;
                TheAnimator.SetInteger("AttackID", TheAnimator.GetInteger("CurrentAttackChain"));
            }
        }
    }

    public void Animate()
    {
        bool isIdle = true;
        float forwardMovement = 0;
        float sideMovement = 0;

        //Manually force random idle states
        if (TheAnimator.GetCurrentAnimatorStateInfo(0).IsName("PlayerIdle1") == true ||
            TheAnimator.GetCurrentAnimatorStateInfo(0).IsName("PlayerIdle2") == true ||
            TheAnimator.GetCurrentAnimatorStateInfo(0).IsName("PlayerIdle3") == true ||
            TheAnimator.GetCurrentAnimatorStateInfo(0).IsName("PlayerIdle4") == true)
        {
            if (TheAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.5f)
            {
                if (SetRandomIdle == true)
                {
                    TheAnimator.SetInteger("RandomIdle", Random.Range(1, 5));
                    SetRandomIdle = false;
                }
            }
            else SetRandomIdle = true;
        }

        //Movement
        if (PM.UP == true)
        {
            isIdle = false;
            forwardMovement = Vector3.Dot(transform.forward, new Vector3(0, 0, 1));
            sideMovement = Vector3.Dot(transform.right, new Vector3(0, 0, 1));
        }
        if (PM.DOWN == true)
        {
            isIdle = false;
            forwardMovement = Vector3.Dot(transform.forward, new Vector3(0, 0, -1));
            sideMovement = Vector3.Dot(transform.right, new Vector3(0, 0, -1));
        }
        if (PM.RIGHT == true)
        {
            isIdle = false;
            forwardMovement = Vector3.Dot(transform.forward, new Vector3(1, 0, 0));
            sideMovement = Vector3.Dot(transform.right, new Vector3(1, 0, 0));
        }
        if (PM.LEFT == true)
        {
            isIdle = false;
            forwardMovement = Vector3.Dot(transform.forward, new Vector3(-1, 0, 0));
            sideMovement = Vector3.Dot(transform.right, new Vector3(-1, 0, 0));
        }

        if (PM.DODGE == true) TheAnimator.SetInteger("AttackID", 1);


        //Strafe
        if (PM.LOCKON == true)
        {
            if (TheAnimator.GetBool("IsStrafe") == true) TheAnimator.SetBool("IsStrafe", false);
            else TheAnimator.SetBool("IsStrafe", true);
        }


        //Set final results
        TheAnimator.SetFloat("MoveForward", forwardMovement);
        TheAnimator.SetFloat("MoveSide", sideMovement);
        TheAnimator.SetBool("IsIdle", isIdle);
    }
}
