using UnityEngine;
using System.Collections;

public class LeapingHill : MonoBehaviour
{

    private float _hillScaleZ;
    private float _hillScaleX;
    private float _hillScaleY;
    private float _heightFactor = 2f;
    private float _spikeUpTime = 0.2f;
    private float _spikeDownTime = 0.4f;

    // Use this for initialization
    void Start ()
	{
        _hillScaleX = transform.localScale.x;
        _hillScaleY = transform.localScale.y;
        _hillScaleZ = transform.localScale.z;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
    public void SpikeHill()
    {
        iTween.Stop(gameObject);
        iTween.ValueTo(gameObject, iTween.Hash("from", _hillScaleZ, "to", _hillScaleZ * _heightFactor, "time", _spikeUpTime, "easeType", "easeOutQuint", "onUpdate", "SetHillHeight", "onComplete", "ResetHeight"));

    }

    private void SetHillHeight(float value)
    {
        transform.localScale = new Vector3(_hillScaleX, _hillScaleY, value);
    }

    private void ResetHeight()
    {
        iTween.ValueTo(gameObject, iTween.Hash("from", _hillScaleZ * _heightFactor, "to", _hillScaleZ, "time", _spikeDownTime, "easeType", "easeOutBounce", "onUpdate", "SetHillHeight"));

    }
}
