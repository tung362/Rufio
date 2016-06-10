using UnityEngine;
using System.Collections;

public class PlayerBullet : MonoBehaviour
{
    public float Speed = 800;
    public Vector3 Direction = new Vector3(0, 0, 1);
    private Rigidbody TheRigidbody;

    void Start()
    {
        TheRigidbody = GetComponent<Rigidbody>();
    }

	void FixedUpdate ()
    {
        TheRigidbody.velocity = Direction * Speed * Time.fixedDeltaTime;
    }

    void OnCollisionEnter(Collision other)
    {
        if (other.collider.gameObject.layer == LayerMask.NameToLayer("Wall") || other.collider.gameObject.layer == LayerMask.NameToLayer("Terrain")) Destroy(gameObject);
    }
}
