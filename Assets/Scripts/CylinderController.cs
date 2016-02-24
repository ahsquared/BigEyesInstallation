using UnityEngine;
using System.Collections.Generic;
using System.Timers;

public class CylinderController : OSCBodyController
{

    public string widthControlPath = "/clank/volume";
    public string rotationControlPath = "/clank/reverb";
    public float velocity = -10f;

    // Use this for initialization
    void Start()
    {
        controlType = "cylinder";
        oscPaths.Add(widthControlPath);
        oscPaths.Add(rotationControlPath);

        gameObject.GetComponent<Rigidbody>().velocity = transform.TransformDirection(new Vector3(0, 0, (Random.value * velocity) + velocity));
    }
}

