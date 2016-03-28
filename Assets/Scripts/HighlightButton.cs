using UnityEngine;
using System.Collections;

public class HighlightButton : MonoBehaviour {

    private bool _playing = false;
    private string _colliderName;
    private Material _mat;
    private Color _originalColor;
    private string _colorName = "_PrimaryColor";

    // Use this for initialization
    void Start () {
        _mat = gameObject.GetComponent<Renderer>().material;
        _originalColor = _mat.GetColor(_colorName);
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
            _mat.SetColor(_colorName, new Color(255f, 0f, 0f));

            // Fire a burst of particles
            gameObject.GetComponent<ParticleSystem>().Emit(20);
        }
    }
    
    void OnTriggerExit(Collider other)
    {

        if ((other.name == _colliderName) && _playing)
        {
            _playing = false;
            _mat.SetColor(_colorName, _originalColor);
        }
    }
}
