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
    private float DistanceToGround = int.MaxValue;
    private Vector3 GroundPosition;

    [Header("Animation Settings")]
    public bool CanAttack = true;
    private float CanAttackTimer = 0;
    public float CanAttackDelay = 1;

    private PlayerManager PM;

    void Start()
    {
        PM = GetComponent<PlayerManager>();
        TheAnimator = GetComponentInChildren<Animator>();
    }

    public void Attack()
    {
        //Attack cool down
        if(CanAttack == false)
        {
            CanAttackTimer += Time.deltaTime;
            if(CanAttackTimer >= CanAttackDelay)
            {
                CanAttack = true;
                CanAttackTimer = 0;
            }
        }
        else
        {
            if (TheAnimator.GetBool("Fall") == false && TheAnimator.GetInteger("HurtID") == 0)
            {
                //Attack
                if (TheAnimator.GetInteger("CurrentAttackChain") >= 2)
                {
                    AttackResetTimer += Time.deltaTime;
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
        }
    }

    public void Animate()
    {
        bool isIdle = true;
        bool isBlock = false;
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

        if (PM.DODGE == true)
        {
            if(TheAnimator.GetBool("Fall") == false && TheAnimator.GetInteger("HurtID") == 0) TheAnimator.SetInteger("AttackID", 1);
        }

        if (PM.BLOCK == true)
        {
            if (TheAnimator.GetBool("Fall") == false && TheAnimator.GetInteger("HurtID") == 0) isBlock = true;
            //if(Input.GetKeyDown("e")) TheAnimator.SetBool("BlockHit", true);
        }

        //TestKey
        if(Input.GetKeyDown("e")) TheAnimator.SetInteger("HurtID", 2);


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
        TheAnimator.SetBool("IsBlock", isBlock);
    }

    public void Fall()
    {
        //Fall
        RayCastGroundCheck(transform.position, -transform.up, 100);
        if (DistanceToGround <= 1.5f)
        {
            TheAnimator.SetBool("Fall", false);
            transform.position = new Vector3(transform.position.x, transform.position.y + GroundPosition.y, transform.position.z);
        }
        else TheAnimator.SetBool("Fall", true);
    }

    void RayCastGroundCheck(Vector3 Start, Vector3 End, float Length)
    {
        RaycastHit[] hits = Physics.RaycastAll(Start, End, Length);

        float closestDistance = int.MaxValue;
        for (int i = 0; i < hits.Length; ++i)
        {
            if (hits[i].collider.gameObject.layer != LayerMask.NameToLayer("Player") || hits[i].collider.gameObject.layer != LayerMask.NameToLayer("Enemy"))
            {
                float distance = Vector3.Distance(transform.position, hits[i].point);
                if (closestDistance > distance)
                {
                    closestDistance = distance;
                    DistanceToGround = closestDistance;
                    GroundPosition = hits[i].point;
                }
            }
        }
    }
}
