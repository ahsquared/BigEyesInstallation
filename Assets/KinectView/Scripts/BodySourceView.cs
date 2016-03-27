using UnityEngine;
using UnityStandardAssets.ImageEffects;
using System.Collections;
using System.Collections.Generic;
using Kinect = Windows.Kinect;
using Math = System.Math;
using BigEyes;

public class BodySourceView : MonoBehaviour
{
    public GameObject MainCamera;

    public Material HandMaterial;
    public GameObject ParticleSystem;
    public GameObject BodySourceManager;

    private OSCController _oscControl;

    public OSCBodyController BodyController;
    public GameObject BodyControllerViz;

    public string BodySide = "left";
    private Kinect.HandState _handState = Kinect.HandState.Unknown;
    private bool _handHasOpened = true;
    public bool UseHandState = false;
    public int ClapWidth = 10;

    public bool Mirrored = false;
    public float ZOffset = 35f;

    private Dictionary<ulong, GameObject> _bodies = new Dictionary<ulong, GameObject>();
    private BodySourceManager _bodyManager;

    private string _controlType = "none";

    private float _maxArmWidth = 0f;
    private float _maxHandDepth = 0f;
    private float _maxHeight = 0f;

    public bool AddLines = false;

    public bool AddRigidBody = false;
    private float _groundPositionY = 0f;
    public GameObject Ground;

    private List<string> _oscPaths = new List<string>();

    /// <summary>
    /// The currently tracked body
    /// </summary>
    private Kinect.Body _currentTrackedBody = null;

    /// <summary>
    /// The currently tracked body
    /// </summary>
    private ulong _currentTrackingId = 0;

    private Dictionary<Kinect.JointType, Kinect.JointType> _boneMap = new Dictionary<Kinect.JointType, Kinect.JointType>()
    {
        { Kinect.JointType.FootLeft, Kinect.JointType.AnkleLeft },
        { Kinect.JointType.AnkleLeft, Kinect.JointType.KneeLeft },
        { Kinect.JointType.KneeLeft, Kinect.JointType.HipLeft },
        { Kinect.JointType.HipLeft, Kinect.JointType.SpineBase },

        { Kinect.JointType.FootRight, Kinect.JointType.AnkleRight },
        { Kinect.JointType.AnkleRight, Kinect.JointType.KneeRight },
        { Kinect.JointType.KneeRight, Kinect.JointType.HipRight },
        { Kinect.JointType.HipRight, Kinect.JointType.SpineBase },

        { Kinect.JointType.HandTipLeft, Kinect.JointType.HandLeft },
        { Kinect.JointType.ThumbLeft, Kinect.JointType.HandLeft },
        { Kinect.JointType.HandLeft, Kinect.JointType.WristLeft },
        { Kinect.JointType.WristLeft, Kinect.JointType.ElbowLeft },
        { Kinect.JointType.ElbowLeft, Kinect.JointType.ShoulderLeft },
        { Kinect.JointType.ShoulderLeft, Kinect.JointType.SpineShoulder },

        { Kinect.JointType.HandTipRight, Kinect.JointType.HandRight },
        { Kinect.JointType.ThumbRight, Kinect.JointType.HandRight },
        { Kinect.JointType.HandRight, Kinect.JointType.WristRight },
        { Kinect.JointType.WristRight, Kinect.JointType.ElbowRight },
        { Kinect.JointType.ElbowRight, Kinect.JointType.ShoulderRight },
        { Kinect.JointType.ShoulderRight, Kinect.JointType.SpineShoulder },

        { Kinect.JointType.SpineBase, Kinect.JointType.SpineMid },
        { Kinect.JointType.SpineMid, Kinect.JointType.SpineShoulder },
        { Kinect.JointType.SpineShoulder, Kinect.JointType.Neck },
        { Kinect.JointType.Neck, Kinect.JointType.Head },

    };

   

    void Start()
    {
       if (!_oscControl) _oscControl = GameObject.Find("OSC").GetComponent<OSCController>();
    }

