using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System;

// The gameObject which holds this controller will be the root.
public class IKController : IKBone
{
    [Header("IK Controller")]
    public Vector3 rootPosition; // also known as p0
    public Transform target;
    public List<IKBone> allBones;
    public float marginOfError;
    public bool reachedTarget;
    private Vector3 prevTargetPos;

    [Header("Debug Settings")]
    public bool debug;
    public bool debugLines;

    public List<Vector3> forwardReachPositions = new List<Vector3>();

    protected override void Awake()
    {
        base.Awake();
        rootPosition = jointPosition;
        reachedTarget = false;
        prevTargetPos = target.position;
    }

    private void Start()
    {
        Debug.LogFormat("Total length of body is: {0}", GetTotalBodyLength());
        Debug.LogFormat("You {0} reach the target, currently.", CanReachTarget() ? "can" : "cannot");

        if (CanReachTarget() && !reachedTarget) ForwardReach();
        else StretchTowardTarget();
    }

    private void Update()
    {
        if (CanReachTarget())
        {
            if (!reachedTarget || target.position != prevTargetPos)
            {
                ForwardReach();
                prevTargetPos = target.position;
            }
            else
            {
                Debug.Log("Target has not moved. Or we have reached the target");
            }
        }
        else
        {
            StretchTowardTarget();
        }
    }

    private bool CanReachTarget()
    {
        float distance  = (target.position - rootPosition).magnitude;
        return distance <= GetTotalBodyLength();
    }

    private float GetTotalBodyLength()
    {
        // Populate allBones list
        if (allBones.Count == 0) GetAllBoneChildren(this.transform.parent);

        float length = 0;
        allBones.ForEach(bone => length += bone.boneLength);
        return length;
    }

    private void GetAllBoneChildren(Transform parent)
    {
        if (!parent) return;

        foreach (Transform child in parent)
        {
            if (!child) continue;

            IKBone childBone = child.GetComponent<IKBone>();
            if (childBone) allBones.Add(childBone);

            GetAllBoneChildren(child);
        }
    }

    private void StretchTowardTarget()
    {
        allBones[0].UpdateJointRotation(target.position);

        for (int i = 1; i < allBones.Count; i++)
        {
            IKBone boneOfInterest = allBones[i];
            
            // the below line won't work for bones of varying length.
            boneOfInterest.transform.parent.localPosition = Vector3.right * i * boneOfInterest.boneLength;
            boneOfInterest.transform.parent.localRotation = Quaternion.Euler(0, 0, 0);
        }
    }

    private void ForwardReach()
    {
        if (debug) Debug.Log("FORWARD REACHING...");
        Vector3 desiredJointPos = target.position;
        forwardReachPositions.Clear();

        for (int i = allBones.Count - 1; i >= 0; i--)
        {
            IKBone boneOfInterest = allBones[i];
            forwardReachPositions.Add(desiredJointPos);

            Vector3 firstJoint = boneOfInterest.jointPosition;
            Vector3 lastJoint = desiredJointPos;

            if (debug) Debug.Log("First Joint: " + firstJoint);
            if (debug) Debug.Log("Last Joint: " + lastJoint);
            //Debug.DrawLine(firstJoint, lastJoint, Color.magenta, 1000);

            Vector3 unitDir = (firstJoint - lastJoint).normalized;
            if (debug) Debug.Log("Unit Direction: " + unitDir);

            desiredJointPos = lastJoint + (unitDir * boneOfInterest.boneLength);
            if (debug) Debug.Log("New End Point: " + desiredJointPos);

            if (debugLines) Debug.DrawLine(lastJoint, desiredJointPos, Color.yellow, Time.deltaTime);
        }

        // if we didn't reach the start joint/root...
        BackwardReach();
    }

    private void BackwardReach()
    {
        if (debug) Debug.Log("BACKWARD REACHING...");
        forwardReachPositions.Reverse();
        Vector3 desiredJointStart = rootPosition;

        for (int i = 0; i < allBones.Count; i++)
        {
            IKBone boneOfInterest = allBones[i];

            Vector3 firstJoint = desiredJointStart;
            Vector3 lastJoint = forwardReachPositions[i];

            if (debug) Debug.Log("First Joint: " + firstJoint);
            if (debug) Debug.Log("Last Joint: " + lastJoint);

            Vector3 unitDir = (lastJoint - firstJoint).normalized;
            if (debug) Debug.Log("Unit Direction: " + unitDir);

            lastJoint = firstJoint + (unitDir * boneOfInterest.boneLength);
            if (debug) Debug.Log("New End Point: " + lastJoint);

            if (debugLines) Debug.DrawLine(firstJoint, lastJoint, Color.green, Time.deltaTime);

            desiredJointStart = lastJoint;

            boneOfInterest.UpdateJointRotation(lastJoint);
            boneOfInterest.UpdateJointPosition(firstJoint);
        }

        if (CalculateMarginOfError(forwardReachPositions[allBones.Count - 1], target.position) < marginOfError)
        {
            reachedTarget = true;
        }
    }

    private float CalculateMarginOfError(Vector3 lastJoint, Vector3 goal)
    {
        return (lastJoint - goal).magnitude;
    }
}
