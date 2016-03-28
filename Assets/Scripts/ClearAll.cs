using UnityEngine;
using System.Collections;
using System.Globalization;

public class ClearAll : MonoBehaviour {

    private SetTrackNumber[] _instruments;
    private OscButtonPush _button;
    private bool _pushed = false;
    private string _colliderName;

    // Use this for initialization
    void Start () {
        _instruments = GameObject.Find("Instruments").GetComponentsInChildren<SetTrackNumber>();
        _button = gameObject.GetComponent<OscButtonPush>();
    }
	
	// Update is called once per frame
	void Update () {
	
	}
    void OnTriggerEnter(Collider other)
    {

        if ((other.name == "HandLeft" || other.name == "HandRight") && !_pushed)
        {
            _pushed = true;
            _colliderName = other.name;
            ClearAllLoops();
        }
    }
    void OnTriggerExit(Collider other)
    {

        if ((other.name == _colliderName) && _pushed)
        {
            _pushed = false;
        }
    }
    public void ClearAllLoops()
    {
        foreach (SetTrackNumber instrument in _instruments)
        {
            string msgPath = _button.OscMessagePath.Replace("tx", instrument.TrackNumber.ToString(CultureInfo.CurrentCulture)).Replace("lx", 1.ToString(CultureInfo.CurrentCulture));
            _button.SendOsc(msgPath, 1f);
            Debug.Log("Send OSC to: " + msgPath + " with value " + 1);
        }
    }
}
