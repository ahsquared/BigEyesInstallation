using UnityEngine;
using System.Collections.Generic;
using System.Timers;

public class SphereController : MonoBehaviour
{

    public BodySourceView _bodySourceView;
    public string controlType = "sphere";
    public string widthControlPath = "/clank/pan";
    public string rotationControlPath = "/clank/reverb";
    public string depthControlPath = "/clank/volume";

    public int maxTime = 100;
    public int time;
    private Timer timer;

    private List<string> oscPaths = new List<string>();

    // Use this for initialization
    void Start()
    {
        gameObject.GetComponent<Rigidbody>().velocity = transform.TransformDirection(new Vector3(0, 0, -1));
        time = maxTime;
        timer = new Timer(1000);
        timer.Elapsed += (object sender, ElapsedEventArgs e) => time--;
        timer.Stop();
    }

    // Update is called once per frame
    void Update()
    {
        if (time <= 0)
        {
            _bodySourceView.SetControlType("none");
            time = maxTime;
            timer.Stop();
            timer.Dispose();
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.name == "HandLeft" && !timer.Enabled)
        {
            Debug.Log("hit left hand");
            gameObject.GetComponent<Renderer>().material.color = new Color(1f, 0, 0, 0.5f);
            gameObject.GetComponent<Rigidbody>().velocity = transform.TransformDirection(new Vector3(0, 0, 0));
            oscPaths.Clear();
            oscPaths.Add(widthControlPath);
            oscPaths.Add(rotationControlPath);
            oscPaths.Add(depthControlPath);
            _bodySourceView.SetOSCPaths(oscPaths);
            _bodySourceView.SetControlType(controlType);
            timer.Start();
        }
    }

    //void OnTriggerExit(Collider other)
    //{
    //    if (other.name == "HandLeft")
    //    {
    //        _bodySourceView.SetControlType("None");
    //    }
    //}
}

