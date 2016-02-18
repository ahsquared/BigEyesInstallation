using UnityEngine;
using System.Collections;

public class ControlSelector : MonoBehaviour
{

    public BodySourceView _bodySourceView;
    public string controlType = "";
    public string[] oscPaths;

    void Awake ()
    {
        if (controlType == "dough")
        {
            oscPaths = new string[2];
        }
    }

    // Use this for initialization
    void Start()
    {
        gameObject.GetComponent<Rigidbody>().velocity = transform.TransformDirection(new Vector3(0, 0, -1));
        //if (controlType == "dough")
        //{
        //    oscPaths = new string[2];
        //}
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter(Collider other)
    {
        if (other.name == "HandLeft")
        {
            Debug.Log("hit left hand");
            _bodySourceView.SetControlType(controlType);
            //Destroy(gameObject);
            gameObject.SetActive(false);
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.name == "HandLeft")
        {
            _bodySourceView.SetControlType("None");
        }
    }
}

