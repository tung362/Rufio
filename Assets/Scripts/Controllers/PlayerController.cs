using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    //Control
    [Header("Control")]
    public float Speed = 8;
    public float StrafeSpeed = 4;
    [HideInInspector]
    public bool CanTeleport = false;
    [HideInInspector]
    public int TeleportID = 0; //The id of the animation state thats about to teleport
    private GameObject StrafeTarget;
    private Vector3 TeleportDestination = Vector3.zero;
    private Vector3 TeleportMovement = Vector3.zero;
    private float ClosestTeleportDistance = int.MaxValue;
    private GameObject CollidedWall;
    private bool TeleportDestinationReached = true;
    public GameObject Camera;

    [Header("Animation Settings")]
    public float DashDistance = 5;
    public float DashSpeed = 80;
    public float StrafeAttackDistance = 3;
    public float NormalAttackDistance = 1;
    public float RunAttackDistance = 2;
    public float AttackSpeed = 25;

    private Rigidbody TheRigidbody;
    private Animator TheAnimator;

    private PlayerManager PM;

    void Start ()
    {
        PM = GetComponent<PlayerManager>();
        TheAnimator = GetComponentInChildren<Animator>();
        TheRigidbody = GetComponent<Rigidbody>();
    }

    public void Control()
    {
        Vector3 rotationMovement = Vector3.zero;
        Vector3 movement = Vector3.zero;

        //Find Closest Strafe Target
        if(TheAnimator.GetBool("IsStrafe") == true && TheAnimator.GetBool("Fall") == false && TheAnimator.GetLayerWeight(TheAnimator.GetLayerIndex("Fall")) <= 0 &&
             TheAnimator.GetBool("IsGun") == false && TheAnimator.GetBool("ChangingWeapons") == false)
        {
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
            if (enemies[0] == null) { }
            else
            {
                float closestDistance = Vector3.Distance(transform.position, enemies[0].transform.position);// int.MaxValue;
                StrafeTarget = enemies[0];
                for (int i = 1; i < enemies.Length; ++i)
                {
                    float distance = Vector3.Distance(transform.position, enemies[i].transform.position);
                    if (closestDistance > distance)
                    {
                        closestDistance = distance;
                        StrafeTarget = enemies[i];
                    }
                }
            }

            if (StrafeTarget != null)
            {
                transform.LookAt(new Vector3(StrafeTarget.transform.position.x, transform.position.y, StrafeTarget.transform.position.z));
            }
        }

        //Movement
        if (PM.UP == true)
        {
            rotationMovement.y = 1;
            if (TheAnimator.GetBool("IsStrafe") == false) movement = transform.forward;
            else movement = Camera.transform.forward;
        }

        if (PM.DOWN == true)
        {
            rotationMovement.y = -1;
            if (TheAnimator.GetBool("IsStrafe") == false) movement = transform.forward;
            else movement = -Camera.transform.forward;
        }

        if (PM.RIGHT == true)
        {
            rotationMovement.x = 1;
            if (TheAnimator.GetBool("IsStrafe") == false) movement = transform.forward;
            else movement = Camera.transform.right;
        }

        if (PM.LEFT == true)
        {
            rotationMovement.x = -1;
            if (TheAnimator.GetBool("IsStrafe") == false) movement = transform.forward;
            else movement = -Camera.transform.right;
        }


        //Apply rotation
        if (TheAnimator.GetBool("IsStrafe") == false && (PM.UP || PM.DOWN || PM.LEFT || PM.RIGHT) && TheAnimator.GetInteger("AttackID") == 0)
        {
            Quaternion previousRotation = transform.rotation;

            float rotationAngle = Vector3.Angle(new Vector3(0, 1, 0), rotationMovement);
            Vector3 cross = Vector3.Cross(new Vector3(0, 1, 0), rotationMovement);
            if (-cross.z < 0) rotationAngle = -rotationAngle;
            if (rotationMovement == Vector3.zero) rotationAngle = 0;
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, Camera.transform.rotation.eulerAngles.y + rotationAngle, transform.rotation.eulerAngles.z);

            //Prevent Rotation on hurt
            if (TheAnimator.GetInteger("HurtID") > 0) transform.rotation = previousRotation;

            //Prevent Rotation on landing of a fall
            if (TheAnimator.GetCurrentAnimatorStateInfo(TheAnimator.GetLayerIndex("Fall")).IsName("PlayerLand") == true)
            {
                if (TheAnimator.GetBool("Fall") == false && TheAnimator.GetCurrentAnimatorStateInfo(TheAnimator.GetLayerIndex("Fall")).normalizedTime < 1) transform.rotation = previousRotation;
            }
        }


        //Is it currently teleporting?
        if(TeleportDestinationReached == false)
        {
            TheRigidbody.velocity = TeleportMovement;

            if (ClosestTeleportDistance > Vector3.Distance(transform.position, TeleportDestination)) ClosestTeleportDistance = Vector3.Distance(transform.position, TeleportDestination);
            else
            {
                TeleportDestinationReached = true;
                if(CollidedWall != null) transform.position = TeleportDestination;
                TheRigidbody.velocity = Vector3.zero;
                gameObject.layer = LayerMask.NameToLayer("Player"); //Solid
                CollidedWall = null;
                ClosestTeleportDistance = int.MaxValue;
            }
        }
        else
        {
            //Apply Movement
            if (TheAnimator.GetBool("IsIdle") == false && TheAnimator.GetBool("IsStrafe") == false && TheAnimator.GetInteger("AttackID") == 0) TheRigidbody.velocity = new Vector3(movement.x * Speed, TheRigidbody.velocity.y, movement.z * Speed);
            else if (TheAnimator.GetBool("IsStrafe") == true && TheAnimator.GetInteger("AttackID") == 0) TheRigidbody.velocity = new Vector3(movement.x * StrafeSpeed, TheRigidbody.velocity.y, movement.z * StrafeSpeed);
            else TheRigidbody.velocity = new Vector3(TheRigidbody.velocity.x, TheRigidbody.velocity.y, TheRigidbody.velocity.z);

            if(TheAnimator.GetInteger("HurtID") > 0) TheRigidbody.velocity = new Vector3(0, 0, 0);

            //Prevent movement on landing of a fall
            if (TheAnimator.GetCurrentAnimatorStateInfo(TheAnimator.GetLayerIndex("Fall")).IsName("PlayerLand") == true)
            {
                if (TheAnimator.GetBool("Fall") == false && TheAnimator.GetCurrentAnimatorStateInfo(TheAnimator.GetLayerIndex("Fall")).normalizedTime < 1) TheRigidbody.velocity = new Vector3(0, 0, 0);
            }
        }

        //Teleport
        if (CanTeleport == true && TheAnimator.GetBool("Fall") == false && TheAnimator.GetInteger("HurtID") == 0)
        {
            TeleportDestinationReached = false;
            //Dash
            if (TeleportID == 1)
            {
                LineCastCollisionCheck(transform.position, transform.forward, DashDistance + 1);
                if(CollidedWall == null) TeleportDestination = transform.position + transform.forward * DashDistance;
                TeleportMovement = new Vector3(transform.forward.x, 0, transform.forward.z) * DashSpeed;
                gameObject.layer = LayerMask.NameToLayer("DodgeGhost"); //Ghost
            }
            //Attack
            else if (TeleportID >= 2)
            {
                //Strafe and normal attack has different lunge distance to prevent abuse of spamming attack as a second dash
                if(TheAnimator.GetBool("IsStrafe") == false)
                {
                    //Normal
                    if(TheAnimator.GetBool("IsIdle") == true)
                    {
                        LineCastCollisionCheck(transform.position, transform.forward, NormalAttackDistance + 1);
                        if (CollidedWall == null) TeleportDestination = transform.position + transform.forward * NormalAttackDistance;
                    }
                    else
                    {
                        LineCastCollisionCheck(transform.position, transform.forward, RunAttackDistance);
                        if (CollidedWall == null) TeleportDestination = transform.position + transform.forward * RunAttackDistance;
                    }
                }
                else
                {
                    //Strafe
                    LineCastCollisionCheck(transform.position, transform.forward, StrafeAttackDistance + 1);
                    if (CollidedWall == null) TeleportDestination = transform.position + transform.forward * StrafeAttackDistance;
                }
                TeleportMovement = new Vector3(transform.forward.x, 0, transform.forward.z) * AttackSpeed;
            }

            //Prevent movement on hurt
            if (TheAnimator.GetInteger("HurtID") > 0) TeleportMovement = new Vector3(0, 0, 0);

            //Prevent movement 
            if(TheAnimator.GetBool("IsGun") == true || TheAnimator.GetBool("ChangingWeapons") == true) TeleportMovement = new Vector3(0, 0, 0);

            //Prevent movement on landing of a fall
            if (TheAnimator.GetCurrentAnimatorStateInfo(TheAnimator.GetLayerIndex("Fall")).IsName("PlayerLand") == true)
            {
                if (TheAnimator.GetBool("Fall") == false && TheAnimator.GetCurrentAnimatorStateInfo(TheAnimator.GetLayerIndex("Fall")).normalizedTime < 1) TeleportMovement = new Vector3(0, 0, 0);
            }
            TeleportID = 0;
            CanTeleport = false;
        }
    }

    float shortestArc(float a, float b)
    {
        if (Mathf.Abs(b - a) < Mathf.PI)
            return b - a;
        if (b > a)
            return b - a - Mathf.PI * 2.0f;
        return b - a + Mathf.PI * 2.0f;
    }

    //Used for checking if player is going to run into any walls (Prevents player from clipping through)
    void LineCastCollisionCheck(Vector3 Start, Vector3 End, float Length)
    {
        RaycastHit[] hits = Physics.RaycastAll(Start, End, Length);

        float closestDistance = int.MaxValue;
        for (int i = 0; i < hits.Length; ++i)
        {
            if (hits[i].collider.gameObject.layer == LayerMask.NameToLayer("Wall"))
            {
                float distance = Vector3.Distance(transform.position, hits[i].point);
                if (closestDistance > distance)
                {
                    closestDistance = distance;
                    TeleportDestination = new Vector3(hits[i].point.x, hits[i].point.y, hits[i].point.z);
                    CollidedWall = hits[i].collider.gameObject;
                }
            }
        }
    }
}
