using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//  t_ denotes a Timer
//  s_ denotes a state
// st_ denotes a statistic
// ro_ denotes a raw object (typically not for direct use)

/// <summary>
/// This class is meant to be inherited from for other more speciallized AI types. 
/// It is designed to contain and handle basic interaction, but nothing too advanced.
/// </summary>
public class BasicBrain : MonoBehaviour
{
    /* INSPECTOR STUFF */
    public PlayerAttitude DefaultAttitude = PlayerAttitude.IDLE;
    public PlayerRelationship DefaultRelationship = PlayerRelationship.NEUTRAL;
    /* PUBLIC STUFF */
    [HideInInspector] public enum PlayerAttitude { IDLE, HOSTILE, ALERT };
    [HideInInspector] public enum PlayerRelationship { FREIND, NEUTRAL, ENEMY };
    /* BOOLEAN STATES */
    protected PlayerAttitude s_CurrentAttitude; //how do I react to my surroundings?
    protected PlayerRelationship s_CurrentRelationship; //Based on the player, who are my enemies?
    protected bool s_CanSeeEnemy; //Can I see an enemy?
    protected bool s_HasSeenEnemy; //Have I seen an enemy recently?
    protected bool s_CanAttack; //Is an attack possible?
    protected bool s_SearchForTar; //Am I looking for a target?
    [HideInInspector] public PlayerRelationship s_CurRel() { return s_CurrentRelationship; }
    /* TIMERS */
    private float t_HasSeenEnemy; //How long have I seen the enemy? (Determines if the AI can legally look for the target)
    /* STATISTICS */
    protected StatTracker st_Stats;
    protected GameObject st_CurTar; //My current targeted enemy.
    protected List<GameObject> st_EnemiesInSight = new List<GameObject>(); //The enemies I can see;
    protected Vector3 st_PlayerLocation() { return ro_Player.transform.position; } //One-way access to the player's location]
    protected Vector3 st_PlayerRelLoc() { return st_PlayerLocation() - transform.position; }
    protected Vector3 st_DirectionOf(Vector3 target, Vector3 from) { return (target - from).normalized; }
    private Vector3 st_TargetDirection(Vector3 target) { return (target - transform.position).normalized; }
    private int st_EnemyCount; //Keeps track of the amount enemies in the current scene, not for use by AI directly.
    /* RAW OBJECTS */
    protected Rigidbody ro_Me; //Rigidbody attached to the AI
    private GameObject ro_Player; //Player data is NOT directly accessable by the AI, it must go thorugh one-way access functions. Yay data protection!!   a
    private GameObject ro_MySpecialEyes; //This is an empty gameobject used by the AI to determine what it can see. 
    private NavMeshAgent ro_Nav; //the NavMesh

    /* STATE FUNCTIONS */
    #region
    /// <summary>
    /// This function finds all valid enemies in it's view cone
    /// </summary>
    /// <param name="validEnemies"></param>
    private void isEnemyInSight(GameObject[] validEnemies)
    {
        st_EnemiesInSight.Clear();

        for (int a = 0; a < validEnemies.Length; a++)
        {
            float ang = Vector3.Angle(validEnemies[a].transform.position, ro_MySpecialEyes.transform.forward);
            if (ang <= 22.5f)
            {
                RaycastHit losData;
                bool rayHit = Physics.Raycast(new Ray(ro_MySpecialEyes.transform.position, st_DirectionOf(validEnemies[a].transform.position, ro_MySpecialEyes.transform.position)), out losData);
                if (rayHit && losData.transform.tag == validEnemies[a].tag) { st_EnemiesInSight.Add(validEnemies[a]); s_CanSeeEnemy = true; }
            }
        }

        Debug.Log(Vector3.Angle(st_PlayerRelLoc(), ro_MySpecialEyes.transform.forward));

        if (s_CurrentRelationship == PlayerRelationship.ENEMY && Vector3.Angle(st_PlayerRelLoc(), ro_MySpecialEyes.transform.forward) <= 22.5f)
        {
            RaycastHit losData;
            bool rayHit = Physics.Raycast(new Ray(ro_MySpecialEyes.transform.position, st_DirectionOf(st_PlayerLocation(), ro_MySpecialEyes.transform.position)), out losData);
            if (rayHit && losData.transform.tag == "Player") { st_EnemiesInSight.Add(ro_Player); s_CanSeeEnemy = true; }
        }
    }
    /// <summary>
    /// This function is activated whenever an Enemy makes a sound.
    /// </summary>
    /// <returns></returns>
    private bool hasHeardEnemy()
    {
        return false;
    }
    private void setObservatioinalData()
    {
        // I'M BLIIIIIINNNNND!!!
        s_CanSeeEnemy = false; s_HasSeenEnemy = false;
        switch (s_CurrentRelationship)
        {
            case PlayerRelationship.ENEMY: isEnemyInSight(GameObject.FindGameObjectsWithTag("Friend")); break;
            case PlayerRelationship.FREIND: isEnemyInSight(GameObject.FindGameObjectsWithTag("Enemy")); break;
            case PlayerRelationship.NEUTRAL: break; //No behaviour
        }
    }
    void setCurrentAttitude()
    {
        if (s_CurrentRelationship == PlayerRelationship.ENEMY && s_CanSeeEnemy) s_CurrentAttitude = PlayerAttitude.HOSTILE;
        else s_CurrentAttitude = PlayerAttitude.IDLE;
    }
    #endregion
    /* BEHAVIOUR */
    #region
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
        List<GameObject> seir = transform.GetComponentInChildren<AttackRange>().st_EnemiesInRange;

        GameObject closest = seir[0];
        float comp = Vector3.Distance(transform.position, seir[0].transform.position);

        for (int a = 1; a < seir.Count; a++)
        {
            float dist = Vector3.Distance(transform.position, seir[a].transform.position);
            if (dist < comp)
            {
                closest = seir[a];
                comp = dist;
            }
        }
    }

    /// <summary>
    /// AI will look for a "lost" object. Tenacity is a multiplier for how long it will look. Used only for an immeadiate area.
    /// </summary>
    /// <param name="tenacity"></param>
    /// <param name="radius"></param>
    protected void Search(float tenacity, float radius)
    {

    }
    #endregion
    /* UNITY FUNCTIONS */
    #region
    void Awake()
    {
        s_CurrentAttitude = DefaultAttitude;
        s_CurrentRelationship = DefaultRelationship;
        st_Stats = GetComponent<StatTracker>();
        ro_Me = GetComponent<Rigidbody>();
        ro_Player = GameObject.FindGameObjectWithTag("Player");
        ro_MySpecialEyes = transform.FindChild("Eyes").gameObject;

        if (ro_MySpecialEyes == null)
        {
            var thing = new GameObject();
            thing.transform.position = transform.position;
            thing.transform.parent = transform;
        }
        if (st_Stats == null)
        {
            st_Stats = new StatTracker();
            st_Stats.reinit(100, 5, 2);
        }
    }

    void Update()
    {
        setCurrentAttitude();
        debugDrirectionalData();
    }

    void FixedUpdate()
    {
        setObservatioinalData();
    }
    #endregion
    /* DEBUG FUNCTIONS */
    #region
    void debugDrirectionalData()
    {
        if (s_CurrentAttitude == PlayerAttitude.HOSTILE && s_CanSeeEnemy)
        {
            MoveTo(st_PlayerLocation(), st_Stats.st_CurrentSpeed());
        }
        else Face(st_PlayerLocation());

    }
    #endregion
}
