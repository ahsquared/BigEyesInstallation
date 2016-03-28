using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PianoGrid : MonoBehaviour {

    private float _gridSpacing = 0.25f;
    private int _numColumns = 6;
    private int _numKeys = 18;
    public float ArcAngle = 40f; // arc degrees
    public float ArcRadius = 30f;
    public float ArcRadiusOffset = 4f;
    public Vector3 TriggerSize = new Vector3(3f, 5f, 3f);
    private List<GameObject> _pianoKeys = new List<GameObject>();

    private int[] _diatonicNotes = new int[] {
        48, 50, 52, 53, 55, 57, 59, 60, 62, 64, 65, 67, 69, 71, 72, 74, 76, 77, 79   
    };
    private int[] _chromaticNotes = new int[] {
        48, 49, 50, 51, 52, 53, 54, 55, 56, 57, 58, 59, 60, 61, 62, 63, 64, 65, 66  
    };
    public GameObject NoteObject; 

	// Use this for initialization
	void Start () {
//        GeneratePianoGrid(_numKeys, _numColumns, _gridSpacing);
        GeneratePianoGridOnArc(_numKeys, _numColumns, ArcAngle, ArcRadius, ArcRadiusOffset);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    Vector3[] GetPointsOnArc(int numKeys, int numColumns, float arcAngle, float arcRadius, float arcRadiusOffset, float yPos, float angleOffset)
    {
        Vector3[] pointsOnArc = new Vector3[numKeys];
        for (int i = 0; i < numKeys; i++)
        {
            float angleIncrement = arcAngle / numColumns;
            int col = i % numColumns;
            float radius = arcRadius + (Mathf.Floor(i / numColumns) * arcRadiusOffset);
            pointsOnArc[i] = new Vector3
            {
                x = (-radius * Mathf.Cos(((angleIncrement * col) + angleOffset) * (Mathf.PI / 180))),
                z = (radius * Mathf.Sin(((angleIncrement * col) + angleOffset) * (Mathf.PI / 180))),
                y = yPos + (Mathf.Floor(i / numColumns)) * 3f
            };
        }
        return pointsOnArc;
    }
//    Vector3[] GetPointsOnArc(int numKeys, int numColumns, float arcAngle, float arcRadius, float arcRadiusOffset, float yPos, float angleOffset)
//    {
//        Vector3[] pointsOnArc = new Vector3[numKeys];
//        for (int i = 0; i < numKeys; i++)
//        {
//            float angleIncrement = arcAngle / ((numColumns - 1) + (Mathf.Floor(i / numColumns) * numColumns));
//            int col = i % numColumns;
//            float radius = arcRadius + (Mathf.Floor(i/ numColumns) *arcRadiusOffset);
//            pointsOnArc[i] = new Vector3
//            {
//                x = (radius * Mathf.Cos(((angleIncrement * col) + angleOffset) * (Mathf.PI / 180))),
//                z = (radius * Mathf.Sin(((angleIncrement * col) + angleOffset) * (Mathf.PI / 180))),
//                y = yPos
//            };
//        }
//        return pointsOnArc;
//    }

    void GeneratePianoGridOnArc(int numKeys, int numColumns, float arcAngle, float arcRadius, float arcRadiusOffset)
    {
        Quaternion keyRotation = Quaternion.AngleAxis(320f, Vector3.right);
        Vector3[] keyPoints = GetPointsOnArc(numKeys, numColumns, arcAngle, arcRadius, arcRadiusOffset, 0, 73f);
        for (int i = 0; i < numKeys; i++)
        {
            Vector3 initPos = new Vector3(keyPoints[i].x, keyPoints[i].y, keyPoints[i].z);
            GameObject pianoKey = Instantiate(NoteObject, initPos, Quaternion.identity) as GameObject;
            _pianoKeys.Add(pianoKey);
            pianoKey.transform.localScale = TriggerSize;
            pianoKey.transform.rotation = keyRotation;
            pianoKey.name = "pianoKey " + i;
            pianoKey.transform.parent = gameObject.transform;
            pianoKey.GetComponent<ButtonNote>().Note = _diatonicNotes[i];
            pianoKey.GetComponent<ButtonNote>().OscControllerMessages = new string[2];
            pianoKey.GetComponent<ButtonNote>().OscControllerMessages[0] = "/be/track/tx/fx/1";
            pianoKey.GetComponent<ButtonNote>().OscControllerMessages[1] = "/be/track/tx/fx/2";

        }
        gameObject.transform.position = new Vector3(-1f, -6, -17f);
        //Quaternion gridRotation = Quaternion.AngleAxis(320f, Vector3.right);
        //gameObject.transform.rotation = gridRotation;
    }

    public void UpdateNotes(bool isChromatic)
    {
        for (int i = 0; i < _pianoKeys.Count; i++)
        {
            _pianoKeys[i].GetComponent<ButtonNote>().Note = isChromatic ? _chromaticNotes[i] : _diatonicNotes[i];
        }
    }

//    void GeneratePianoGrid(int numKeys, int numColumns, float gridSpacing)
//    {
//        for (int i = 0; i < numKeys; i++)
//        {
//            float xPos = (i % numColumns) * _size.x;
//            float zPos = Mathf.Floor(i / numColumns) * _size.z;
//            Vector3 initPos = new Vector3(xPos + (xPos * gridSpacing), 0f, zPos + (zPos * gridSpacing));
//            GameObject pianoKey = Instantiate(NoteObject, initPos, Quaternion.identity) as GameObject;
//            pianoKey.transform.localScale = _size;
//            pianoKey.name = "pianoKey " + i;
//            pianoKey.transform.parent = gameObject.transform;
//            pianoKey.GetComponent<ButtonNote>().Note = _diatonicNotes[i];
//            pianoKey.GetComponent<ButtonNote>().OscControllerMessages = new string[2];
//            pianoKey.GetComponent<ButtonNote>().OscControllerMessages[0] = "/be/track/tx/fx/1";
//            pianoKey.GetComponent<ButtonNote>().OscControllerMessages[1] = "/be/track/tx/fx/2";
//
//        }
//        gameObject.transform.position = new Vector3(-7f, -3f, 20f);
//        Quaternion gridRotation = Quaternion.AngleAxis(335f, Vector3.right);
//        gameObject.transform.rotation = gridRotation;
//       
//    }
}