    void Update()
    {
        if (BodySourceManager == null)
        {
            return;
        }

        _bodyManager = BodySourceManager.GetComponent<BodySourceManager>();
        if (_bodyManager == null)
        {
            return;
        }

        Kinect.Body[] data = _bodyManager.GetData();
        if (data == null)
        {
            return;
        }

        List<ulong> trackedIds = new List<ulong>();
        foreach (var body in data)
        {
            if (body == null)
            {
                continue;
            }

            if (body.IsTracked)
            {
                trackedIds.Add(body.TrackingId);
            }
        }

        List<ulong> knownIds = new List<ulong>(_bodies.Keys);      

        Kinect.Body closestBody = FindClosestBody(data);

        if (closestBody == null)
        {
            return;
        }

        // First delete untracked bodies
        foreach (ulong trackingId in knownIds)
        {
            if (!trackedIds.Contains(trackingId))
            {
                Destroy(_bodies[trackingId]);
                _bodies.Remove(trackingId);
            }
            // delete bodies that aren't the closest
            if (trackingId != closestBody.TrackingId)
            {
                Destroy(GameObject.Find("Body:" + trackingId));
                _bodies.Remove(trackingId);
            }
        }

        if (closestBody.IsTracked)
        {
            if (!_bodies.ContainsKey(closestBody.TrackingId))
            {
                _bodies[closestBody.TrackingId] = CreateHands(closestBody, closestBody.TrackingId);
                _oscControl.SendOSC("/be/play", 1f);
            }

            RefreshHands(closestBody, _bodies[closestBody.TrackingId], (long) closestBody.TrackingId);
        }
        
        //foreach (var body in data)
        //{
        //    if (body == null)
        //    {
        //        continue;
        //    }

        //    if (body.IsTracked)
        //    {
        //        if (!_Bodies.ContainsKey(body.TrackingId))
        //        {
        //            _Bodies[body.TrackingId] = CreateBodyObject(body.TrackingId);
        //        }

        //        RefreshBodyObject(body, _Bodies[body.TrackingId]);
        //    }
        //}
    }

    private GameObject CreateBodyObject(Kinect.Body kinectBody, ulong id)
    {
        GameObject body = new GameObject("Body:" + id);

        if (AddRigidBody)
        {
            body.AddComponent<Rigidbody>();
            body.transform.position = new Vector3(0, 5, 0);
            body.GetComponent<Rigidbody>().freezeRotation = true;
        }

        for (Kinect.JointType jt = Kinect.JointType.SpineBase; jt <= Kinect.JointType.ThumbRight; jt++)
        {
            GameObject jointObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            jointObj.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
            jointObj.name = jt.ToString();
            Rigidbody rb = jointObj.AddComponent<Rigidbody>();
            rb.useGravity = false;
            jointObj.transform.parent = body.transform;

            if (AddLines)
            {
                LineRenderer lr = jointObj.AddComponent<LineRenderer>();
                lr.SetVertexCount(2);
                lr.material = HandMaterial;
                lr.SetWidth(0.2f, 0.2f);
            }

            if (jt == Kinect.JointType.Head)
            {
                jointObj.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
            }

        }

        MainCamera.GetComponent<DepthOfField>().focalTransform = GameObject.Find("SpineBase").transform;

        _maxHeight = (GetVector3FromJoint(kinectBody.Joints[Kinect.JointType.Head]) - GetVector3FromJoint(kinectBody.Joints[Kinect.JointType.FootLeft])).sqrMagnitude;

        return body;
    }

