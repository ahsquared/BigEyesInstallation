using UnityEngine;
using System.Collections;

public class LightingControl : MonoBehaviour {

    private float _intensity = 0.5f;
    private float _intensityJump = 4f;
    private float _spikeUpTime = 0.4f;
    private float _spikeDownTime = 0.2f;
    private Light _light;

    // Use this for initialization
    void Start()
    {
        _light = gameObject.GetComponent<Light>();
        iTween.Init(gameObject);
        SpikeLight();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SpikeLight()
    {
        iTween.Stop(gameObject);
        iTween.ValueTo(gameObject, iTween.Hash("from", _intensity, "to", _intensity + _intensityJump, "time", _spikeUpTime, "onUpdate", "SetLightIntensity", "onComplete", "ResetIntensity"));

    }

    private void SetLightIntensity(float value)
    {
        _light.intensity = value;
    }

    private void ResetIntensity()
    {
        iTween.ValueTo(gameObject, iTween.Hash("from", _intensity + _intensityJump, "to", _intensity, "time", _spikeDownTime, "onUpdate", "SetLightIntensity"));

    }
}
