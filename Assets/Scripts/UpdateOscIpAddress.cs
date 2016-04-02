using UnityEngine;
using System.Collections;
using BigEyes;
using UnityEngine.UI;

public class UpdateOscIpAddress : MonoBehaviour {
    private OscOut _oscOut;
    private OSCController _oscController;
    private string _ipAddress;

    // Use this for initialization
    void Start () {
        _oscOut = GameObject.Find("OSC").GetComponent<OscOut>();
        _oscController = GameObject.Find("OSC").GetComponent<OSCController>();
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    public void UpdateIpAddress()
    {
        _ipAddress = gameObject.GetComponent<InputField>().text;
        Debug.Log("ip address: " + _ipAddress);
        _oscController.IpAddress = _ipAddress;
        _oscOut.Close();
        _oscOut.Open(_oscOut.port, _ipAddress);
    }
}
