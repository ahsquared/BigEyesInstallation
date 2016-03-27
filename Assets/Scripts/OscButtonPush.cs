using UnityEngine;
using System.Collections;
using System.Globalization;
using BigEyes;
using UnityEngine.UI;

public class OscButtonPush : MonoBehaviour {

    private OscOut _oscOut;
    public string OscMessagePath;
    public float OffValue = 0f;
    public float OnValue = 1f;
    private bool _pushed = false;
    private Color _originalColor;
    private Color _activeColor = new Color(33f / 255f, 77f / 255f, 178f / 255f);

    private Image _image;
    private GameObject _imageActive;
    private GameObject _text;

    private InstrumentState _instrumentState;
    private float _trackNumber;

    void Start()
    {
        if (!_instrumentState) _instrumentState = GameObject.Find("InstrumentUI").GetComponent<InstrumentState>();

        _oscOut = GameObject.Find("OSC").GetComponent<OscOut>();
        _image = Helpers.GetChildGameObjectByName(gameObject, "Image").GetComponent<Image>();
        _imageActive = Helpers.GetChildGameObjectByName(gameObject, "ImageActive");
        _text = Helpers.GetChildGameObjectByName(gameObject, "Text");
        if (_imageActive) _imageActive.SetActive(false);
        if (_text) _text.SetActive(false);
        _originalColor = _image.color;
    }


    void Update()
    {

    }

    void OnTriggerEnter(Collider other)
    {

        if ((other.name == "HandLeft" || other.name == "HandRight" ||
            other.name == "WristLeft" || other.name == "WristRight" ||
            other.name == "ThumbLeft" || other.name == "ThumbRight" ||
            other.name == "HandTipLeft" || other.name == "HandTipRight") && !_pushed)
        {
            _pushed = true;
            _trackNumber = _instrumentState.TrackNumber;
            string msgPath = OscMessagePath.Replace("tx", _trackNumber.ToString(CultureInfo.CurrentCulture)).Replace("lx", 1.ToString(CultureInfo.CurrentCulture));
            SendOsc(msgPath, OnValue);
            _image.color = _activeColor;

            _imageActive.SetActive(_pushed);
            _text.SetActive(_pushed);

            Debug.Log("send osc to: " + msgPath + ", val: " + OnValue);
        }
    }
    void OnTriggerExit(Collider other)
    {

        if ((other.name == "HandLeft" || other.name == "HandRight" ||
            other.name == "WristLeft" || other.name == "WristRight" ||
            other.name == "ThumbLeft" || other.name == "ThumbRight" ||
            other.name == "HandTipLeft" || other.name == "HandTipRight") && _pushed)
        {
            _pushed = false;
            _trackNumber = _instrumentState.TrackNumber;
            string msgPath = OscMessagePath.Replace("tx", _trackNumber.ToString(CultureInfo.CurrentCulture)).Replace("lx", 1.ToString(CultureInfo.CurrentCulture));
            SendOsc(msgPath, OffValue);
            _image.color = _originalColor;

            _imageActive.SetActive(_pushed);
            _text.SetActive(_pushed);

            Debug.Log("send osc to: " + msgPath + ", val: " + OffValue);
        }
    }

    public void SendOsc(string oscPath, float value)
    {
        _oscOut.Send(oscPath, value);
    }
}
