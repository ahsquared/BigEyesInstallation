using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class ControllerTrigger : MonoBehaviour {

    private OSCBodyController _controllerObj;

    void Start()
    {
        _controllerObj = gameObject.transform.parent.gameObject.GetComponent<OSCBodyController>();
        
    }

    void OnTriggerEnter(Collider other)
    {
        if ((other.name == "HandLeft" || other.name == "HandRight" ||
            other.name == "WristLeft" || other.name == "WristRight" ||
            other.name == "ThumbLeft" || other.name == "ThumbRight" ||
            other.name == "HandTipLeft" || other.name == "HandTipRight") && !_controllerObj.OscController.ControllerActive)
        {
            Debug.Log("hit hand");
            Debug.Log(_controllerObj);
            if (_controllerObj.TimerText == null)
            {
                _controllerObj.TimerText = _controllerObj.GetComponentInChildren<Text>();
                _controllerObj.TimerText.text = "ready";
            }

            _controllerObj.OscController.ControllerActive = true;
            _controllerObj.BodySourceView.BodySide = (other.name == "HandLeft" || other.name == "WristLeft" || other.name == "ThumbLeft" || other.name == "HandTipLeft") ? "left" : "right";
            _controllerObj.GetComponent<Renderer>().material.color = new Color(1f, 0, 0, 0.5f);
            _controllerObj.GetComponent<Rigidbody>().velocity = _controllerObj.transform.TransformDirection(new Vector3(0, 0, 0));
            _controllerObj.BodySourceView.BodyController = _controllerObj;
            _controllerObj.BodySourceView.BodyControllerViz = _controllerObj.Visualizer;
            _controllerObj.BodySourceView.SetOscPaths(_controllerObj.OscPaths);
            _controllerObj.BodySourceView.SetControlType(_controllerObj.ControlType);
            _controllerObj.GetComponent<Rigidbody>().isKinematic = true;
            StartCoroutine("handleTimerEvent", _controllerObj.Time);
        }
    }

    private IEnumerator HandleTimerEvent(int time)
    {
        while (time > 0)
        {
            time--;
            _controllerObj.TimerText.text = (time + 1).ToString();
            yield return new WaitForSeconds(1);
        }
        _controllerObj.BodySourceView.SetControlType("none");
        _controllerObj.GetComponent<Renderer>().material.color = new Color(143f/255f, 58f/255f, 58f/255f);
        time = _controllerObj.MaxTime;
        _controllerObj.TimerText.text = "";
        _controllerObj.Visualizer.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        _controllerObj.Visualizer.transform.localEulerAngles = new Vector3(0f, 0f, 0f);
        _controllerObj.Visualizer.transform.localPosition = new Vector3(0f, 0f, 0f);
        _controllerObj.OscController.ControllerActive = false;
        //Destroy(controllerObj);
    }
}
