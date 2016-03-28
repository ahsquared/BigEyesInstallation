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
        //SpikeLight();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void TweenLightIntensity(float currentIntensity, float newIntensity, string onComplete)
    {
        iTween.Stop(gameObject);
        iTween.ValueTo(gameObject, iTween.Hash("from", currentIntensity, "to", newIntensity, "time", _spikeUpTime, "onUpdate", "SetLightIntensity", "onComplete", onComplete));
    }
    public void TweenLightIntensity(float currentIntensity, float newIntensity)
    {
        iTween.Stop(gameObject);
        iTween.ValueTo(gameObject, iTween.Hash("from", currentIntensity, "to", newIntensity, "time", _spikeUpTime, "onUpdate", "SetLightIntensity"));
    }

    public void SpikeLight()
    {
        TweenLightIntensity(_intensity, _intensity + _intensityJump, "ResetIntensity");
    }

    private void SetLightIntensity(float value)
    {
        _light.intensity = value;
    }

    private void ResetIntensity()
    {
        TweenLightIntensity(_intensity + _intensityJump, _intensity);

    }
}
