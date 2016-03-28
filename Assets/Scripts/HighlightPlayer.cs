using UnityEngine;
using System.Collections;

public class HighlightPlayer : MonoBehaviour {
    private bool _playing = false;
    private string _colliderName;

    private LightingControl _playerSpotlight;

    void Start()
    {
        _playerSpotlight = GameObject.Find("Player Spotlight").GetComponent<LightingControl>();

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

           _playerSpotlight.SpikeLight();

        }
    }

    void OnTriggerExit(Collider other)
    {

        if ((other.name == _colliderName) && _playing)
        {
            _playing = false;

        }
    }
}

