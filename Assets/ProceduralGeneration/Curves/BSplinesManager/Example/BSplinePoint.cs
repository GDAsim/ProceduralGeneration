using UnityEngine;

public class BSplinePoint : MonoBehaviour
{
    public enum HandleType
    {
        Mirror, // Same Distance, Opposite Side
        Aligned, // Different Distance, Opposite Side
        Join, // Same Distance, Same Side
        Free
    }

    [SerializeField] HandleType handleType = HandleType.Mirror;
    [SerializeField, HideInInspector] Vector3 leftControlLocalPos = new(-1, 0, 0);
    [SerializeField, HideInInspector] Vector3 rightControlLocalPos = new(1, 0, 0);

    public Vector3 Position
    {
        get
        {
            return this.transform.position;
        }
        set
        {
            this.transform.position = value;
        }
    }
    Vector3 LeftControlLocalPosition
    {
        get
        {
            return leftControlLocalPos;
        }
        set
        {
            leftControlLocalPos = value;

            if (this.handleType == HandleType.Mirror)
            {
                rightControlLocalPos = -value;
            }
        }
    }
    Vector3 RightControlLocalPosition
    {
        get
        {
            return rightControlLocalPos;
        }
        set
        {
            rightControlLocalPos = value;

            if (this.handleType == HandleType.Mirror)
            {
                leftControlLocalPos = -value;
            }
        }
    }
    public Vector3 LeftPos
    {
        get
        {
            return transform.TransformPoint(LeftControlLocalPosition);
        }
        set
        {
            LeftControlLocalPosition = transform.InverseTransformPoint(value);
        }
    }
    public Vector3 RightPos
    {
        get
        {
            return transform.TransformPoint(RightControlLocalPosition);
        }
        set
        {
            RightControlLocalPosition = transform.InverseTransformPoint(value);
        }
    }
}