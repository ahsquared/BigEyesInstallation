using UnityEngine;
using System.Collections;
using System.Globalization;
using BigEyes;
using UnityEngine.UI;

public class ClickTrackToggle : MonoBehaviour {


    private OscOut _oscOut;
    public string OscMessagePath;
    public float OffValue = 10f;
    private bool _toggleState = false;
    public float DebounceTime = 0.25f;
    private bool _debouncing = false;
    private bool _debouncingWait = false;
    private InstrumentState _instrumentState;

    private Color _originalColor;
    private Color _activeColor = new Color(33f / 255f, 77f / 255f, 178f / 255f);
    private Image _image;
    private GameObject _imageActive;

    void Start()
    {
        if (!_oscOut) _oscOut = GameObject.Find("OSC").GetComponent<OscOut>();
        if (!_instrumentState) _instrumentState = GameObject.Find("InstrumentUI").GetComponent<InstrumentState>();
        _image = Helpers.GetChildGameObjectByName(gameObject, "Image").GetComponent<Image>();
        _imageActive = Helpers.GetChildGameObjectByName(gameObject, "ImageActive");
        if (_imageActive) _imageActive.SetActive(false);
        _originalColor = _image.color;

        SendOsc(OscMessagePath, OffValue);
        SetButtonState(_toggleState);
    }


    void Update()
    {

    }

    void OnTriggerEnter(Collider other)
    {
        if ((other.name == "HandLeft" || other.name == "HandRight") && !_debouncing)
        {
            _toggleState = !_toggleState;
            float value = _toggleState ? _instrumentState.SetNumber - 1 : OffValue;
            SendOsc(OscMessagePath, value);
            _debouncing = true;
            _debouncingWait = true;
            SetButtonState(_toggleState);
            StartCoroutine("Debounce");

            // Fire a burst of particles
            gameObject.GetComponent<ParticleSystem>().Emit(20);

            _imageActive.SetActive(_toggleState);
        }
    }

    public void SetButtonState(bool state)
    {
        _image.color = state ? _activeColor : _originalColor;
        _imageActive.SetActive(state);
    }

    public void SendOsc(string oscPath, float value)
    {
        _oscOut.Send(oscPath, value);
    }

    private IEnumerator Debounce()
    {
        while (_debouncingWait)
        {
            _debouncingWait = false;
            yield return new WaitForSeconds(DebounceTime);
        }
        _debouncing = false;
    }
}
