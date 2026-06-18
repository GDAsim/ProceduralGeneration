using System;
using UnityEngine;

[Serializable]
public class BSplinePoint
{
    public enum HandleType
    {
        Mirror, // Same Distance, Opposite Side
        Aligned, // Different Distance, Opposite Side
        Join, // Same Distance, Same Side
        Free
    }

    public BSplinePoint(HandleType handleType, Vector3 position,
        Vector3 leftControlLocalPos, Vector3 rightControlLocalPos)
    {
        this.handleType = handleType;
        this.position = position;
        this.leftControlLocalPos = leftControlLocalPos;
        this.rightControlLocalPos = rightControlLocalPos;
    }

    HandleType handleType;
    [SerializeField] Vector3 position;
    Vector3 leftControlLocalPos;
    Vector3 rightControlLocalPos;

    public Vector3 Position
    {
        get
        {
            return position;
        }
        set
        {
            position = value;
        }
    }
    public Vector3 LeftControlLocalPosition
    {
        get
        {
            return leftControlLocalPos;
        }
        private set
        {
            leftControlLocalPos = value;

            if (this.handleType == HandleType.Mirror)
            {
                rightControlLocalPos = -value;
            }
        }
    }
    public Vector3 RightControlLocalPosition
    {
        get
        {
            return rightControlLocalPos;
        }
        private set
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
            return position + LeftControlLocalPosition;
        }
        set
        {
            LeftControlLocalPosition = value - position;
        }
    }
    public Vector3 RightPos
    {
        get
        {
            return position + RightControlLocalPosition;
        }
        set
        {
            RightControlLocalPosition = value - position;
        }
    }
}