using UnityEngine;

// The gameObject which holds this controller will be the root.
public class IKController : IKBone
{
    [Header("IK Controller")]
    public Vector3 rootPosition; // also known as p0
    public Transform target;

    protected override void Awake()
    {
        base.Awake();
        rootPosition = jointPosition;
        Debug.LogFormat("You {0} reach the target, currently.", CanReachTarget() ? "can" : "cannot");
    }

    private bool CanReachTarget()
    {
        float distance  = (target.position - rootPosition).magnitude;
        return distance <= boneLength;
    }
}
