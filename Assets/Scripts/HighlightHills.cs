using UnityEngine;
using System.Collections;

public class HighlightHills : MonoBehaviour {

    private bool _playing = false;
    private string _colliderName;

    private LeapingHill[] _leapingHills;
    private LightingControl[] _spotLights;

    void Start()
    {
        _leapingHills = GameObject.Find("Hills").GetComponentsInChildren<LeapingHill>();
        _spotLights = GameObject.Find("SpotLights").GetComponentsInChildren<LightingControl>();

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

            // Randomly pick a light/hill to spike
            int hillIndex = Random.Range(0, _spotLights.Length);
            // Spike the lights
            //            foreach (LightingControl spotlight in _spotLights)
            //            {
            //                spotlight.SpikeLight();
            //            }
            _spotLights[hillIndex].SpikeLight();

            // make the hills leap
            //            foreach (LeapingHill leapingHill in _leapingHills)
            //            {
            //                leapingHill.SpikeHill();
            //            }
            _leapingHills[hillIndex].SpikeHill();
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
