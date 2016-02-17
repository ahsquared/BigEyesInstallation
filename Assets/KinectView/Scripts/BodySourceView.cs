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

    private Dictionary<ulong, GameObject> _Bodies = new Dictionary<ulong, GameObject>();
    private BodySourceManager _BodyManager;

    private string controlType = "none";
    private GameObject trail;

    private float maxArmWidth = 0f;

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

        for (Kinect.JointType jt = Kinect.JointType.SpineBase; jt <= Kinect.JointType.ThumbRight; jt++)
        {
            GameObject jointObj = GameObject.CreatePrimitive(PrimitiveType.Cube);

            LineRenderer lr = jointObj.AddComponent<LineRenderer>();
            lr.SetVertexCount(2);
            lr.material = BoneMaterial;
            lr.SetWidth(0.05f, .05f);

            jointObj.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
            jointObj.name = jt.ToString();
            jointObj.transform.parent = body.transform;
            //if (jt == Kinect.JointType.HandLeft)
            //{
            //    jointObj.GetComponent<Collider>().isTrigger = true;
            //    LineRenderer doughLine = jointObj.AddComponent<LineRenderer>();
            //    doughLine.SetVertexCount(2);
            //    doughLine.material = BoneMaterial;
            //    doughLine.SetWidth(0.05f, .05f);
            //}
        }
        trail = GameObject.Find("Trail");
        trail.GetComponent<TrailRenderer>().enabled = false;
        return body;
    }

    private void RefreshBodyObject(Kinect.Body body, GameObject bodyObject)
    {
        for (Kinect.JointType jt = Kinect.JointType.SpineBase; jt <= Kinect.JointType.ThumbRight; jt++)
        {
            Kinect.Joint sourceJoint = body.Joints[jt];
            Kinect.Joint? targetJoint = null;

            if (_BoneMap.ContainsKey(jt))
            {
                targetJoint = body.Joints[_BoneMap[jt]];
            }

            Transform jointObj = bodyObject.transform.FindChild(jt.ToString());
            jointObj.localPosition = GetVector3FromJoint(sourceJoint);

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

        if (controlType == "trail")
        {
            var pos = GetVector3FromJoint(body.Joints[Kinect.JointType.HandLeft]);
            trail.transform.position = pos;

        }
        if (controlType.ToLower() == "dough")
        {
            Vector3 offset = GetVector3FromJoint(body.Joints[Kinect.JointType.HandRight]) - GetVector3FromJoint(body.Joints[Kinect.JointType.HandLeft]);
            float distance = offset.sqrMagnitude;
            maxArmWidth = Mathf.Max(distance, maxArmWidth);
            float angle = Vector3.Angle(offset, transform.right);
            Debug.Log("distance: " + distance / 100f);
            //Debug.Log("angle: " + angle);
            //Transform jointHandLeft = bodyObject.transform.FindChild(Kinect.JointType.HandLeft.ToString());
            //Transform jointHandRight = bodyObject.transform.FindChild(Kinect.JointType.HandRight.ToString());
            //LineRenderer doughLine = jointHandLeft.GetComponent<LineRenderer>();
            //doughLine.SetPosition(0, jointHandLeft.localPosition);
            //doughLine.SetPosition(1, GetVector3FromJoint(body.Joints[Kinect.JointType.HandRight]));
            //doughLine.SetColors(GetColorForState(body.Joints[Kinect.JointType.HandLeft].TrackingState), GetColorForState(body.Joints[Kinect.JointType.HandRight].TrackingState));

            oscControl.SendOSC("/clank/volume", distance / maxArmWidth);
            oscControl.SendOSC("/clank/reverb", angle / 180f);
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
