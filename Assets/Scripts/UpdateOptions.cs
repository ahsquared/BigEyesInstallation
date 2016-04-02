using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UpdateOptions : MonoBehaviour
{

    private bool _toggleState;
    public GameObject OptionsCanvas;

    private Text _inIpAddress;

    private InputField _oscOutIPInput;
    private OscOut _oscOut;

	// Use this for initialization
	void Start () {
	    if (!OptionsCanvas) OptionsCanvas = Helpers.GetChildGameObjectByName(gameObject, "OptionsCanvas");
	    if (!_oscOut) _oscOut = GameObject.Find("OSC").GetComponent<OscOut>();
        OptionsCanvas.SetActive(_toggleState);

    }

    // Update is called once per frame
    void Update ()
	{
        if (Input.GetKeyUp(KeyCode.Space) )
        {
            _toggleState = !_toggleState;
            OptionsCanvas.SetActive(_toggleState);

            if (_toggleState)
            {

                // OSC In IP Address
                if (!_inIpAddress) _inIpAddress = GameObject.Find("OSC In IP Address").GetComponent<Text>();
                _inIpAddress.text = OscIn.ipAddress;

                // OSC Out IP Address
                if (!_oscOutIPInput) _oscOutIPInput = GameObject.Find("OSC Out IP Input").GetComponent<InputField>();
                _oscOutIPInput.text = _oscOut.ipAddress;
            }
        }
    }
}
