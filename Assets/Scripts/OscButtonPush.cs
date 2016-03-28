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
    private string _colliderName;

    private Color _originalColor;
    private Color _activeColor = new Color(33f / 255f, 77f / 255f, 178f / 255f);

    private Image _image;
    private GameObject _imageActive;

    private InstrumentState _instrumentState;
    private float _trackNumber;

    void Start()
    {
        if (!_instrumentState) _instrumentState = GameObject.Find("InstrumentUI").GetComponent<InstrumentState>();

        _oscOut = GameObject.Find("OSC").GetComponent<OscOut>();
        _image = Helpers.GetChildGameObjectByName(gameObject, "Image").GetComponent<Image>();
        _imageActive = Helpers.GetChildGameObjectByName(gameObject, "ImageActive");
        if (_imageActive) _imageActive.SetActive(false);
        _originalColor = _image.color;
    }


    void Update()
    {

    }

    void OnTriggerEnter(Collider other)
    {

        if ((other.name == "HandLeft" || other.name == "HandRight") && !_pushed)
        {
            _pushed = true;
            _colliderName = other.name;
            _trackNumber = _instrumentState.TrackNumber;
            string msgPath = OscMessagePath.Replace("tx", _trackNumber.ToString(CultureInfo.CurrentCulture)).Replace("lx", 1.ToString(CultureInfo.CurrentCulture));
            SendOsc(msgPath, OnValue);
            _image.color = _activeColor;

            _imageActive.SetActive(_pushed);

            // Fire a burst of particles
            gameObject.GetComponent<ParticleSystem>().Emit(20);

            //Debug.Log("send osc to: " + msgPath + ", val: " + OnValue);
        }
    }
    void OnTriggerExit(Collider other)
    {

        if ((other.name == _colliderName) && _pushed)
        {
            _pushed = false;
            _trackNumber = _instrumentState.TrackNumber;
            string msgPath = OscMessagePath.Replace("tx", _trackNumber.ToString(CultureInfo.CurrentCulture)).Replace("lx", 1.ToString(CultureInfo.CurrentCulture));
            SendOsc(msgPath, OffValue);
            _image.color = _originalColor;

            _imageActive.SetActive(_pushed);

            //Debug.Log("send osc to: " + msgPath + ", val: " + OffValue);
        }
    }

    public void SendOsc(string oscPath, float value)
    {
        _oscOut.Send(oscPath, value);
    }
}
