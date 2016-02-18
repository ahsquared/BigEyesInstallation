using UnityEngine;
using System.Collections.Generic;
using System.Timers;

public class DoughController : OSCBodyController
{

    public string widthControlPath = "/clank/volume";
    public string rotationControlPath = "/clank/reverb";

    // Use this for initialization
    void Start()
    {
        controlType = "dough";
        oscPaths.Add(widthControlPath);
        oscPaths.Add(rotationControlPath);
        //gameObject.GetComponent<Rigidbody>().velocity = transform.TransformDirection(new Vector3(0, 0, -1));
    }
}

