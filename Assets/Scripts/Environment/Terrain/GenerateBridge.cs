using UnityEngine;
using System.Collections;

public class GenerateBridge : MonoBehaviour
{
    public GameObject[] ObjectsToSpawn;
    public GameObject GenerationEndPoint;
    public float Offset = 1;
    private float CurrentDistanceToEnd = int.MaxValue;
    private GameObject PreviousSpawnedObject;

    void Start ()
    {
        transform.LookAt(GenerationEndPoint.transform);
        PreviousSpawnedObject = gameObject;
        while (true)
        {
            GameObject platform = (GameObject)Instantiate(RandomObjectToSpawn(), PreviousSpawnedObject.transform.position + (PreviousSpawnedObject.transform.forward * Offset), PreviousSpawnedObject.transform.rotation);
            float distance = Vector3.Distance(platform.transform.position, GenerationEndPoint.transform.position);
            if (CurrentDistanceToEnd > distance) CurrentDistanceToEnd = distance;
            else break;

            PreviousSpawnedObject = platform;
        }
    }

    GameObject RandomObjectToSpawn()
    {
        return ObjectsToSpawn[Random.Range(0, ObjectsToSpawn.Length)];
    }
}