    private void RefreshBodyObject(Kinect.Body body, GameObject bodyObject, long trackingId)
    {
        

        for (Kinect.JointType jt = Kinect.JointType.SpineBase; jt <= Kinect.JointType.ThumbRight; jt++)
        {
            Kinect.Joint sourceJoint = body.Joints[jt];
            Kinect.Joint? targetJoint = null;

            if (_boneMap.ContainsKey(jt))
            {
                targetJoint = body.Joints[_boneMap[jt]];
            }
           
            if (jt == Kinect.JointType.FootLeft)
            {
                _groundPositionY = Mathf.Min(_groundPositionY, GetVector3FromJoint(body.Joints[Kinect.JointType.FootLeft]).y);
            }
            if (jt == Kinect.JointType.FootRight)
            {
                _groundPositionY = Mathf.Min(_groundPositionY, GetVector3FromJoint(body.Joints[Kinect.JointType.FootRight]).y);
            }
            
            Transform jointObj = bodyObject.transform.FindChild(jt.ToString());
            jointObj.localPosition = GetVector3FromJoint(sourceJoint);
            
            if (AddLines)
            {
                LineRenderer lr = jointObj.GetComponent<LineRenderer>();
                if (targetJoint.HasValue)
                {                   
                    lr.SetPosition(0, jointObj.localPosition);
                    lr.SetPosition(1, GetVector3FromJoint(targetJoint.Value));
                    lr.SetColors(GetColorForState(sourceJoint.TrackingState), GetColorForState(targetJoint.Value.TrackingState));
                }
                else
                {
                    lr.enabled = false;
                }
            }

        }
       
        if (_controlType.ToLower() == "cylinder" && BodyControllerViz != null)
        {
            Vector3 handWidth = GetVector3FromJoint(body.Joints[Kinect.JointType.HandRight]) - GetVector3FromJoint(body.Joints[Kinect.JointType.HandLeft]);
            float width = handWidth.sqrMagnitude;
            _maxArmWidth = Mathf.Max(width, _maxArmWidth);
            float angle = Vector3.Angle(handWidth, transform.right);
            //Debug.Log("distance: " + width);
            //Debug.Log("maxArmWidth: " + maxArmWidth);
            //Debug.Log("volume: " + width / maxArmWidth);
            // compute log scale from linear scale
            // x'i = (log(xi)-log(xmin)) / (log(xmax)-log(xmin))​
            float widthLog = (Mathf.Log(width)) / (Mathf.Log(_maxArmWidth));

            BodyControllerViz.transform.localScale = new Vector3(0.2f, handWidth.magnitude / 4, 0.2f);
            BodyControllerViz.transform.rotation = Quaternion.LookRotation(handWidth) * Quaternion.Euler(90, 0, 0);
            BodyControllerViz.transform.position = GetCenterPosition(body);

            _oscControl.SendOSC(_oscPaths[0], widthLog);
            _oscControl.SendOSC(_oscPaths[1], angle / 180f);
        }
        if (_controlType.ToLower() == "stick" && BodyControllerViz != null)
        {
            Vector3 handWidth = GetVector3FromJoint(body.Joints[Kinect.JointType.HandRight]) - GetVector3FromJoint(body.Joints[Kinect.JointType.HandLeft]);
            Vector3 handHeight;
            Vector3 midpoint;
            if (BodySide == "right")
            {
                handHeight = GetVector3FromJoint(body.Joints[Kinect.JointType.HandRight]) - GetVector3FromJoint(body.Joints[Kinect.JointType.FootRight]);
                midpoint = GetMidpointOfVectors(body.Joints[Kinect.JointType.HandRight], body.Joints[Kinect.JointType.FootRight]);
                _handState = body.HandRightState;
            } else
            {
                handHeight = GetVector3FromJoint(body.Joints[Kinect.JointType.HandLeft]) - GetVector3FromJoint(body.Joints[Kinect.JointType.FootLeft]);
                midpoint = GetMidpointOfVectors(body.Joints[Kinect.JointType.HandLeft], body.Joints[Kinect.JointType.FootLeft]);
                _handState = body.HandLeftState;
            }
            float height = handHeight.sqrMagnitude;
            _maxHeight = Mathf.Max(height, _maxHeight);
            float angle = Vector3.Angle(handHeight, transform.right);
            float heightLog = (Mathf.Log(height)) / (Mathf.Log(_maxHeight));

            BodyControllerViz.transform.localScale = new Vector3(0.2f, handHeight.magnitude / 4, 0.2f);
            BodyControllerViz.transform.rotation = Quaternion.LookRotation(handHeight) * Quaternion.Euler(90, 0, 0);
            BodyControllerViz.transform.position = midpoint;

            if (UseHandState)
            {
                if (_handState == Kinect.HandState.Closed && _handHasOpened)
                {
                    _oscControl.SendOSC(_oscPaths[0], (height / _maxHeight) * 128f, 0.5f);
                    _handHasOpened = false;
                    Debug.Log("send note: " + (height / _maxHeight) * 128f);
                }
                else if (_handState == Kinect.HandState.Open)
                {
                    _handHasOpened = true;
                }
            } else {
                if (handWidth.sqrMagnitude < ClapWidth && _handHasOpened)
                {
                    _oscControl.SendOSC(_oscPaths[0], (height / _maxHeight) * 128f, 0.1f);
                    _handHasOpened = false;
                }
                else if (handWidth.sqrMagnitude >= ClapWidth)
                {
                    _handHasOpened = true;
                }
            }
            Debug.Log("stick rotation angle: " + angle + ", " + Mathf.Clamp(((angle - 45f) / 90f) * 127f, 0f, 127f));
            _oscControl.SendOSC(_oscPaths[1], Mathf.Clamp(((angle - 45f) / 90f) * 127f, 0f, 127f));

        }
        if (_controlType.ToLower() == "sphere" && BodyControllerViz != null)
        {
            float width = GetHandWidth(body);
            _maxArmWidth = Mathf.Max(width, _maxArmWidth);
            float angle = GetHandAngle(body);
            float depth = GetHandDepthFromBody(body);
            _maxHandDepth = Mathf.Max(depth, _maxHandDepth);

            //Debug.Log("distance: " + width);
            //Debug.Log("maxArmWidth: " + maxArmWidth);
            //Debug.Log("volume: " + width / maxArmWidth);
            //Debug.Log("depth: " + depth / maxHandDepth);

            float widthLog = (Mathf.Log(width)) / (Mathf.Log(_maxArmWidth));
            float depthLog = (Mathf.Log(depth)) / (Mathf.Log(_maxHandDepth));

            BodyControllerViz.transform.localScale = new Vector3(width / 2, width / 2, width / 2);
            BodyControllerViz.transform.rotation = Quaternion.LookRotation(GetVector3FromJoint(body.Joints[Kinect.JointType.HandRight]) - GetVector3FromJoint(body.Joints[Kinect.JointType.HandLeft]));
            BodyControllerViz.transform.position = GetCenterPosition(body);

            _oscControl.SendOSC(_oscPaths[0], widthLog);
            _oscControl.SendOSC(_oscPaths[1], angle / 180f);
            _oscControl.SendOSC(_oscPaths[2], depthLog);
        }
    }

