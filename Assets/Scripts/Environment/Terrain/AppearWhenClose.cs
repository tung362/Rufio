using UnityEngine;
using System.Collections;

public class AppearWhenClose : MonoBehaviour
{
    public float FadeSpeed = 0.3f;
    public float DistanceToActivate = 2;
    public bool InstantTransition = false; //If there should even be a fade or not
	void Update ()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        bool activate = false;
        for(int i = 0; i < players.Length; ++i)
        {
            if(Vector3.Distance(transform.position, players[i].transform.position) <= DistanceToActivate)
            {
                activate = true;
                break;
            }
        }

        if(activate == true)
        {
            if(GetComponent<Renderer>().material.color.a < 1)
            {
                if (InstantTransition == false)
                {
                    Color newAlpha = new Vector4(0, 0, 0, FadeSpeed) * Time.deltaTime;
                    GetComponent<Renderer>().material.color += newAlpha;
                }
                else
                {
                    Color newAlpha = new Vector4(GetComponent<Renderer>().material.color.r, GetComponent<Renderer>().material.color.g, GetComponent<Renderer>().material.color.b, 1);
                    GetComponent<Renderer>().material.color = newAlpha;
                }
            }
        }
        else
        {
            if (GetComponent<Renderer>().material.color.a > 0)
            {
                if (InstantTransition == false)
                {
                    Color newAlpha = new Vector4(0, 0, 0, FadeSpeed) * Time.deltaTime;
                    GetComponent<Renderer>().material.color -= newAlpha;
                }
                else
                {
                    Color newAlpha = new Vector4(GetComponent<Renderer>().material.color.r, GetComponent<Renderer>().material.color.g, GetComponent<Renderer>().material.color.b, 0);
                    GetComponent<Renderer>().material.color = newAlpha;
                }
            }
        }
	}
}
