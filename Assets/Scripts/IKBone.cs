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

    public void UpdateJointPosition(Vector3 newPos)
    {
        this.transform.parent.position = newPos;
    }

    public void UpdateJointRotation(Vector3 targetPos)
    {
        transform.parent.LookAt(targetPos);
        transform.parent.RotateAround(transform.parent.position, transform.parent.up, -90);

        /* Old rotation method
        float angle = 180 + AngleInDeg(targetPos, transform.parent.position);        
        Vector3 desiredRot = new Vector3(transform.parent.rotation.x, transform.parent.rotation.y, angle);
        transform.parent.rotation = Quaternion.Euler(desiredRot);
        */
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