    private GameObject CreateHands(Kinect.Body kinectBody, ulong id)
    {
        GameObject body = new GameObject("Body:" + id);
        Kinect.JointType[] handJoints = {
            Kinect.JointType.HandLeft, Kinect.JointType.HandRight
        };
        if (AddRigidBody)
        {
            body.AddComponent<Rigidbody>();
            body.transform.position = new Vector3(0, 5, 0);
            body.GetComponent<Rigidbody>().freezeRotation = true;
        }
        Vector3 initPos = new Vector3(0, 0, 0);
        for (Kinect.JointType jt = Kinect.JointType.SpineBase; jt <= Kinect.JointType.ThumbRight; jt++)
        {
            GameObject jointObj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            jointObj.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
            jointObj.name = jt.ToString();
            Rigidbody rb = jointObj.AddComponent<Rigidbody>();
            rb.useGravity = false;
            jointObj.transform.GetComponent<MeshRenderer>().enabled = false;
            foreach (Kinect.JointType j in handJoints)
            {
                if (j.ToString() == jt.ToString())
                {
                    jointObj.transform.GetComponent<MeshRenderer>().enabled = true;
                    jointObj.transform.GetComponent<MeshRenderer>().material = HandMaterial;
                    jointObj.transform.localScale = new Vector3(1f, 1f, 1f);
                }

            }
            jointObj.transform.parent = body.transform;

            if (AddLines)
            {
                
                GameObject psObject = Instantiate(ParticleSystem, initPos, Quaternion.identity) as GameObject;
                ParticleSystem ps = psObject.GetComponent<ParticleSystem>();
                ps.Play();
                psObject.transform.localScale = new Vector3(1f, 1f, 1f);
                psObject.transform.parent = jointObj.transform;

            }

        }

        MainCamera.GetComponent<DepthOfField>().focalTransform = GameObject.Find("HandLeft").transform;

        _maxHeight = (GetVector3FromJoint(kinectBody.Joints[Kinect.JointType.Head]) - GetVector3FromJoint(kinectBody.Joints[Kinect.JointType.FootLeft])).sqrMagnitude;

        return body;
    }


