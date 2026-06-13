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

    Vector3 leftControlPos;
    Vector3 rightControlPos;

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
    public Vector3 LeftControlPosition
    {
        get
        {
            return leftControlPos;
        }
        set
        {
            leftControlPos = value;

            if (this.handleType == HandleType.Mirror)
            {
                rightControlPos = -value;
            }
        }
    }
    public Vector3 RightControlPosition
    {
        get
        {
            return rightControlPos;
        }
        set
        {
            rightControlPos = value;

            if (this.handleType == HandleType.Mirror)
            {
                leftControlPos = -value;
            }
        }
    }
    public Vector3 LeftPos
    {
        get
        {
            return transform.TransformPoint(LeftControlPosition);
        }
        set
        {
            LeftControlPosition = transform.InverseTransformPoint(value);
        }
    }
    public Vector3 RightPos
    {
        get
        {
            return transform.TransformPoint(RightControlPosition);
        }
        set
        {
            RightControlPosition = transform.InverseTransformPoint(value);
        }
    }
}