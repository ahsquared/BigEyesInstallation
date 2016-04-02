using UnityEngine;
using System.Collections;
using System.Globalization;
using BigEyes;
using UnityEngine.UI;

public class SetSoundSetNumber : MonoBehaviour {

    private OSCController _oscController;

    public int SetNumber;
    public bool SetActive;

    public InstrumentState InstrumentState;
    private SetSoundSetNumber[] _soundSets;
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

    void Awake()
    {
        if (!InstrumentState) InstrumentState = GameObject.Find("InstrumentUI").GetComponent<InstrumentState>();
        if (!_oscController) _oscController = GameObject.Find("OSC").GetComponent<OSCController>();
    }

    // Use this for initialization
    void Start()
    {
        if (!_recordButton) _recordButton = GameObject.Find("Record").GetComponent<OSCButtonToggle>();
        if (!_pianoGrid) _pianoGrid = GameObject.Find("PianoGrid").GetComponent<PianoGrid>();
        _soundSets = GameObject.Find("Sets").GetComponentsInChildren<SetSoundSetNumber>();
        _image = Helpers.GetChildGameObjectByName(gameObject, "Image").GetComponent<Image>();
        _imageInactive = Helpers.GetChildGameObjectByName(gameObject, "Image");
        _imageActive = Helpers.GetChildGameObjectByName(gameObject, "ImageActive");
        _toggleState = SetActive;
        _originalColor = _image.color;
        SetSoundSetState(_toggleState);
    }

    // Update is called once per frame
    void Update()
    {

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
            foreach (SetSoundSetNumber instrument in _soundSets)
            {
                instrument.SetSoundSetState(false);
            }
            SetSoundSetState(true);

            // if track number changed stop recording reset record button
            if (InstrumentState.SetNumber != SetNumber)
            {
                _recordButton.SetButtonState(false);
                foreach (SetSoundSetNumber set in _soundSets)
                {
                    string trackNumber = (set.SetNumber + ((InstrumentState.SetNumber - 1) * 4)).ToString(CultureInfo.CurrentCulture);
                    string msgPath = _recordButton.OscMessagePath.Replace("tx", trackNumber).Replace("lx", 1.ToString(CultureInfo.CurrentCulture));
                    _recordButton.SendOsc(msgPath, 0f);
                    // send Stop All Notes MIDI CC (Panic button) to prevent stuck notes
                    set.SendPanic(trackNumber);
                }


            }

            InstrumentState.SetNumber = SetNumber;

            // Fire a burst of particles
            gameObject.GetComponent<ParticleSystem>().Emit(20);
        }
    }

    public void SendPanic(string trackNumber)
    {
        _oscController.SendOSC("/be/track/" + trackNumber + "/panic", 1);
        _oscController.SendOSC("/be/track/" + trackNumber + "/panic", 0);
    }

    public void ClearLoop()
    {
        _oscController.SendOSC("/be/track/" + (SetNumber + ((InstrumentState.SetNumber - 1) * 4)) + "/loop/1/clear", 1);
        _oscController.SendOSC("/be/track/" + (SetNumber + ((InstrumentState.SetNumber - 1) * 4)) + "/loop/1/clear", 0);
        InstrumentState.ResetRecordCounter();
    }

    public void SetSoundSetState(bool state)
    {
        _image.color = state ? _activeColor : _originalColor;
        _imageInactive.SetActive(!state);
        _imageActive.SetActive(state);
        SetActive = state;
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