    private void RefreshHands(Kinect.Body body, GameObject bodyObject, long trackingId)
    {

//        Kinect.JointType[] handJoints = {
//            Kinect.JointType.HandLeft, Kinect.JointType.HandRight
//        };
//        foreach (Kinect.JointType hand in handJoints)
//        {
//            Kinect.Joint sourceJoint = body.Joints[hand];
//
//            Transform jointObj = bodyObject.transform.FindChild(hand.ToString());
//            jointObj.localPosition = GetVector3FromJoint(sourceJoint);
//
//        }
        for (Kinect.JointType jt = Kinect.JointType.SpineBase; jt <= Kinect.JointType.ThumbRight; jt++)
        {
            Kinect.Joint sourceJoint = body.Joints[jt];

            if (jt == Kinect.JointType.FootLeft)
            {
                _groundPositionY = Mathf.Min(_groundPositionY, GetVector3FromJoint(body.Joints[Kinect.JointType.FootLeft]).y);
            }
            if (jt == Kinect.JointType.FootRight)
            {
                _groundPositionY = Mathf.Min(_groundPositionY, GetVector3FromJoint(body.Joints[Kinect.JointType.FootRight]).y);
            }

            Transform jointObj = bodyObject.transform.FindChild(jt.ToString());
            jointObj.localPosition = GetVector3FromJoint(sourceJoint);

        }
        if (_controlType.ToLower() == "cylinder" && BodyControllerViz != null)
        {
            Vector3 handWidth = GetVector3FromJoint(body.Joints[Kinect.JointType.HandRight]) - GetVector3FromJoint(body.Joints[Kinect.JointType.HandLeft]);
            float width = handWidth.sqrMagnitude;
            _maxArmWidth = Mathf.Max(width, _maxArmWidth);
            float angle = Vector3.Angle(handWidth, transform.right);
            //Debug.Log("distance: " + width);
            //Debug.Log("maxArmWidth: " + maxArmWidth);
            //Debug.Log("volume: " + width / maxArmWidth);
            // compute log scale from linear scale
            // x'i = (log(xi)-log(xmin)) / (log(xmax)-log(xmin))​
            float widthLog = (Mathf.Log(width)) / (Mathf.Log(_maxArmWidth));

            BodyControllerViz.transform.localScale = new Vector3(0.2f, handWidth.magnitude / 4, 0.2f);
            BodyControllerViz.transform.rotation = Quaternion.LookRotation(handWidth) * Quaternion.Euler(90, 0, 0);
            BodyControllerViz.transform.position = GetCenterPosition(body);

            _oscControl.SendOSC(_oscPaths[0], widthLog);
            _oscControl.SendOSC(_oscPaths[1], angle / 180f);
        }
        if (_controlType.ToLower() == "stick" && BodyControllerViz != null)
        {
            Vector3 handWidth = GetVector3FromJoint(body.Joints[Kinect.JointType.HandRight]) - GetVector3FromJoint(body.Joints[Kinect.JointType.HandLeft]);
            Vector3 handHeight;
            Vector3 midpoint;
            if (BodySide == "right")
            {
                handHeight = GetVector3FromJoint(body.Joints[Kinect.JointType.HandRight]) - GetVector3FromJoint(body.Joints[Kinect.JointType.FootRight]);
                midpoint = GetMidpointOfVectors(body.Joints[Kinect.JointType.HandRight], body.Joints[Kinect.JointType.FootRight]);
                _handState = body.HandRightState;
            }
            else
            {
                handHeight = GetVector3FromJoint(body.Joints[Kinect.JointType.HandLeft]) - GetVector3FromJoint(body.Joints[Kinect.JointType.FootLeft]);
                midpoint = GetMidpointOfVectors(body.Joints[Kinect.JointType.HandLeft], body.Joints[Kinect.JointType.FootLeft]);
                _handState = body.HandLeftState;
            }
            float height = handHeight.sqrMagnitude;
            _maxHeight = Mathf.Max(height, _maxHeight);
            float angle = Vector3.Angle(handHeight, transform.right);
            float heightLog = (Mathf.Log(height)) / (Mathf.Log(_maxHeight));

            BodyControllerViz.transform.localScale = new Vector3(0.2f, handHeight.magnitude / 4, 0.2f);
            BodyControllerViz.transform.rotation = Quaternion.LookRotation(handHeight) * Quaternion.Euler(90, 0, 0);
            BodyControllerViz.transform.position = midpoint;

            if (UseHandState)
            {
                if (_handState == Kinect.HandState.Closed && _handHasOpened)
                {
                    _oscControl.SendOSC(_oscPaths[0], (height / _maxHeight) * 128f, 0.5f);
                    _handHasOpened = false;
                    Debug.Log("send note: " + (height / _maxHeight) * 128f);
                }
                else if (_handState == Kinect.HandState.Open)
                {
                    _handHasOpened = true;
                }
            }
            else
            {
                if (handWidth.sqrMagnitude < ClapWidth && _handHasOpened)
                {
                    _oscControl.SendOSC(_oscPaths[0], (height / _maxHeight) * 128f, 0.1f);
                    _handHasOpened = false;
                }
                else if (handWidth.sqrMagnitude >= ClapWidth)
                {
                    _handHasOpened = true;
                }
            }
            Debug.Log("stick rotation angle: " + angle + ", " + Mathf.Clamp(((angle - 45f) / 90f) * 127f, 0f, 127f));
            _oscControl.SendOSC(_oscPaths[1], Mathf.Clamp(((angle - 45f) / 90f) * 127f, 0f, 127f));

        }
        if (_controlType.ToLower() == "sphere" && BodyControllerViz != null)
        {
            float width = GetHandWidth(body);
            _maxArmWidth = Mathf.Max(width, _maxArmWidth);
            float angle = GetHandAngle(body);
            float depth = GetHandDepthFromBody(body);
            _maxHandDepth = Mathf.Max(depth, _maxHandDepth);

            //Debug.Log("distance: " + width);
            //Debug.Log("maxArmWidth: " + maxArmWidth);
            //Debug.Log("volume: " + width / maxArmWidth);
            //Debug.Log("depth: " + depth / maxHandDepth);

            float widthLog = (Mathf.Log(width)) / (Mathf.Log(_maxArmWidth));
            float depthLog = (Mathf.Log(depth)) / (Mathf.Log(_maxHandDepth));

            BodyControllerViz.transform.localScale = new Vector3(width / 2, width / 2, width / 2);
            BodyControllerViz.transform.rotation = Quaternion.LookRotation(GetVector3FromJoint(body.Joints[Kinect.JointType.HandRight]) - GetVector3FromJoint(body.Joints[Kinect.JointType.HandLeft]));
            BodyControllerViz.transform.position = GetCenterPosition(body);

            _oscControl.SendOSC(_oscPaths[0], widthLog);
            _oscControl.SendOSC(_oscPaths[1], angle / 180f);
            _oscControl.SendOSC(_oscPaths[2], depthLog);
        }
    }
    /// <summary>
    /// Set OSC Paths used by controllers
    ///
    /// </summary>
    /// <param name="oscPaths"></param>
    public void SetOscPaths(List<string> oscPaths)
    {

        Debug.Log("OSC Paths before:" + _oscPaths.Count);
        _oscPaths.Clear();
        foreach (string path in oscPaths)
        {
            _oscPaths.Add(path);
        }
        Debug.Log("OSC Paths after:" + _oscPaths.Count);
    }

