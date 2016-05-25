using UnityEngine;
using System.Collections;

//  t_ denotes a Timer
//  s_ denotes a state
// st_ denotes a statistic
// ro_ denotes a raw object (not for direct use)

public class BasicBrain : MonoBehaviour
{
    /* STATES */
    private enum s_PlayerAttitude { IDLE, HOSTILE, ALERT }
    private enum s_Personality { FREIND, NEUTRAL, ENEMY }
    private bool s_HasSeenPlayer;
    /* TIMERS */
    private float t_HasSeenPlayer;
    /* STATISTICS */
    public float st_Speed;
    public float st_Damage;

    /* RAW OBJECTS */

	void Start ()
    {
	
	}
	

	void Update ()
    {
	
	}
}
