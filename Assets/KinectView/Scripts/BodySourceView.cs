using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Kinect = Windows.Kinect;
using Math = System.Math;

public class BodySourceView : MonoBehaviour
{
    public Material BoneMaterial;
    public GameObject BodySourceManager;

    public BigEyes.OSCController oscController;
    private BigEyes.OSCController oscControl;

    public OSCBodyController bodyController;
    public GameObject bodyControllerViz;

    private Dictionary<ulong, GameObject> _Bodies = new Dictionary<ulong, GameObject>();
    private BodySourceManager _BodyManager;

    private string controlType = "none";
    private GameObject trail;

    private float maxArmWidth = 0f;
    private float maxHandDepth = 0f;

    public bool addLines = false;

    public bool addRigidBody = false;
    private float groundPositionY = 0f;
    public GameObject ground;

    private List<string> _oscPaths = new List<string>();

    /// <summary>
    /// The currently tracked body
    /// </summary>
    private Kinect.Body currentTrackedBody = null;

    /// <summary>
    /// The currently tracked body
    /// </summary>
    private ulong currentTrackingId = 0;

    private Dictionary<Kinect.JointType, Kinect.JointType> _BoneMap = new Dictionary<Kinect.JointType, Kinect.JointType>()
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
        oscControl = oscController.GetComponent<BigEyes.OSCController>();
    }

    void Update()
    {
        if (BodySourceManager == null)
        {
            return;
        }

        _BodyManager = BodySourceManager.GetComponent<BodySourceManager>();
        if (_BodyManager == null)
        {
            return;
        }

        Kinect.Body[] data = _BodyManager.GetData();
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

        List<ulong> knownIds = new List<ulong>(_Bodies.Keys);

        // First delete untracked bodies
        foreach (ulong trackingId in knownIds)
        {
            if (!trackedIds.Contains(trackingId))
            {
                Destroy(_Bodies[trackingId]);
                _Bodies.Remove(trackingId);
            }
        }

        Kinect.Body closestBody = FindClosestBody(data);

        if (closestBody == null)
        {
            return;
        }

        if (closestBody.IsTracked)
        {
            if (!_Bodies.ContainsKey(closestBody.TrackingId))
            {
                _Bodies[closestBody.TrackingId] = CreateBodyObject(closestBody.TrackingId);
            }

            RefreshBodyObject(closestBody, _Bodies[closestBody.TrackingId]);
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

    private GameObject CreateBodyObject(ulong id)
    {
        GameObject body = new GameObject("Body:" + id);
        if (addRigidBody)
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
            jointObj.transform.parent = body.transform;

            if (addLines)
            {
                LineRenderer lr = jointObj.AddComponent<LineRenderer>();
                lr.SetVertexCount(2);
                lr.material = BoneMaterial;
                lr.SetWidth(1f, 1f);
            }

            if (jt == Kinect.JointType.Head)
            {
                jointObj.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
            }

        }
        trail = GameObject.Find("Trail");
        trail.GetComponent<TrailRenderer>().enabled = false;
        return body;
    }

    private void RefreshBodyObject(Kinect.Body body, GameObject bodyObject)
    {
        float groundOffset;
        Vector3 targetPosition;
        Vector3 offsetTargetPosition;

        for (Kinect.JointType jt = Kinect.JointType.SpineBase; jt <= Kinect.JointType.ThumbRight; jt++)
        {
            Kinect.Joint sourceJoint = body.Joints[jt];
            Kinect.Joint? targetJoint = null;

            if (_BoneMap.ContainsKey(jt))
            {
                targetJoint = body.Joints[_BoneMap[jt]];
            }

            if (jt == Kinect.JointType.FootLeft)
            {
                groundPositionY = Mathf.Min(groundPositionY, GetVector3FromJoint(body.Joints[Kinect.JointType.FootLeft]).y);
            }
            if (jt == Kinect.JointType.FootRight)
            {
                groundPositionY = Mathf.Min(groundPositionY, GetVector3FromJoint(body.Joints[Kinect.JointType.FootRight]).y);
            }
            
            groundOffset = -(groundPositionY - ground.transform.position.y);

            Transform jointObj = bodyObject.transform.FindChild(jt.ToString());
            jointObj.localPosition = GetVector3FromJoint(sourceJoint);
            Vector3 offsetJointPosition = new Vector3(jointObj.localPosition.x, jointObj.localPosition.y + groundOffset, jointObj.localPosition.z);
            if (addLines)
            {
                LineRenderer lr = jointObj.GetComponent<LineRenderer>();
                if (targetJoint.HasValue)
                {
                    targetPosition = GetVector3FromJoint(targetJoint.Value);
                    offsetTargetPosition = new Vector3(targetPosition.x, targetPosition.y + groundOffset, targetPosition.z);
                    lr.SetPosition(0, offsetJointPosition);
                    lr.SetPosition(1, offsetTargetPosition);
                    lr.SetColors(GetColorForState(sourceJoint.TrackingState), GetColorForState(targetJoint.Value.TrackingState));
                }
                else
                {
                    lr.enabled = false;
                }
            }

        }
        if (controlType == "trail")
        {
            var pos = GetVector3FromJoint(body.Joints[Kinect.JointType.HandLeft]);
            trail.transform.position = pos;

        }
        if (controlType.ToLower() == "cylinder")
        {
            Vector3 handWidth = GetVector3FromJoint(body.Joints[Kinect.JointType.HandRight]) - GetVector3FromJoint(body.Joints[Kinect.JointType.HandLeft]);
            float width = handWidth.sqrMagnitude;
            maxArmWidth = Mathf.Max(width, maxArmWidth);
            float angle = Vector3.Angle(handWidth, transform.right);
            //Debug.Log("distance: " + width);
            //Debug.Log("maxArmWidth: " + maxArmWidth);
            //Debug.Log("volume: " + width / maxArmWidth);
            // compute log scale from linear scale
            // x'i = (log(xi)-log(xmin)) / (log(xmax)-log(xmin))​
            float widthLog = (Mathf.Log(width)) / (Mathf.Log(maxArmWidth));

            bodyControllerViz.transform.localScale = new Vector3(0.2f, handWidth.magnitude / 6, 0.2f);
            bodyControllerViz.transform.rotation = Quaternion.LookRotation(handWidth) * Quaternion.Euler(90, 0, 0);
            bodyControllerViz.transform.position = getCenterPosition(body);

            oscControl.SendOSC(_oscPaths[0], widthLog);
            oscControl.SendOSC(_oscPaths[1], angle / 180f);
        }
        if (controlType.ToLower() == "sphere")
        {
            float width = getHandWidth(body);
            maxArmWidth = Mathf.Max(width, maxArmWidth);
            float angle = getHandAngle(body);
            float depth = getHandDepthFromBody(body);
            maxHandDepth = Mathf.Max(depth, maxHandDepth);

            //Debug.Log("distance: " + width);
            //Debug.Log("maxArmWidth: " + maxArmWidth);
            //Debug.Log("volume: " + width / maxArmWidth);
            //Debug.Log("depth: " + depth / maxHandDepth);

            float widthLog = (Mathf.Log(width)) / (Mathf.Log(maxArmWidth));
            float depthLog = (Mathf.Log(depth)) / (Mathf.Log(maxHandDepth));

            bodyControllerViz.transform.localScale = new Vector3(width / 3, width / 3, width / 3);
            bodyControllerViz.transform.rotation = Quaternion.LookRotation(GetVector3FromJoint(body.Joints[Kinect.JointType.HandRight]) - GetVector3FromJoint(body.Joints[Kinect.JointType.HandLeft]));
            bodyControllerViz.transform.position = getCenterPosition(body);

            oscControl.SendOSC(_oscPaths[0], widthLog);
            oscControl.SendOSC(_oscPaths[1], angle / 180f);
            oscControl.SendOSC(_oscPaths[2], depthLog);
        }
    }

    /// <summary>
    /// Set OSC Paths used by controllers
    ///
    /// </summary>
    /// <param name="oscPaths"></param>
    public void SetOSCPaths(List<string> oscPaths)
    {

        Debug.Log("OSC Paths before:" + _oscPaths.Count);
        _oscPaths.Clear();
        foreach (string path in oscPaths)
        {
            _oscPaths.Add(path);
        }
        Debug.Log("OSC Paths after:" + _oscPaths.Count);
    }

    Vector3 getCenterPosition(Kinect.Body body)
    {
        Vector3 midPoint = ((GetVector3FromJoint(body.Joints[Kinect.JointType.HandRight]) - GetVector3FromJoint(body.Joints[Kinect.JointType.HandLeft])) * 0.5f) + GetVector3FromJoint(body.Joints[Kinect.JointType.HandLeft]);
        return midPoint;
    }

    /// <summary>
    /// get the distance between the left and right hand
    /// </summary>
    /// <param name="body"></param>
    /// <returns>float between left and right hand</returns>
    float getHandWidth(Kinect.Body body)
    {
        Vector3 handWidth = GetVector3FromJoint(body.Joints[Kinect.JointType.HandRight]) - GetVector3FromJoint(body.Joints[Kinect.JointType.HandLeft]);
        return handWidth.magnitude;
    }

    /// <summary>
    /// get the angle of the line between left and right hands relative to floor
    /// </summary>
    /// <param name="handWidth"></param>
    /// <returns>float</returns>
    float getHandAngle(Kinect.Body body)
    {
        Vector3 handWidth = GetVector3FromJoint(body.Joints[Kinect.JointType.HandRight]) - GetVector3FromJoint(body.Joints[Kinect.JointType.HandLeft]);
        float angle = Vector3.Angle(handWidth, transform.right);
        return angle;
    }

    float getHandDepthFromBody(Kinect.Body body)
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
        controlType = ct;

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

    private static Vector3 GetVector3FromJoint(Kinect.Joint joint)
    {
        return new Vector3(joint.Position.X * 10, joint.Position.Y * 10, joint.Position.Z * 10);
    }
}
