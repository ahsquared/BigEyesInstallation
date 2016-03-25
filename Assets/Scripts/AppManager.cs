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
	
	}

    void OnApplicationQuit()
    {
        _oscOut.Send("/be/play", 0f);
    }
}