    Vector3 GetCenterPosition(Kinect.Body body)
    {
        Vector3 midPoint = ((GetVector3FromJoint(body.Joints[Kinect.JointType.HandRight]) - GetVector3FromJoint(body.Joints[Kinect.JointType.HandLeft])) * 0.5f) + GetVector3FromJoint(body.Joints[Kinect.JointType.HandLeft]);
        return midPoint;
    }

    Vector3 GetMidpointOfVectors(Kinect.Joint joint1, Kinect.Joint joint2)
    {
        Vector3 midPoint = ((GetVector3FromJoint(joint1) - GetVector3FromJoint(joint2)) * 0.5f) + GetVector3FromJoint(joint2);
        return midPoint;
    }

    /// <summary>
    /// get the distance between the left and right hand
    /// </summary>
    /// <param name="body"></param>
    /// <returns>float between left and right hand</returns>
    float GetHandWidth(Kinect.Body body)
    {
        Vector3 handWidth = GetVector3FromJoint(body.Joints[Kinect.JointType.HandRight]) - GetVector3FromJoint(body.Joints[Kinect.JointType.HandLeft]);
        return handWidth.magnitude;
    }

    /// <summary>
    /// get the angle of the line between left and right hands relative to floor
    /// </summary>
    /// <param name="handWidth"></param>
    /// <returns>float</returns>
    float GetHandAngle(Kinect.Body body)
    {
        Vector3 handWidth = GetVector3FromJoint(body.Joints[Kinect.JointType.HandRight]) - GetVector3FromJoint(body.Joints[Kinect.JointType.HandLeft]);
        float angle = Vector3.Angle(handWidth, transform.right);
        return angle;
    }

