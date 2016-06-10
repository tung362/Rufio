using UnityEngine;
using System.Collections;

public class PlayerDamage : MonoBehaviour
{
    private Animator TheAnimator;
    private GlobalVars Global;

	void Start ()
    {
        TheAnimator = GetComponentInChildren<Animator>();
        Global = FindObjectOfType<GlobalVars>();

    }

    void OnCollisionEnter(Collision other)
    {
        //Projectile
        if(other.gameObject.layer == LayerMask.NameToLayer("EnemyProjectile"))
        {
            //Hurt Animation
            TheAnimator.SetInteger("HurtID", other.gameObject.GetComponent<HitBoxDamage>().DamageID);
            //Damage
            Global.CurrentHealth -= other.gameObject.GetComponent<HitBoxDamage>().DamageValue;
            Destroy(other.gameObject);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        //Melee
        if (other.gameObject.layer == LayerMask.NameToLayer("EnemyHitBox") && other.gameObject.name == "EnemyHitBox")
        {
            //Hurt Animation
            //if(TheAnimator.GetInteger("HurtID") == 3 || TheAnimator.GetInteger("HurtID") == 4) TheAnimator.SetInteger("HurtID", 4);
            //else TheAnimator.SetInteger("HurtID", other.gameObject.GetComponent<HitBoxDamage>().DamageID);
            TheAnimator.SetInteger("HurtID", other.gameObject.GetComponent<HitBoxDamage>().DamageID);
            //Damage
            Global.CurrentHealth -= other.gameObject.GetComponent<HitBoxDamage>().DamageValue;
        }
    }
}
