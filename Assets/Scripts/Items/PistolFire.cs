using UnityEngine;
using System.Collections;

public class PistolFire : MonoBehaviour
{
    public float FireOnAnimationTime = 0.25f;
    private bool CanFire = true;
    public GameObject ProjectilePrefab;
    public GameObject AnimatorObject;
    private Animator TheAnimator;

	void Start ()
    {
        TheAnimator = AnimatorObject.GetComponent<Animator>();
    }
	
	void Update ()
    {
        if(TheAnimator.GetFloat("ShootForward") != 0 || TheAnimator.GetFloat("ShootSide") != 0)
        {
            if (CanFire == true)
            {
                if (TheAnimator.GetCurrentAnimatorStateInfo(TheAnimator.GetLayerIndex("Gun")).normalizedTime % 1 >= FireOnAnimationTime)
                {
                    GameObject bullet = (GameObject)Instantiate(ProjectilePrefab, transform.position, transform.rotation);
                    bullet.GetComponent<PlayerBullet>().Direction = new Vector3(transform.forward.x, 0, transform.forward.z);
                    //transform.rotation = Quaternion.Euler(transform.forward.x, transform.forward.y, transform.forward.z);
                    CanFire = false;
                }
            }
            else
            {
                if (TheAnimator.GetCurrentAnimatorStateInfo(TheAnimator.GetLayerIndex("Gun")).normalizedTime % 1 <= FireOnAnimationTime) CanFire = true;
            }
        }
    }
}
