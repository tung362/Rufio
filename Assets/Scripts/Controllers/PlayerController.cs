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
    private Vector3 TeleportDestination = new Vector3(0, 0, 0);
    private Vector3 TeleportMovement = new Vector3(0, 0, 0);
    private bool TeleportDestinationReached = true;
    public GameObject Camera;
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
        Vector3 rotationMovement = new Vector3(0, 0, 0);
        Vector3 movement = new Vector3(0, 0, 0);

        //Find Closest Strafe Target
        if(TheAnimator.GetBool("IsStrafe") == true)
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
        if(TheAnimator.GetBool("IsStrafe") == false && (PM.UP || PM.DOWN || PM.LEFT || PM.RIGHT) && TheAnimator.GetInteger("AttackID") == 0)
        {
            float rotationAngle = Vector3.Angle(new Vector3(0, 1, 0), rotationMovement);
            Vector3 cross = Vector3.Cross(new Vector3(0, 1, 0), rotationMovement);
            if (-cross.z < 0) rotationAngle = -rotationAngle;
            if (rotationMovement == Vector3.zero) rotationAngle = 0;
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, Camera.transform.rotation.eulerAngles.y + rotationAngle, transform.rotation.eulerAngles.z);
        }

        //Is it currently teleporting?
        if(TeleportDestinationReached == false)
        {
            TheRigidbody.velocity = TeleportMovement;
            if (Vector3.Distance(new Vector3(transform.position.x, 0, transform.position.z), TeleportDestination) <= 4)
            {
                TeleportDestinationReached = true;
                TheRigidbody.velocity = new Vector3(0, 0, 0);
                gameObject.layer = LayerMask.NameToLayer("Player"); //Ghost
            }
        }
        else
        {
            //Apply Movement
            if (TheAnimator.GetBool("IsIdle") == false && TheAnimator.GetBool("IsStrafe") == false && TheAnimator.GetInteger("AttackID") == 0) TheRigidbody.velocity = new Vector3(movement.x * Speed, TheRigidbody.velocity.y, movement.z * Speed);
            else if (TheAnimator.GetBool("IsStrafe") == true && TheAnimator.GetInteger("AttackID") == 0) TheRigidbody.velocity = new Vector3(movement.x * StrafeSpeed, TheRigidbody.velocity.y, movement.z * StrafeSpeed);
            else TheRigidbody.velocity = new Vector3(TheRigidbody.velocity.x, TheRigidbody.velocity.y, TheRigidbody.velocity.z);
        }

        //Teleport
        if (CanTeleport == true)
        {
            TeleportDestinationReached = false;
            //To do: limit to a distance and if reached change the velocity to 0
            if (TeleportID == 1)
            {
                TeleportDestination = new Vector3(transform.position.x + (transform.forward.x * 9), 0, transform.position.z + (transform.forward.z * 9));
                TeleportMovement = new Vector3(transform.forward.x, 0, transform.forward.z) * 80;
                gameObject.layer = LayerMask.NameToLayer("DodgeGhost"); //Ghost
            }
            else if (TeleportID >= 2)
            {
                TeleportDestination = new Vector3(transform.position.x + (transform.forward.x * 7), 0, transform.position.z + (transform.forward.z * 7));
                TeleportMovement = new Vector3(transform.forward.x, 0, transform.forward.z) * 30;
                //TheRigidbody.velocity = new Vector3(transform.forward.x, 0, transform.forward.z) * 30;
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
}
