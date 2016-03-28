using UnityEngine;
using System.Collections;

public class HighlightBarrel : MonoBehaviour {

    private bool _playing = false;
    private string _colliderName;
    private Light _light;

    // Use this for initialization
    void Start()
    {
        if (!_light) _light = gameObject.GetComponent<Light>();
    }

    // Update is called once per frame
    void Update()
    {

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
            _light.intensity = 0f;
        }
    }
}
