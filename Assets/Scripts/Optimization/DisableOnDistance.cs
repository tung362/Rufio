using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class DisableOnDistance : MonoBehaviour
{
    public float DisableDistance = 20;

	void Update ()
    {
        GameObject[] objects = SceneManager.GetActiveScene().GetRootGameObjects();
        for (int i = 0; i < objects.Length; ++i)
        {
            if (Vector3.Distance(transform.position, objects[i].transform.position) <= DisableDistance) objects[i].SetActive(true);
            else objects[i].SetActive(false);
        }
    }
}
