using System;
using System.Collections.Generic;
using UnityEngine;
using static BSplinePoint;

[Serializable]
public class BSpline
{
    public List<BSplinePoint> bSplinePoints { get; } = new();

    public BSplinePoint AddPoint(int index)
    {
        if (index < 0 || index > bSplinePoints.Count) throw new ArgumentOutOfRangeException(nameof(index));

        BSplinePoint newPoint;

        if (bSplinePoints.Count == 0)
        {
            newPoint = new BSplinePoint(HandleType.Aligned, Vector3.zero,
                new Vector3(-1, 0, 0), new Vector3(1, 0, 0));
        }
        else if (index == 0)
        {
            newPoint = CreateEndPoint(bSplinePoints[0], -1f);
        }
        else if (index == bSplinePoints.Count)
        {
            newPoint = CreateEndPoint(bSplinePoints[^1], 1f);
        }
        else
        {
            newPoint = SplitSegment(bSplinePoints[index - 1], bSplinePoints[index]);
        }

        bSplinePoints.Insert(index, newPoint);

        return newPoint;
    }

    // New end point placed one step beyond the current end, in the end's tangent direction
    BSplinePoint CreateEndPoint(BSplinePoint end, float direction)
    {
        Vector3 tangent = direction > 0 ? end.RightControlLocalPosition : end.LeftControlLocalPosition;
        Vector3 step = tangent.sqrMagnitude > 1e-6f
            ? Vector3.Normalize(tangent) * 3f * Math.Abs(direction)
            : new Vector3(direction * 3f, 0, 0);

        Vector3 handle = step / 3f; // local offset, a third of the step
        return new BSplinePoint(HandleType.Aligned, end.Position + step, -handle, handle);
    }

    // Splits the cubic Bezier between a and b at t = 0.5 and returns the new point.
    // The neighbors' handles are shortened so the curve shape stays the same.
    BSplinePoint SplitSegment(BSplinePoint a, BSplinePoint b)
    {
        Vector3 p0 = a.Position;
        Vector3 p1 = a.RightPos;
        Vector3 p2 = b.LeftPos;
        Vector3 p3 = b.Position;

        Vector3 q0 = (p0 + p1) * 0.5f;
        Vector3 q1 = (p1 + p2) * 0.5f;
        Vector3 q2 = (p2 + p3) * 0.5f;
        Vector3 r0 = (q0 + q1) * 0.5f;
        Vector3 r1 = (q1 + q2) * 0.5f;
        Vector3 s = (r0 + r1) * 0.5f;

        a.RightPos = q0;
        b.LeftPos = q2;

        return new BSplinePoint(HandleType.Aligned, s, r0 - s, r1 - s);
    }

    public void RemovePoint(int index)
    {
        //if (index < 0 || index >= bSplinePoints.Count)
        //    return false;
        //if (bSplinePoints.Count <= MinPoints)
        //    return false; // keep at least a valid curve

        //bSplinePoints.RemoveAt(index);
        //return true;
    }
}
