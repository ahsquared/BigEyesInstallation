using UnityEngine;
using System.Collections;

public class LookAtCamera : MonoBehaviour {

    public Camera SourceCamera;

    // Use this for initialization
    void Start () {
        if (SourceCamera == null)
        {
            SourceCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
        }
    }

    void Update()
    {
        
        if (SourceCamera != null)
        {
            transform.rotation = SourceCamera.transform.rotation;
        }
    }
}
