using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour
{
    //For multiplayer to see which camera this belongs to
    public int PlayerID = 0;

    //Camera Follow
    public float FollowSmoothness = 0.3f;
    public Vector2 FollowOffsetRange = new Vector2(-2, 2);
    private GameObject target;
    private Vector3 Velo = Vector3.zero;
    private Vector3 CurrentTargetPosition = new Vector3(0, 0, 0);
    private Vector3 CamTargetDifference;

    //Camera Rotation Follow
    public float RotationSmoothness = 5;

    //Camera Shake
    //Used to prevent the camera position being spammed set update
    public bool RunOnce = true;

    private LevelTracker Tracker;

    void Start()
    {
        Tracker = GameObject.Find("LevelTracker").GetComponent<LevelTracker>();
        CurrentTargetPosition = transform.position;
    }

    void FixedUpdate()
    {
        //Find a target
        if (target == null)
        {
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            foreach (GameObject player in players)
            {
                if (player.GetComponent<PlayerID>() != null && player.GetComponent<PlayerID>().ID == PlayerID)
                {
                    target = player;
                    CamTargetDifference = transform.position - target.transform.position;
                }
            }
        }
        else
        {
            //Camera Follow
            transform.position = Vector3.SmoothDamp(transform.position, target.transform.position + CamTargetDifference, ref Velo, FollowSmoothness);

            //Camera Rotation Follow

            //Camera Shake
            if (Tracker.StartCamShake == true)
            {
                if (RunOnce == true) RunOnce = false;
                transform.position = new Vector3(CurrentTargetPosition.x, CurrentTargetPosition.y, CurrentTargetPosition.z);
                transform.position = new Vector3(transform.position.x + Random.Range(Tracker.CamShakePower.x, Tracker.CamShakePower.y), transform.position.y + Random.Range(Tracker.CamShakePower.x, Tracker.CamShakePower.y), transform.position.z + Random.Range(Tracker.CamShakePower.x, Tracker.CamShakePower.y));
            }
            else
            {
                if (RunOnce == false)
                {
                    transform.position = new Vector3(CurrentTargetPosition.x, CurrentTargetPosition.y, CurrentTargetPosition.z);
                    RunOnce = true;
                }
            }
        }
    }
}
