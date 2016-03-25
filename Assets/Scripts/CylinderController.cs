using UnityEngine;
using System.Collections.Generic;
using System.Timers;

public class CylinderController : OSCBodyController
{

    public string WidthControlPath = "/clank/volume";
    public string RotationControlPath = "/clank/reverb";
    public float Velocity = -10f;

    // Use this for initialization
    void Start()
    {
        ControlType = "cylinder";
        OscPaths.Add(WidthControlPath);
        OscPaths.Add(RotationControlPath);

        gameObject.GetComponent<Rigidbody>().velocity = transform.TransformDirection(new Vector3(0, 0, (Random.value * Velocity) + Velocity));
    }
}

