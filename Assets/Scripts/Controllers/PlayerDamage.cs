using UnityEngine;
using System.Collections;

//Handles objects that collides with the player and take damage accordingly
public class PlayerDamage : MonoBehaviour
{
    private Animator TheAnimator;
    private PlayerController ThePlayerController;
    private GlobalVars Global;

	void Start ()
    {
        TheAnimator = GetComponentInChildren<Animator>();
        ThePlayerController = GetComponent<PlayerController>();
        Global = FindObjectOfType<GlobalVars>();
    }

    void OnCollisionEnter(Collision other)
    {
        //Projectile
        if(other.gameObject.layer == LayerMask.NameToLayer("EnemyProjectile"))
        {
            //Hurt Animation
            TheAnimator.SetInteger("HurtID", other.gameObject.GetComponent<HitBoxDamage>().DamageID);
            //Look at the damager
            ThePlayerController.ManualRotation = other.transform.position;
            //Damage
            Global.CurrentHealth -= other.gameObject.GetComponent<HitBoxDamage>().DamageValue;
            Destroy(other.gameObject);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        //Melee
        if (other.gameObject.layer == LayerMask.NameToLayer("EnemyHitBox"))
        {
            //Hurt Animation
            if (TheAnimator.GetInteger("HurtID") == 3 || TheAnimator.GetInteger("HurtID") == 4) TheAnimator.SetInteger("HurtID", 4);
            else TheAnimator.SetInteger("HurtID", other.gameObject.GetComponent<HitBoxDamage>().DamageID);
            //Look at the damager
            ThePlayerController.ManualRotation = other.transform.root.transform.position;
            //Damage
            Global.CurrentHealth -= other.gameObject.GetComponent<HitBoxDamage>().DamageValue;
        }
    }
}
