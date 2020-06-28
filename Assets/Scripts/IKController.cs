using UnityEngine;

// The gameObject which holds this controller will be the root.
public class IKController : IKBone
{
    [Header("IK Controller")]
    public Vector3 rootPosition; // also known as p0

    protected override void Awake()
    {
        base.Awake();
        rootPosition = jointPosition;
    }
}
