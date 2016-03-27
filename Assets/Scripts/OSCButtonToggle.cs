using UnityEngine;
using System.Collections;
using System.Globalization;
using System.Linq;
using BigEyes;
using UnityEngine.UI;

public class OSCButtonToggle : MonoBehaviour
{

    private OscOut _oscOut;
    public string OscMessagePath;
    public float OffValue = 0f;
    public float OnValue = 1f;
    private bool _toggleState = false;
    public float DebounceTime = 0.25f;
    private bool _debouncing = false;
    private bool _debouncingWait = false;
    private float _trackNumber;
    private InstrumentState _instrumentState;

    private Color _originalColor;
    private Color _activeColor = new Color(33f / 255f, 77f / 255f, 178f / 255f);
    private Image _image;
    private GameObject _imageActive;
    private GameObject _text;

    private LightingControl[] _spotLights;

    void Start()
    {
        if (!_oscOut) _oscOut = GameObject.Find("OSC").GetComponent<OscOut>();
        if (!_instrumentState) _instrumentState = GameObject.Find("InstrumentUI").GetComponent<InstrumentState>();
        _image = Helpers.GetChildGameObjectByName(gameObject, "Image").GetComponent<Image>();
        _imageActive = Helpers.GetChildGameObjectByName(gameObject, "ImageActive");
        _text = Helpers.GetChildGameObjectByName(gameObject, "Text");
        if (_imageActive) _imageActive.SetActive(false);
        if (_text) _text.SetActive(false);
        _originalColor = _image.color;
        _spotLights = GameObject.Find("SpotLights").GetComponentsInChildren<LightingControl>();
    }


    void Update()
    {

    }

    void OnTriggerEnter(Collider other)
    {
        if ((other.name == "HandLeft" || other.name == "HandRight" ||
            other.name == "WristLeft" || other.name == "WristRight" ||
            other.name == "ThumbLeft" || other.name == "ThumbRight" ||
            other.name == "HandTipLeft" || other.name == "HandTipRight") && !_debouncing)
        {
            _toggleState = !_toggleState;
            float value = _toggleState ? OnValue : OffValue;
            _trackNumber = _instrumentState.TrackNumber;
            string msgPath = OscMessagePath.Replace("tx", _trackNumber.ToString(CultureInfo.CurrentCulture)).Replace("lx", 1.ToString(CultureInfo.CurrentCulture));
            SendOsc(msgPath, value);
            _debouncing = true;
            _debouncingWait = true;
            _image.color = _toggleState ? _activeColor : _originalColor;
            _imageActive.SetActive(_toggleState);
            _text.SetActive(_toggleState);
            StartCoroutine("Debounce");
            Debug.Log("send osc to: " + OscMessagePath + ", val: " + value);

            // Fire a burst of particles
            gameObject.GetComponent<ParticleSystem>().Emit(20);

            // Spike the lights
            foreach (LightingControl spotlight in _spotLights)
            {
                spotlight.SpikeLight();
            }

            _imageActive.SetActive(_toggleState);

        }
    }

//    void OnTriggerExit(Collider other)
//    {
//        if ((other.name == "HandLeft" || other.name == "HandRight" ||
//            other.name == "WristLeft" || other.name == "WristRight" ||
//            other.name == "ThumbLeft" || other.name == "ThumbRight" ||
//            other.name == "HandTipLeft" || other.name == "HandTipRight") && !_debouncing)
//        {
//            _imageActive.SetActive(_toggleState);
//            _text.SetActive(_toggleState);
//        }
//    }

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
