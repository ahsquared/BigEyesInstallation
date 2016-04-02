using UnityEngine;
using System.Collections;

public class EnableRecord : MonoBehaviour
{

    private BodySourceView _bodySourceView;

    void Awake()
    {
        if (!_bodySourceView)
            _bodySourceView = GameObject.Find("Body View").GetComponent<BodySourceView>();
    }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void UpdateRecordEnabledState(bool state)
    {
        Debug.Log(state);
        _bodySourceView.RecordEnabled = state;
    }
}
