using UnityEngine;
using System.Collections;
using System.Globalization;
using BigEyes;
using UnityEngine.UI;

public class SetTrackNumber : MonoBehaviour
{
    private OSCController _oscController;

    public int TrackNumber;
    public bool TrackActive;
    public bool IsChromatic;

    private InstrumentState _instrumentState;
    private SetTrackNumber[] _instruments;
    private Color _originalColor;
    private Color _activeColor = new Color(33f / 255f, 77f / 255f, 178f / 255f);

    private Image _image;
    private GameObject _imageInactive;
    private GameObject _imageActive;

    private bool _toggleState = false;
    public float DebounceTime = 0.25f;
    private bool _debouncing = false;
    private bool _debouncingWait = false;

    private OSCButtonToggle _recordButton;

    private PianoGrid _pianoGrid;

    // Use this for initialization
    void Start () {
        if (!_instrumentState) _instrumentState = GameObject.Find("InstrumentUI").GetComponent<InstrumentState>();
        if (!_recordButton) _recordButton = GameObject.Find("Record").GetComponent<OSCButtonToggle>();
        if (!_pianoGrid) _pianoGrid = GameObject.Find("PianoGrid").GetComponent<PianoGrid>();
        if (!_oscController) _oscController = GameObject.Find("OSC").GetComponent<OSCController>();
        _instruments = GameObject.Find("Instruments").GetComponentsInChildren<SetTrackNumber>();
        _image = Helpers.GetChildGameObjectByName(gameObject, "Image").GetComponent<Image>();
        _imageInactive = Helpers.GetChildGameObjectByName(gameObject, "Image");
        _imageActive = Helpers.GetChildGameObjectByName(gameObject, "ImageActive");
        _toggleState = TrackActive;
        _originalColor = _image.color;
        SetTrackState(_toggleState);
    }

    // Update is called once per frame
    void Update () {
	
	}
    void OnTriggerEnter(Collider other)
    {

        if ((other.name == "HandLeft" || other.name == "HandRight") && !_debouncing)
        {
            _toggleState = true;
            _debouncing = true;
            _debouncingWait = true;
            StartCoroutine("Debounce");
            // _instruments.TrackActive = false;
            foreach (SetTrackNumber instrument in _instruments)
            {
                instrument.SetTrackState(false);
            }
            SetTrackState(true);

            // if track number changed stop recording reset record button
            if (_instrumentState.TrackNumber != TrackNumber)
            {
                _recordButton.SetButtonState(false);
                foreach (SetTrackNumber instrument in _instruments)
                {
                    string trackNumber = instrument.TrackNumber.ToString(CultureInfo.CurrentCulture);
                    string msgPath = _recordButton.OscMessagePath.Replace("tx", trackNumber).Replace("lx", 1.ToString(CultureInfo.CurrentCulture));
                    _recordButton.SendOsc(msgPath, 0f);
                    // send Stop All Notes MIDI CC (Panic button) to prevent stuck notes
                    instrument.SendPanic();
                }

                
            }

            if (IsChromatic)
            {
                _pianoGrid.UpdateNotes(true);
            }
            else
            {
                _pianoGrid.UpdateNotes(false);
            }

            _instrumentState.TrackNumber = TrackNumber;
        }
    }

    public void SendPanic()
    {
        _oscController.SendOSC("/be/track/" + TrackNumber + "/panic", 1);
        _oscController.SendOSC("/be/track/" + TrackNumber + "/panic", 0);
    }

    public void SetTrackState(bool state)
    {
        _image.color = state ? _activeColor : _originalColor;
        _imageInactive.SetActive(!state);
        _imageActive.SetActive(state);
        TrackActive = state;
    }

    private IEnumerator Debounce()
    {
        while (_debouncingWait)
        {
            _debouncingWait = false;
            yield return new WaitForSeconds(DebounceTime);
        }
        _debouncing = false;
        _toggleState = false;
    }


}
