using UnityEngine;
using System.Collections;

//  t_ denotes a Timer
//  s_ denotes a state
// st_ denotes a statistic
// ro_ denotes a raw object (typically not for direct use)

//Welcome to comment city. Usefull Population: 15,

    /// <summary>
    /// This class is meant to be inherited from for other more speciallized AI types. 
    /// It is designed to contain and handle basic interaction, but nothing too advanced.
    /// </summary>
public class BasicBrain : MonoBehaviour
{
    /* INSPECTOR STUFF */
    public float BaseSpeed;
    public float BaseDamage;
    public PlayerAttitude DefaultAttitude = PlayerAttitude.IDLE;
    public PlayerRelationship DefaultRelationship = PlayerRelationship.NEUTRAL;
    /* ENUMS BECAUSE I LIKE THEM */
    [HideInInspector] public enum PlayerAttitude { IDLE, HOSTILE, ALERT };
    [HideInInspector] public enum PlayerRelationship { FREIND, NEUTRAL, ENEMY };
    /* STATES */
    protected PlayerAttitude s_CurrentAttitude; //how do I react to my surroundings?
    protected PlayerRelationship s_CurrentRelationship; //Based on the player, who are my enemies?
    protected bool s_CanSeePlayer; //Can I see the player?
    protected bool s_HasSeenPlayer; //Have I seen the player recently?
    protected bool s_CanSeeEnemy; //Can I see an enemy?
    /* TIMERS */
    private float t_HasSeenPlayer; //How long have I seen the player? (Determines if the AI can legally look for the player)
    /* STATISTICS */
    protected float st_Speed; //What is my current speed?
    protected float st_Damage; //What is my current damage?
    protected int st_EnemiesInSight; //How many enemies can I see?
    protected Vector3 st_PlayerLocation() { return ro_Player.transform.position; } //One-way access to the player's location]
    protected Vector3 st_PlayerRelLoc() { return st_PlayerLocation() - transform.position; }

    private Vector3 st_TargetDirection(Vector3 target) { return (target - transform.position).normalized; }
    private int st_EnemyCount; //Keeps track of the amount enemies in the current scene, not for use by AI directly.
    /* RAW OBJECTS */
    protected Rigidbody ro_Me; //Rigidbody attached to the AI

    private GameObject ro_Player; //Player data is NOT directly accessable by the AI, it must go thorugh one-way access functions. Yay data protection!!   

    /**/
    private NavMeshAgent nma;


    /* STATE FUNCTIONS */
    private bool isPlayerInSight()
    {
        if (Vector3.Angle(st_PlayerRelLoc(), transform.forward) <= 22.5f) return true;
        Debug.Log(Vector3.Angle(st_PlayerRelLoc(), transform.forward));
        return false;
    }

    //This function does alot of work every frame... gotta be a way to optimize when it checks the nitty gritty stuff.
    private void setObservatioinalData()
    {
        // I'M BLIIIIIINNNNND!!!
        s_CanSeePlayer = false; s_HasSeenPlayer = false; s_CanSeeEnemy = false;
        st_EnemiesInSight = 0; 

        if (isPlayerInSight()) { s_CanSeePlayer = true; t_HasSeenPlayer += Time.deltaTime; }
        else if (t_HasSeenPlayer > 0.0f) { s_HasSeenPlayer = true; t_HasSeenPlayer -= Time.deltaTime; }
        else t_HasSeenPlayer = 0.0f; //No negative timers (NOT EVEN A LITTLE)!!

        switch (s_CurrentRelationship)
        {
            case PlayerRelationship.ENEMY:
                //This part is gross, but it works.
                GameObject[] enemies = GameObject.FindGameObjectsWithTag("Friend");
                if (enemies != null)
                {
                    st_EnemyCount = enemies.Length;
                    for (int a = st_EnemiesInSight; a < st_EnemyCount; a++)
                        if (Mathf.Abs(Vector3.Angle(enemies[a].transform.position, transform.forward)) <= 45.0f)
                        { st_EnemiesInSight++; s_CanSeeEnemy = true; }
                }

                st_EnemyCount++; //Adds player to enemy count (so if said player is an enemy, st_EC will always be at least one.)
                if (s_CanSeePlayer) { s_CanSeeEnemy = true; st_EnemiesInSight++; } //++ Always accounts for the player, since they will never be in enemies[]. MAGIC!!
                break;
            case PlayerRelationship.FREIND:
                //Same drek from ENEMY case, but this adds the player's enemies.
                GameObject[] enemies2 = GameObject.FindGameObjectsWithTag("Enemy");
                if (enemies2 != null)
                {
                    st_EnemyCount = enemies2.Length;
                    for (int a = st_EnemiesInSight; a < st_EnemyCount; a++)
                        if (Vector3.Angle(enemies2[a].transform.position, transform.forward) <= 45.0f)
                        { st_EnemiesInSight++; s_CanSeeEnemy = true; }
                }
                break;
            case PlayerRelationship.NEUTRAL:  break; //Don't got's nothin' fo' dis behaviour yet.
        }
    }

    void setCurrentAttitude()
    {
        if (s_CurrentRelationship == PlayerRelationship.ENEMY && s_CanSeePlayer) s_CurrentAttitude = PlayerAttitude.HOSTILE;
        else s_CurrentAttitude = PlayerAttitude.IDLE;
    }

    /* DEBUG FUNCTIONS */

    void debugDrirectionalData()
    {
        if (s_CurrentAttitude == PlayerAttitude.HOSTILE && s_CanSeePlayer)
        {
            MoveTo(st_PlayerLocation(), st_Speed);
            Debug.Log("I SEE YOU~! ");
        }
        else Face(st_PlayerLocation());
        Debug.DrawLine(transform.position, transform.position + transform.forward);
    }

    /* BEHAVIOUR */
    /// <summary>
    /// Turn towards a target vector. Awareness is a multiplier for how fast it moves.
    /// </summary>
    /// <param name="target"></param>
    /// <param name="awareness"></param>
    protected void Face(Vector3 target, bool lookAtY = false, float awareness = 3.0f)
    {
        Vector3 dir = (target - transform.position).normalized;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), awareness * Time.deltaTime);
    }

    /// <param name="target"></param>
    /// <param name="speed"></param>
    protected void MoveTo(Vector3 target, float speed)
    {
        target -= transform.position;
        ro_Me.velocity = new Vector3(target.x, 0, target.z).normalized * speed;
        //ro_Me.AddForce(st_TargetDirection(target) * speed);
    }

    /// <summary>
    /// Basic attack routine. Use for enemies that don't need more intelligent attack system.
    /// </summary>
    protected void Attack()
    {

    }

    /// <summary>
    /// AI will look for a "lost" object. Tenacity is a multiplier for how long it will look. Used only for an immeadiate area.
    /// </summary>
    /// <param name="tenacity"></param>
    /// <param name="radius"></param>
    protected void Search(float tenacity, float radius)
    {

    }

    /* UNITY FUNCTIONS */
	 void Start ()
    {
        s_CurrentAttitude = DefaultAttitude;
        s_CurrentRelationship = DefaultRelationship;
        st_Speed = BaseSpeed;
        st_Damage = BaseDamage;
        ro_Me = GetComponent<Rigidbody>();
        ro_Player = GameObject.FindGameObjectWithTag("Player");
    }


     void Update()
    {
        setObservatioinalData();
        setCurrentAttitude();
    }

    void FixedUpdate()
    {
        debugDrirectionalData();
    }
}
