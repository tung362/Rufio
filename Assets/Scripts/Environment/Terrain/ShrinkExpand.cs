using UnityEngine;
using System.Collections;

public class ShrinkExpand : MonoBehaviour
{
    public bool Expand = true;
    public float ChangeSpeed = 0.3f;
    public Vector3 StartingSize = Vector3.zero;

    public float ExpandSize = 2;
    public float ShrinkSize = 1;

    void Start()
    {
        transform.localScale = StartingSize;
    }

	void FixedUpdate ()
    {
        if (Expand == true)
        {
            if (transform.localScale.x < ExpandSize) transform.localScale += new Vector3(ChangeSpeed, 0, 0) * Time.deltaTime;
            if (transform.localScale.y < ExpandSize) transform.localScale += new Vector3(0, ChangeSpeed, 0) * Time.deltaTime;
            if (transform.localScale.z < ExpandSize) transform.localScale += new Vector3(0, 0, ChangeSpeed) * Time.deltaTime;
        }
        else
        {
            if (transform.localScale.x > ShrinkSize) transform.localScale -= new Vector3(ChangeSpeed, 0, 0) * Time.deltaTime;
            if (transform.localScale.y > ShrinkSize) transform.localScale -= new Vector3(0, ChangeSpeed, 0) * Time.deltaTime;
            if (transform.localScale.z > ShrinkSize) transform.localScale -= new Vector3(0, 0, ChangeSpeed) * Time.deltaTime;
        }
	}
}