    float GetHandDepthFromBody(Kinect.Body body)
    {
        Vector3 leftHandDepth = GetVector3FromJoint(body.Joints[Kinect.JointType.SpineShoulder]) - GetVector3FromJoint(body.Joints[Kinect.JointType.HandLeft]);
        Vector3 rightHandDepth = GetVector3FromJoint(body.Joints[Kinect.JointType.SpineShoulder]) - GetVector3FromJoint(body.Joints[Kinect.JointType.HandRight]);

        float leftHandDepthMagZ = Mathf.Abs(leftHandDepth.z);
        float rightHandDepthMagZ = Mathf.Abs(rightHandDepth.z);

        if (leftHandDepthMagZ > rightHandDepthMagZ)
        {
            return leftHandDepthMagZ;
        }
        else
        {
            return rightHandDepthMagZ;
        }
    }

    /// <summary>
    /// Finds the closest body from the sensor if any
    /// </summary>
    /// <param name="bodyFrame">A body frame</param>
    /// <returns>Closest body, null of none</returns>
    private static Kinect.Body FindClosestBody(Kinect.Body[] bodies)
    {
        Kinect.Body result = null;
        double closestBodyDistance = double.MaxValue;

        foreach (var body in bodies)
        {
            if (body.IsTracked)
            {
                var currentLocation = body.Joints[Kinect.JointType.SpineBase].Position;

                var currentDistance = VectorLength(currentLocation);

                if (result == null || currentDistance < closestBodyDistance)
                {
                    result = body;
                    closestBodyDistance = currentDistance;
                }
            }
        }

        return result;
    }

    /// <summary>
    /// Returns the length of a vector from origin
    /// </summary>
    /// <param name="point">Point in space to find it's distance from origin</param>
    /// <returns>Distance from origin</returns>
    private static double VectorLength(Kinect.CameraSpacePoint point)
    {
        var result = Math.Pow(point.X, 2) + Math.Pow(point.Y, 2) + Math.Pow(point.Z, 2);

        result = Math.Sqrt(result);

        return result;
    }

    public void SetControlType(string ct)
    {
        _controlType = ct;

    }

    private static Color GetColorForState(Kinect.TrackingState state)
    {
        switch (state)
        {
            case Kinect.TrackingState.Tracked:
                return Color.green;

            case Kinect.TrackingState.Inferred:
                return Color.red;

            default:
                return Color.black;
        }
    }

    private Vector3 GetVector3FromJoint(Kinect.Joint joint)
    {
        float z = Mirrored ? joint.Position.Z * 20 : ((-joint.Position.Z * 20) + ZOffset);
        return new Vector3(joint.Position.X * 20, joint.Position.Y * 20, z);
    }
}
