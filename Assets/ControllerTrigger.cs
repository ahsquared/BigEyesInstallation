using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class ControllerTrigger : MonoBehaviour {

    private OSCBodyController controllerObj;

    void Start()
    {
        controllerObj = gameObject.transform.parent.gameObject.GetComponent<OSCBodyController>();
        
    }

    void OnTriggerEnter(Collider other)
    {
        if ((other.name == "HandLeft" || other.name == "HandRight" ||
            other.name == "WristLeft" || other.name == "WristRight" ||
            other.name == "ThumbLeft" || other.name == "ThumbRight" ||
            other.name == "HandTipLeft" || other.name == "HandTipRight") && !controllerObj.oscController.controllerActive)
        {
            Debug.Log("hit hand");
            Debug.Log(controllerObj);
            if (controllerObj.timerText == null)
            {
                controllerObj.timerText = controllerObj.GetComponentInChildren<Text>();
                controllerObj.timerText.text = "ready";
            }

            controllerObj.oscController.controllerActive = true;
            controllerObj._bodySourceView.bodySide = (other.name == "HandLeft" || other.name == "WristLeft" || other.name == "ThumbLeft" || other.name == "HandTipLeft") ? "left" : "right";
            controllerObj.GetComponent<Renderer>().material.color = new Color(1f, 0, 0, 0.5f);
            controllerObj.GetComponent<Rigidbody>().velocity = controllerObj.transform.TransformDirection(new Vector3(0, 0, 0));
            controllerObj._bodySourceView.bodyController = controllerObj.GetComponent<OSCBodyController>();
            controllerObj._bodySourceView.bodyControllerViz = controllerObj.visualizer;
            controllerObj._bodySourceView.SetOSCPaths(controllerObj.oscPaths);
            controllerObj._bodySourceView.SetControlType(controllerObj.controlType);
            controllerObj.GetComponent<Rigidbody>().isKinematic = true;
            StartCoroutine("handleTimerEvent", controllerObj.time);
        }
    }

    private IEnumerator handleTimerEvent(int time)
    {
        while (time > 0)
        {
            time--;
            controllerObj.timerText.text = (time + 1).ToString();
            yield return new WaitForSeconds(1);
        }
        controllerObj._bodySourceView.SetControlType("none");
        controllerObj.GetComponent<Renderer>().material.color = new Color(1f, 1f, 1f);
        time = controllerObj.maxTime;
        controllerObj.timerText.text = "";
        controllerObj.visualizer.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        controllerObj.visualizer.transform.localEulerAngles = new Vector3(0f, 0f, 0f);
        controllerObj.visualizer.transform.localPosition = new Vector3(0f, 0f, 0f);
        controllerObj.oscController.controllerActive = false;
        Destroy(controllerObj);
    }
}
