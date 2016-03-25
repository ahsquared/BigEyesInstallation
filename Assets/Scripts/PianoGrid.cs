using UnityEngine;
using System.Collections;

public class PianoGrid : MonoBehaviour {

    float _gridSpacing = 0.25f;
    Vector3 _size = new Vector3(3f, 5f, 3f);

    public int[] Notes = new int[] {
        60, 62, 64, 65, 67, 69, 71, 72, 74, 76, 77 , 79   
    };
    public GameObject ButtonNote; 

	// Use this for initialization
	void Start () {
        GeneratePianoGrid(12);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void GeneratePianoGrid(int numKeys)
    {
        for (int i = 0; i < numKeys; i++)
        {
            float xPos = (i % 4) * _size.x;
            float zPos = Mathf.Floor(i / 4) * _size.z;
            Vector3 initPos = new Vector3(xPos + (xPos * _gridSpacing), 0f, zPos + (zPos * _gridSpacing));
            //Debug.Log("pianoKey " + i + initPos);
            GameObject pianoKey = Instantiate(ButtonNote, initPos, Quaternion.identity) as GameObject;
            //GameObject pianoKey = GameObject.CreatePrimitive(PrimitiveType.Cube);
            pianoKey.transform.localScale = _size;
            pianoKey.transform.position = new Vector3(xPos + (xPos * _gridSpacing), 0f, zPos + (zPos * _gridSpacing));
            pianoKey.name = "pianoKey " + i;
            pianoKey.transform.parent = gameObject.transform;
            pianoKey.GetComponent<ButtonNote>().Note = Notes[i];
            pianoKey.GetComponent<ButtonNote>().OscControllerMessages = new string[2];
            pianoKey.GetComponent<ButtonNote>().OscControllerMessages[0] = "/be/track/tx/fx/1";
            pianoKey.GetComponent<ButtonNote>().OscControllerMessages[1] = "/be/track/tx/fx/2";

        }
        gameObject.transform.position = new Vector3(-7f, -3f, 20f);
        Quaternion gridRotation = Quaternion.AngleAxis(335f, Vector3.right);
        gameObject.transform.rotation = gridRotation;
    }
}
