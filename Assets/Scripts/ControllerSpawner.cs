using UnityEngine;
using UnityEngine.UI;

using System.Collections;

public class ControllerSpawner : MonoBehaviour
{

    public GameObject[] Controllers;
    public Text TimerText;
    private int _time = -1;
    public int MaxTime = 10;
    private int _counter = 0;
    private GameObject _currentController;
    private BigEyes.OSCController _oscController;

    // Use this for initialization
    void Start()
    {
        _oscController = GameObject.Find("OSC").GetComponent<BigEyes.OSCController>();
        SpawnController();
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

    private IEnumerator HandleTimerEvent(int time)
    {
        while (time >= 0)
        {
            if (!_oscController.ControllerActive)
            {
                time--;
                TimerText.text = (time + 1).ToString() + "s";
            }
            yield return new WaitForSeconds(1);
        }
        SpawnController();
    }

    void SpawnController()
    {
        if (_currentController != null)
        {
            Destroy(_currentController);
            _oscController.ControllerActive = false;
        }
        _time = MaxTime;
        StartCoroutine("handleTimerEvent", _time);

        Vector3 initPos = new Vector3(Random.Range(-10f, 10f), 10f, 70f);
        int controllerIndex = Random.Range(0, Controllers.Length);
        _currentController = (GameObject)Instantiate(Controllers[controllerIndex], initPos, Quaternion.identity);
        _counter++;
        _currentController.name = _currentController.name + "-" + _counter.ToString();
    }
}
