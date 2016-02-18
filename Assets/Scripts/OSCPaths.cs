using UnityEngine;
using System.Collections.Generic;

public class OSCPaths : MonoBehaviour {

    private List<string> _oscPaths = new List<string>();

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
    
    public void AddOSCPath(string oscPath)
    {
        _oscPaths.Add(oscPath);
    }
}
