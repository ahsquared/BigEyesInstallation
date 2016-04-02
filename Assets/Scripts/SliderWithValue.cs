using UnityEngine;
using System.Collections;
using System.Globalization;
using UnityEngine.UI;

public class SliderWithValue : MonoBehaviour
{

    private Text _value;
	// Use this for initialization
	void Start ()
	{
	    if (!_value) _value = Helpers.GetChildGameObjectByName(gameObject, "Value").GetComponent<Text>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void UpdateValueText(float value)
    {
        _value.text = value.ToString(CultureInfo.CurrentCulture);
    }
}
