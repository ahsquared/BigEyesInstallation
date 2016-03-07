using UnityEngine;
using UnityEngine.UI;

using System.Collections;

public class ControllerSpawner : MonoBehaviour
{

    public GameObject[] controllers;
    public Text timerText;
    private int time = -1;
    public int maxTime = 10;
    private int counter = 0;
    private GameObject _currentController;
    private BigEyes.OSCController oscController;

    // Use this for initialization
    void Start()
    {
        oscController = GameObject.Find("OSC").GetComponent<BigEyes.OSCController>();
        spawnController();
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log("time: " + time);
        //if (time <= 0) {
        //    time = maxTime;
        //    StartCoroutine("handleTimerEvent", time);
        //}
    }

    private IEnumerator handleTimerEvent(int time)
    {
        while (time >= 0)
        {
            if (!oscController.controllerActive)
            {
                time--;
                timerText.text = (time + 1).ToString() + "s";
            }
            yield return new WaitForSeconds(1);
        }
        spawnController();
    }

    void spawnController()
    {
        if (_currentController != null)
        {
            Destroy(_currentController);
            oscController.controllerActive = false;
        }
        time = maxTime;
        StartCoroutine("handleTimerEvent", time);

        Vector3 initPos = new Vector3(Random.Range(-10f, 10f), 10f, 70f);
        int controllerIndex = 2;// Random.Range(0, controllers.Length);
        _currentController = (GameObject)Instantiate(controllers[controllerIndex], initPos, Quaternion.identity);
        counter++;
        _currentController.name = _currentController.name + "-" + counter.ToString();
    }
}
