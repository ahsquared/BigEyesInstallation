using UnityEngine;
using System.Collections;

public class AppManager : MonoBehaviour {
    private OscOut _oscOut;
	// Use this for initialization
	void Start () {
        _oscOut = GameObject.Find("OSC").GetComponent<OscOut>();
	}
	
	// Update is called once per frame
	void Update () {
	    if (Input.GetKey("escape"))
	    {
            _oscOut.Send("/be/play", 0f);
            Application.Quit();
	    }
	}

    void OnApplicationQuit()
    {
        _oscOut.Send("/be/play", 0f);
    }
}
