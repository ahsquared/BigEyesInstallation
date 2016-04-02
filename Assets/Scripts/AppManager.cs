using UnityEngine;
using System.Collections;

public class AppManager : MonoBehaviour {
    private OscOut _oscOut;
    private ClearAll _clearAll;
	// Use this for initialization
	void Start () {
        _oscOut = GameObject.Find("OSC").GetComponent<OscOut>();
	    _clearAll = GameObject.Find("Clear All").GetComponent<ClearAll>();
	}
	
	// Update is called once per frame
	void Update () {
	    if (Input.GetKey("escape"))
	    {
            _oscOut.Send("/be/play", 0f);
            _clearAll.ClearAllLoops(false);
            Application.Quit();
	    }
	}

    void OnApplicationQuit()
    {
        _oscOut.Send("/be/play", 0f);
    }
}
