using UnityEngine;
using System.Collections;
using System.Globalization;
using BigEyes;

public class ButtonNote : MonoBehaviour {

    private OscOut _oscOut;
    private bool _playing = false;
    private string _colliderName;
    private Bounds _bounds;

    public string OscMessagePath = "/be/track/tx/notes/vel";
    [Range (56, 95)]
    public int Note = 56;
    public int NoteVelocity = 80;
    //private float velocityF;
    public string[] OscControllerMessages;
    [Range(0f, 1f)]
    public float MaxVal = 0.8f;

    private InstrumentState _instrumentState;
    private float _trackNumber;


    void Start()
    {
        if (!_oscOut) _oscOut = GameObject.Find("OSC").GetComponent<OscOut>();
        if (!_instrumentState) _instrumentState = GameObject.Find("InstrumentUI").GetComponent<InstrumentState>();
        _bounds = gameObject.GetComponent<Renderer>().bounds;
    }


    void Update()
    {
        //Debug.Log("vel: " + gameObject.GetComponent<Rigidbody>().velocity.sqrMagnitude);
    }

    void OnTriggerEnter(Collider other)
    {

        if ((other.name == "HandLeft" || other.name == "HandRight") && !_playing)
        { 
            //Debug.Log("piano note played: " + gameObject.name);
            _playing = true;
            _colliderName = other.name;
            //velocityF = Mathf.Clamp(Mathf.Abs(other.GetComponent<Rigidbody>().velocity.magnitude) * 100, 0, 128f);
            //Debug.Log("velocity: " + other.GetComponent<Rigidbody>().velocity.magnitude);

            //SendOscControllerValue("/be/track/5/mod", other.GetComponent<Rigidbody>().velocity.magnitude > 10 ? 127 : 0);

           SendAllControllerMessages(MaxVal);
            _trackNumber = _instrumentState.TrackNumber;
            string msgPath = OscMessagePath.Replace("tx", _trackNumber.ToString(CultureInfo.CurrentCulture));
            SendNote(msgPath, Note, NoteVelocity);

        }
    }
    void OnTriggerStay(Collider other)
    {
        if ((other.name == _colliderName) && _playing)
        {
            float maxHeight = _bounds.max.y;
            float minHeight = _bounds.min.y;
            float handY = other.transform.position.y;
            float value = ((handY + maxHeight) / (maxHeight - minHeight)) * MaxVal;
            SendAllControllerMessages(value);
        }
    }
    void OnTriggerExit(Collider other)
    {

        if ((other.name == _colliderName) && _playing)
        {
            _playing = false;
            _trackNumber = _instrumentState.TrackNumber;
            string msgPath = OscMessagePath.Replace("tx", _trackNumber.ToString(CultureInfo.CurrentCulture));
            SendNote(msgPath, Note, 0);
        }
    }

    private void SendAllControllerMessages(float value)
    {
        foreach (string message in OscControllerMessages)
        {
            //SendOscControllerValue(message.Replace("tx", _trackNumber.ToString(CultureInfo.CurrentCulture)), value);
        }
    }

    private void SendNote(string oscPath, int note, int velocity)
    {
        _oscOut.Send(oscPath, note, velocity);
    }
    private void SendNote(string oscPath, int note, float velocity)
    {
        _oscOut.Send(oscPath, note, velocity);
    }
    private void SendOscControllerValue(string oscPath, float value)
    {
        _oscOut.Send(oscPath, value);
    }
}
