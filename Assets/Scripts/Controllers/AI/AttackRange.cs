using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Used by the AI to know what it can and cant attack
/// </summary>
public class AttackRange : MonoBehaviour
{
    [HideInInspector] public List<GameObject> st_EnemiesInRange;
    private BasicBrain.PlayerRelationship pr;

    // Use this for initialization
    void Awake()
    {
        st_EnemiesInRange = new List<GameObject>();
        pr = transform.root.GetComponent<BasicBrain>().DefaultRelationship;
    }

    void Update()
    {
        pr = transform.root.GetComponent<BasicBrain>().s_CurRel();
    }

    void OnTriggerEnter(Collider tango)
    {
        switch(pr)
        {
            case BasicBrain.PlayerRelationship.ENEMY:
                if (tango.transform.tag == "Friend" || tango.transform.tag == "Player")
                    st_EnemiesInRange.Add(tango.gameObject);
                break;
            case BasicBrain.PlayerRelationship.FREIND:
                if (tango.transform.tag == "Enemy")
                    st_EnemiesInRange.Add(tango.gameObject);
                 break;
            case BasicBrain.PlayerRelationship.NEUTRAL: break; //No Functionallity yet.
        }
    }
    void OnTriggerExit(Collider tango)
    {
        if (st_EnemiesInRange.Contains(tango.gameObject))
            st_EnemiesInRange.Remove(tango.gameObject);
    }
}
