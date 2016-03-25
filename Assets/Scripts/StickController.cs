using UnityEngine;
using System.Collections.Generic;
using System.Timers;

public class StickController : OSCBodyController
{

    public string HeightControlPath = "/be/note";
    public string RotationControlPath = "/clank/reverb";
    public float Velocity = -10f;

    // Use this for initialization
    void Start()
    {
        ControlType = "stick";
        OscPaths.Add(HeightControlPath);
        OscPaths.Add(RotationControlPath);

        gameObject.GetComponent<Rigidbody>().velocity = transform.TransformDirection(new Vector3(0, 0, (Random.value * Velocity) + Velocity));
    }
}

