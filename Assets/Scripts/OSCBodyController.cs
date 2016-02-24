using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

public class OSCBodyController : MonoBehaviour {

    public BigEyes.OSCController oscController;
    public BodySourceView _bodySourceView;
    public string controlType;
    public List<string> oscPaths = new List<string>();

    public GameObject visualizer;

    public int maxTime = 100;
    public int time;

    public Text timerText;

    // Use this for initialization
    void Awake()
    {
        oscController = GameObject.Find("OSC").GetComponent<BigEyes.OSCController>();
        time = maxTime;
        
    } 
    

    // Update is called once per frame
    //void Update()
    //{
    //    if (time <= 0)
    //    {
    //        _bodySourceView.SetControlType("none");
    //        gameObject.GetComponent<Renderer>().material.color = new Color(1f, 1f, 1f);
    //        time = maxTime;
    //        oscController.controllerActive = false;
    //        //timer.Dispose();
    //        //Destroy(gameObject);
    //    }
    //}

    //void OnTriggerEnter(Collider other)
    //{
    //    if ((other.name == "HandLeft" || other.name == "HandRight") && !oscController.controllerActive)
    //    {
    //        Debug.Log("hit hand");
    //        if (timerText == null)
    //        {
    //            timerText = gameObject.GetComponentInChildren<Text>();
    //            timerText.text = "ready";
    //        }

    //        oscController.controllerActive = true;
    //        gameObject.GetComponent<Renderer>().material.color = new Color(1f, 0, 0, 0.5f);
    //        gameObject.GetComponent<Rigidbody>().velocity = transform.TransformDirection(new Vector3(0, 0, 0));
    //        _bodySourceView.bodyController = gameObject.GetComponent<OSCBodyController>();
    //        _bodySourceView.bodyControllerViz = visualizer;
    //        _bodySourceView.SetOSCPaths(oscPaths);
    //        _bodySourceView.SetControlType(controlType);
    //        StartCoroutine("handleTimerEvent", time);
    //    }
    //}

    //private IEnumerator handleTimerEvent(int time)
    //{
    //    while (time > 0)
    //    {
    //        time--;
    //        timerText.text = (time + 1).ToString();
    //        yield return new WaitForSeconds(1);
    //    }
    //    _bodySourceView.SetControlType("none");
    //    gameObject.GetComponent<Renderer>().material.color = new Color(1f, 1f, 1f);
    //    time = maxTime;
    //    timerText.text = "";
    //    visualizer.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
    //    visualizer.transform.localEulerAngles = new Vector3(0f, 0f, 0f);
    //    visualizer.transform.localPosition = new Vector3(0f, 0f, 0f);
    //    oscController.controllerActive = false;

    //}

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.name == "faceted-ground")
        {
            gameObject.GetComponent<Rigidbody>().velocity = Vector3.Reflect(other.relativeVelocity * -1, other.contacts[0].normal);
        }
    }
}
