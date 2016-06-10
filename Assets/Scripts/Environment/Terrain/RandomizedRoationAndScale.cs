using UnityEngine;
using System.Collections;

public class RandomizedRoationAndScale : MonoBehaviour
{
    public float RandomSizeRangeMin = 1;
    public float RandomSizeRangeMax = 1;
    public Vector3 RandomRotationRangeMin = Vector3.zero;
    public Vector3 RandomRotationRangeMax = Vector3.zero;

    void Start ()
    {
        transform.localScale = transform.localScale * Random.Range(RandomSizeRangeMin, RandomSizeRangeMax);
        transform.rotation = Quaternion.Euler(Random.Range(RandomRotationRangeMin.x, RandomRotationRangeMax.x), Random.Range(RandomRotationRangeMin.y, RandomRotationRangeMax.y), Random.Range(RandomRotationRangeMin.z, RandomRotationRangeMax.z));
    }
}
