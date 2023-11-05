using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mozgas : MonoBehaviour
{
    public GameObject ezmozog;
    public GameObject ide;

    public float speed;

    // Update is called once per frame
    void Update()
    {
        ezmozog.transform.position = Vector3.MoveTowards(ezmozog.transform.position, ide.transform.position, speed);
    }
}
