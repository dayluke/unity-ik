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

    protected override void Awake()
    {
        base.Awake();
        rootPosition = jointPosition;
    }

    private void Start()
    {
        Debug.LogFormat("Total length of body is: {0}", GetTotalBodyLength());
        Debug.LogFormat("You {0} reach the target, currently.", CanReachTarget() ? "can" : "cannot");

        if (CanReachTarget()) ForwardReach();


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

    private void ForwardReach()
    {
        Vector3 desiredJointPos = target.position;

        for (int i = allBones.Count - 1; i >= 0; i--)
        {
            IKBone boneOfInterest = allBones[i];

            Vector3 firstJoint = boneOfInterest.jointPosition;
            Vector3 lastJoint = desiredJointPos;

            Debug.Log("First Joint: " + firstJoint);
            Debug.Log("Last Joint: " + lastJoint);
            //Debug.DrawLine(firstJoint, lastJoint, Color.magenta, 1000);

            Vector3 unitDir = (firstJoint - lastJoint).normalized;
            Debug.Log("Unit Direction: " + unitDir);

            desiredJointPos = lastJoint + (unitDir * boneOfInterest.boneLength);
            Debug.Log("New End Point: " + desiredJointPos);

            Debug.DrawLine(lastJoint, desiredJointPos, Color.yellow, 1000);
        }

        // if we didn't reach the start joint/root...
        // BackwardReach();
    }
}
