using UnityEngine;
using System.Collections;

public class HighlightButton : MonoBehaviour {

    private bool _playing = false;
    private string _colliderName;
    private Material _mat;
    private Color _primaryOriginalColor;
    private Color _secondaryOriginalColor;
    private string _primaryColorName = "_PrimaryColor";
    private string _secondaryColorName = "_SecondaryColor";
    private Light _light;

    // Use this for initialization
    void Start () {
        _mat = gameObject.GetComponent<Renderer>().material;
        _primaryOriginalColor = _mat.GetColor(_primaryColorName);
        _secondaryOriginalColor = _mat.GetColor(_secondaryColorName);
        if (!_light) _light = gameObject.GetComponent<Light>();
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter(Collider other)
    {

        //if ((other.name == "HandLeft" || other.name == "HandRight" ||
        //    other.name == "WristLeft" || other.name == "WristRight" ||
        //    other.name == "ThumbLeft" || other.name == "ThumbRight" ||
        //    other.name == "HandTipLeft" || other.name == "HandTipRight") && !playing)
        //{
        if ((other.name == "HandLeft" || other.name == "HandRight") && !_playing)
        {
            //Debug.Log("piano note played: " + gameObject.name);
            _playing = true;
            _colliderName = other.name;

            // highlight button
            _mat.SetColor(_primaryColorName, new Color(255f, 0f, 216f));
            _mat.SetColor(_secondaryColorName, new Color(255f, 0f, 216f));

            // turn on flare
            _light.intensity = 0.2f;

            // Fire a burst of particles
            gameObject.GetComponent<ParticleSystem>().Emit(20);
        }
    }
    
    void OnTriggerExit(Collider other)
    {

        if ((other.name == _colliderName) && _playing)
        {
            _playing = false;
            _mat.SetColor(_primaryColorName, _primaryOriginalColor);
            _mat.SetColor(_secondaryColorName, _secondaryOriginalColor);
            _light.intensity = 0f;
        }
    }
}
