using UnityEngine;
using System.Collections;

public class UpdateOptions : MonoBehaviour
{

    private bool _toggleState;
    public GameObject OptionsCanvas;

	// Use this for initialization
	void Start () {
	    if (!OptionsCanvas) OptionsCanvas = Helpers.GetChildGameObjectByName(gameObject, "OptionsCanvas");
        OptionsCanvas.SetActive(_toggleState);

    }

    // Update is called once per frame
    void Update ()
	{
        if (Input.GetKeyUp(KeyCode.Space) )
        {
            _toggleState = !_toggleState;
            OptionsCanvas.SetActive(_toggleState);
        }
    }
}
