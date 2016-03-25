using UnityEngine;
using System.Collections.Generic;
using System.Timers;

public class SphereController : OSCBodyController
{

    public string WidthControlPath = "/clank/pan";
    public string RotationControlPath = "/clank/reverb";
    public string DepthControlPath = "/clank/volume";
    public float Velocity = -10f;

    // Use this for initialization
    void Start()
    {
        ControlType = "sphere";
        OscPaths.Add(WidthControlPath);
        OscPaths.Add(RotationControlPath);
        OscPaths.Add(DepthControlPath);

        gameObject.GetComponent<Rigidbody>().velocity = transform.TransformDirection(new Vector3(0, 0, (Random.value * Velocity) + Velocity));
    }

}

