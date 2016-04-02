using UnityEngine;
using System.Collections;
using System.Globalization;
using UnityEngine.SceneManagement;

public class ClearAll : MonoBehaviour {

    private SetTrackNumber[] _instruments;
    private OscButtonPush _button;
    private bool _pushed = false;
    private string _colliderName;
    private ButtonNote[] _buttonNotes;

    // Use this for initialization
    void Start () {
        _instruments = GameObject.Find("Instruments").GetComponentsInChildren<SetTrackNumber>();
        _button = gameObject.GetComponent<OscButtonPush>();
        _buttonNotes = GameObject.Find("PianoGrid").GetComponentsInChildren<ButtonNote>();
        ClearAllLoops(false);
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
            ClearAllLoops(true);
        }
    }
    void OnTriggerExit(Collider other)
    {

        if ((other.name == _colliderName) && _pushed)
        {
            _pushed = false;
        }
    }
    public void ClearAllLoops(bool reset)
    {
        foreach (SetTrackNumber instrument in _instruments)
        {
            instrument.ClearLoop();
        }

        // reset all button highlights
        foreach (var button in _buttonNotes)
        {
            button.GetComponent<HighlightButton>().UnHighlightButton();
        }

//        if (reset)
//        {
//            SceneManager.LoadScene(0);
//        }
    }
}
