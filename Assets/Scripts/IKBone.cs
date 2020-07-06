using UnityEngine;

public class IKBone : MonoBehaviour
{
    [Header("IK Bone")]
    public float boneLength;
    public Transform joint;

    protected virtual void Awake()
    {
        boneLength = this.GetComponent<BoxCollider>().bounds.size.x;
        joint = this.transform.parent;
    }

    public void UpdateJointPosition(Vector3 newPos)
    {
        joint.position = newPos;
    }

    public void UpdateJointRotation(Vector3 targetPos)
    {
        joint.LookAt(targetPos);
        joint.RotateAround(joint.position, joint.up, -90);
    }

    private float AngleInRad(Vector3 vec1, Vector3 vec2)
    {
        return Mathf.Atan2(vec2.y - vec1.y, vec2.x - vec1.x);
    }

    private float AngleInDeg(Vector3 vec1, Vector3 vec2)
    {
        return AngleInRad(vec1, vec2) * 180 / Mathf.PI;
    }
}
