using System.Linq;
using System.Collections.Generic;
using UnityEngine;

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
    }

    private bool CanReachTarget()
    {
        float distance  = (target.position - rootPosition).magnitude;
        return distance <= GetTotalBodyLength();
    }

    private float GetTotalBodyLength()
    {
        GetAllBoneChildren(this.transform.parent); // Populates allBones list

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
}
