using UnityEngine;
using System.Collections.Generic;
using System.Timers;

public class StickController : OSCBodyController
{

    public string heightControlPath = "/be/note";
    public string rotationControlPath = "/clank/reverb";
    public float velocity = -10f;

    // Use this for initialization
    void Start()
    {
        controlType = "stick";
        oscPaths.Add(heightControlPath);
        oscPaths.Add(rotationControlPath);

        gameObject.GetComponent<Rigidbody>().velocity = transform.TransformDirection(new Vector3(0, 0, (Random.value * velocity) + velocity));
    }
}

