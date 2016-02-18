using UnityEngine;
using System.Collections.Generic;
using System.Timers;

public class OSCBodyController : MonoBehaviour {

    public BigEyes.OSCController oscController;
    public BodySourceView _bodySourceView;
    public string controlType;
    public List<string> oscPaths = new List<string>();

    public int maxTime = 100;
    public int time;
    public Timer timer;

    // Use this for initialization
    void Awake()
    {
        oscController = GameObject.Find("OSC").GetComponent<BigEyes.OSCController>();
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
            gameObject.GetComponent<Renderer>().material.color = new Color(1f, 1f, 1f);
            time = maxTime;
            timer.Stop();
            oscController.controllerActive = false;
            //timer.Dispose();
            //Destroy(gameObject);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if ((other.name == "HandLeft" || other.name == "HandRight") && !timer.Enabled && !oscController.controllerActive)
        {
            Debug.Log("hit hand");
            oscController.controllerActive = true;
            gameObject.GetComponent<Renderer>().material.color = new Color(1f, 0, 0, 0.5f);
            gameObject.GetComponent<Rigidbody>().velocity = transform.TransformDirection(new Vector3(0, 0, 0));
            _bodySourceView.SetOSCPaths(oscPaths);
            _bodySourceView.SetControlType(controlType);
            timer.Start();
        }
    }
}
