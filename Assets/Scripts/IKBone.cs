using UnityEngine;

public class IKBone : MonoBehaviour
{
    [Header("IK Bone")]
    public float boneLength;
    public Vector3 jointPosition;

    protected virtual void Awake()
    {
        boneLength = this.GetComponent<BoxCollider>().bounds.size.x;
        jointPosition = GetJointPosition();
    }

    protected Vector3 GetJointPosition()
    {
        Vector3 jointPos = this.transform.position - (transform.right * (boneLength / 2));
        return jointPos;
    }
}
