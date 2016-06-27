using UnityEngine;
using System.Collections;

/// **m_ denotes a function that modifies a value of the preceeding name prefix.

/// <summary>
/// All of a thing's basic stats, all in one place.
/// </summary>
public class StatTracker : MonoBehaviour
{
    public float baseHealth;
    //public float MP;
    public float baseDamage;
    public float baseSpeed;

    private float curHP;
    //private float curMP;
    private float curDMG;
    private float curSPD;

    public float st_CurrentHealth() { return curHP;  }
    public float st_CurrentDamage() { return curDMG; }
    public float st_CurrentSpeed()  { return curSPD; }

    public void stm_AddHealth(float mod) { curHP += mod; }
    public void stm_SubHealth(float mod) { curHP -= mod; }
    public void stm_AddDamage(float mod) { curDMG += mod; }
    public void stm_SubDamage(float mod) { curDMG -= mod; }
    public void stm_AddSpeed(float mod) { curSPD += mod; }
    public void stm_SubSpeed(float mod) { curSPD -= mod; }

	// Use this for initialization

    public void reinit(float hp, float dmg, float speed)
    {
        curHP = baseHealth = hp;
        curDMG = baseDamage = dmg;
        curSPD = baseSpeed = speed;
    }

    public void reset()
    {
        curHP = baseHealth;
        curDMG = baseDamage;
        curSPD = baseSpeed;
    }

	void Awake ()
    {
        curHP = baseHealth;
        curDMG = baseDamage;
        curSPD = baseSpeed;
	}
}
