using UnityEngine;
using System.Collections.Generic;
using System.Timers;

public class SphereController : OSCBodyController
{

    public string widthControlPath = "/clank/pan";
    public string rotationControlPath = "/clank/reverb";
    public string depthControlPath = "/clank/volume";

    // Use this for initialization
    void Start()
    {
        controlType = "sphere";
        oscPaths.Add(widthControlPath);
        oscPaths.Add(rotationControlPath);
        oscPaths.Add(depthControlPath);
        //gameObject.GetComponent<Rigidbody>().velocity = transform.TransformDirection(new Vector3(0, 0, -1));
    }

}

