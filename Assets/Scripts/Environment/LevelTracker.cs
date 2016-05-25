using UnityEngine;
using System.Collections;

/*
--------------------------------
Author: Tung Nguyen

Purpose: Like the GlobalVars script in which it stores data for the gameobjects to use except
storing it here only save the data locally to the current loaded scene and will reset on load
of new scene.

Script communicate with: GlobalVars(Spawn the object with the script attached, not directly)

Used by: Almost everything

Last edited: Tung Nguyen
--------------------------------
*/

public class LevelTracker : MonoBehaviour
{
    //Cam Shake
    public bool StartCamShake = false;
    [HideInInspector]
    public float CamShakeTimer = 0;
    public float CamShakeDuration = 4;
    public Vector2 CamShakePower = new Vector2(-2, 2);

    //Slow Mo
    public bool StartSlowMo = false;
    [HideInInspector]
    public float SlowMoTimer = 0;
    public float SlowMoDuration = 4;
    public float SlowMoPower = 2;

    public GameObject GlobalPrefab;

    void Awake()
    {
        GameObject globalObject = GameObject.Find("Global");

        //If there is no global object, make one
        if (globalObject == null)
        {
            GameObject aGlobal = (GameObject)Instantiate(GlobalPrefab, new Vector3(0, 0, 0), Quaternion.identity); //Spawn decoration
            aGlobal.name = "Global";
        }
    }

    void Update()
    {
        //Cam Shake
        if (StartCamShake == true)
        {
            CamShakeTimer += Time.deltaTime;
            if (CamShakeTimer >= CamShakeDuration)
            {
                StartCamShake = false;
                CamShakeTimer = 0;
            }
        }

        //Slow Mo
        if (StartSlowMo == true)
        {
            SlowMoTimer += Time.deltaTime;
            if (SlowMoTimer >= SlowMoDuration)
            {
                StartSlowMo = false;
                SlowMoTimer = 0;
            }
        }
    }
}
