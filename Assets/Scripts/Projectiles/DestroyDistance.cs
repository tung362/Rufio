using UnityEngine;
using System.Collections;

//Destroy Object once it moves a specific distance away from where it was spawned
public class DestroyDistance : MonoBehaviour
{
    public float Distance = 10;
    private Vector3 StartingPosition = Vector3.zero;

	// Use this for initialization
	void Start ()
    {
        StartingPosition = transform.position;
    }
	
	// Update is called once per frame
	void FixedUpdate ()
    {
        if (Vector3.Distance(StartingPosition, transform.position) >= Distance) Destroy(gameObject);
	}
}
