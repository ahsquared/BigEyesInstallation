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
    public Light FlareLight;

    void Awake()
    {
        _mat = gameObject.GetComponent<Renderer>().material;
        _primaryOriginalColor = _mat.GetColor(_primaryColorName);
        _secondaryOriginalColor = _mat.GetColor(_secondaryColorName);
        if (!FlareLight) FlareLight = gameObject.GetComponent<Light>();
    }

    // Use this for initialization
    void Start () {
       
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
            FlareLight.intensity = 0.2f;

            // Fire a burst of particles
            gameObject.GetComponent<ParticleSystem>().Emit(20);
        }
    }
    
    void OnTriggerExit(Collider other)
    {

        if ((other.name == _colliderName) && _playing)
        {
            UnHighlightButton();
        }
    }

    public void UnHighlightButton()
    {
        _playing = false;
        _mat.SetColor(_primaryColorName, _primaryOriginalColor);
        _mat.SetColor(_secondaryColorName, _secondaryOriginalColor);
        FlareLight.intensity = 0f;
    }
}
