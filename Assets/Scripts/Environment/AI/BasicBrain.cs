using UnityEngine;
using System.Collections;

//  t_ denotes a Timer
//  s_ denotes a state
// st_ denotes a statistic
// ro_ denotes a raw object (typically not for direct use)

//Welcome to comment city. Usefull Population: 15,

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
    protected Vector3 st_PlayerLocation() { return ro_Player.transform.position; } //One-way access to the player's location

    private int st_EnemyCount; //Keeps track of the amount enemies in the current scene, not for use by AI directly.
    /* RAW OBJECTS */
    private GameObject ro_Player; //Player data is NOT directly accessable by the AI, it must go thorugh one-way access functions. Yay data protection!!   

    /* STATE FUNCTIONS */
    private bool isPlayerInSight()
    {
        if (Vector3.Angle(transform.forward, st_PlayerLocation()) <= 45.0f) return true;
        return false;
    }

    //This function does alot of work every frame... gotta be a way to optimize when it checks the nitty gritty stuff.
    void setObservatioinalData()
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
                if(enemies[0] != null) for(st_EnemyCount = 0; st_EnemyCount < enemies.Length - 1; st_EnemyCount++); // <- Don't kill me for this
                st_EnemyCount++; //Adds player to enemy count

                for (int a = st_EnemiesInSight; a < st_EnemyCount; a++)
                    if (Vector3.Angle(enemies[a].transform.position, transform.forward) <= 45.0f)
                    { st_EnemiesInSight++; s_CanSeeEnemy = true; }
                if (s_CanSeePlayer) { s_CanSeeEnemy = true; st_EnemiesInSight++; } //++ Always accounts for the player, since they will never be in enemies[]. MAGIC!!
                break;
            case PlayerRelationship.FREIND:
                //Same drek from ENEMY case, but this adds the player's enemies.
                GameObject[] enemies2 = GameObject.FindGameObjectsWithTag("Enemy");
                if (enemies2[0] != null) for (st_EnemyCount = 0; st_EnemyCount < enemies2.Length - 1; st_EnemyCount++);

                for (int a = st_EnemiesInSight; a < st_EnemyCount; a++)
                    if (Vector3.Angle(enemies2[a].transform.position, transform.forward) <= 45.0f)
                    { st_EnemiesInSight++; s_CanSeeEnemy = true; }

                break;
            case PlayerRelationship.NEUTRAL:  break; //Don't got's nothin' fo' dis behaviour yet.
        }
    }

    void setCurrentAttitude()
    {
        

    }
     


    /* UNITY FUNCTIONS */
	void Start ()
    {
        s_CurrentAttitude = DefaultAttitude;
        s_CurrentRelationship = DefaultRelationship;
        st_Speed = BaseSpeed;
        st_Damage = BaseDamage;
    }


    void Update()
    {
        setObservatioinalData();

        switch (s_CurrentAttitude)
        {
            case PlayerAttitude.ALERT: break;
            case PlayerAttitude.HOSTILE: break;
            case PlayerAttitude.IDLE: break;
        }
    }
}
