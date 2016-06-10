using UnityEngine;
using System.Collections;

//Destroy Object once a specific time in seconds is reached
public class DestroyTimer : MonoBehaviour
{
    private float Timer = 0;
    public float Delay = 4;
	
	// Update is called once per frame
	void FixedUpdate ()
    {
        Timer += Time.fixedDeltaTime;
        if (Timer >= Delay) Destroy(gameObject);
    }
}
