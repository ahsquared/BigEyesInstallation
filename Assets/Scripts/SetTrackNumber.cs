using UnityEngine;
using System.Collections;
using BigEyes;
using UnityEngine.UI;

public class SetTrackNumber : MonoBehaviour
{
    public float TrackNumber;
    public bool TrackActive;

    private InstrumentState _instrumentState;
    private SetTrackNumber[] _instruments;
    private Color _originalColor;
    private Color _activeColor = new Color(33f / 255f, 77f / 255f, 178f / 255f);
    private Image _image;

    // Use this for initialization
    void Start () {
        if (!_instrumentState) _instrumentState = GameObject.Find("InstrumentUI").GetComponent<InstrumentState>();
        _instruments = GameObject.Find("Instruments").GetComponentsInChildren<SetTrackNumber>();
        _image = gameObject.GetComponentInChildren<Image>();
        _originalColor = _image.color;
    }

    // Update is called once per frame
    void Update () {
	
	}
    void OnTriggerEnter(Collider other)
    {

        if ((other.name == "HandLeft" || other.name == "HandRight" ||
            other.name == "WristLeft" || other.name == "WristRight" ||
            other.name == "ThumbLeft" || other.name == "ThumbRight" ||
            other.name == "HandTipLeft" || other.name == "HandTipRight"))
        {
           // _instruments.TrackActive = false;
            foreach (SetTrackNumber instrument in _instruments)
            {
                instrument.SetTrackState(false);
            }
            SetTrackState(true);
            _instrumentState.TrackNumber = TrackNumber;
        }
    }

    public void SetTrackState(bool state)
    {
        _image.color = state ? _activeColor : _originalColor;
        TrackActive = state;
    }
}
