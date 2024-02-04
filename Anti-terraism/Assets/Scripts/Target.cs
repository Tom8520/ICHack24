using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{
    public void Kill()
    {
        transform.position = new Vector3(Random.Range(-8, 8), Random.Range(2, 9), 11);
//        transform.position = new Vector3(0, Random.Range(5.0f, 6.0f), 11);
    }
}
