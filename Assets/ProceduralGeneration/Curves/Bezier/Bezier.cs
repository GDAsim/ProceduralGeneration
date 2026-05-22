using System;
using System.Collections.Generic;
using UnityEngine;

public class Bezier
{
    /// <summary>
    /// Returns Linear Lerp based on first and last control point
    /// </summary>
    public static Vector3 CalculateLinearCurve(List<Vector3> controlPoints, float t)
    {
        if (controlPoints == null || controlPoints.Count < 2)
        {
            throw new ArgumentException("Linear curve requires at least 2 control points.");
        }

        var p0 = controlPoints[0];
        var p1 = controlPoints[controlPoints.Count - 1];

        return (1 - t) * p0 + t * p1;
    }

    /// <summary>
    /// Returns Linear Lerp based on first and last control point
    /// Uses an alternate Lerp method
    /// </summary>
    public static Vector3 CalculateLinearCurve2(List<Vector3> controlPoints, float t)
    {
        if (controlPoints == null || controlPoints.Count < 2)
        {
            throw new ArgumentException("Linear curve requires at least 2 control points.");
        }

        var p0 = controlPoints[0];
        var p1 = controlPoints[controlPoints.Count - 1];

        return p0 + t * (p1 - p0);
    }

    /// <summary>
    /// Returns Quadratic curve based on first, mid and last control point
    /// Uses Bernstein polynomial formula
    /// </summary>
    public static Vector3 CalculateQuadraticCurve(List<Vector3> controlPoints, float t)
    {
        if (controlPoints == null || controlPoints.Count < 3)
        {
            throw new ArgumentException("Quadratic curve requires at least 3 control points.");
        }

        var p0 = controlPoints[0];
        var p1 = controlPoints[controlPoints.Count / 2];
        var p2 = controlPoints[controlPoints.Count - 1];

        float u = 1 - t;
        return (u * u) * p0 + (2 * u * t) * p1 + (t * t) * p2;
    }

    /// <summary>
    /// Returns Quadratic curve based on first, mid and last control point
    /// Uses multiple lerp, De Casteljau construction
    /// </summary>
    public static Vector3 CalculateQuadraticCurve2(List<Vector3> controlPoints, float t)
    {
        if (controlPoints == null || controlPoints.Count < 3)
        {
            throw new ArgumentException("Quadratic curve requires at least 3 control points.");
        }

        var p0 = controlPoints[0];
        var p1 = controlPoints[controlPoints.Count / 2];
        var p2 = controlPoints[controlPoints.Count - 1];

        var p0p1 = (1 - t) * p0 + t * p1;
        var p1p2 = (1 - t) * p1 + t * p2;

        var p0p1p2 = (1 - t) * p0p1 + t * p1p2;

        return p0p1p2;
    }

    /// <summary>
    /// Returns Quadratic curve based on first, mid and last control point
    /// Uses Bernstein polynomial formula
    /// </summary>
    public static Vector3 CalculateCubicCurve(List<Vector3> controlPoints, float t)
    {
        if (controlPoints == null || controlPoints.Count < 4)
        {
            throw new ArgumentException("Cubic curve requires at least 4 control points.");
        }

        var p0 = controlPoints[0];
        var p1 = controlPoints[1];
        var p2 = controlPoints[controlPoints.Count - 2];
        var p3 = controlPoints[controlPoints.Count - 1];

        float u = 1 - t;
        float uu = u * u;
        float tt = t * t;

        return (uu * u) * p0 + (3 * uu * t) * p1 + (3 * u * tt) * p2 + (tt * t) * p3;
    }

    /// <summary>
    /// Returns Quadratic curve based on first, mid and last control point
    /// Uses multiple lerp, De Casteljau construction
    /// </summary>
    public static Vector3 CalculateCubicCurve2(List<Vector3> controlPoints, float t)
    {
        if (controlPoints == null || controlPoints.Count < 4)
        {
            throw new ArgumentException("Cubic curve requires at least 4 control points.");
        }

        var p0 = controlPoints[0];
        var p1 = controlPoints[1];
        var p2 = controlPoints[controlPoints.Count - 2];
        var p3 = controlPoints[controlPoints.Count - 1];

        var p0p1 = (1 - t) * p0 + t * p1;
        var p1p2 = (1 - t) * p1 + t * p2;
        var p2p3 = (1 - t) * p2 + t * p3;

        var p0p1p2 = (1 - t) * p0p1 + t * p1p2;
        var p2p3p4 = (1 - t) * p1p2 + t * p2p3;

        var p1p2p3p4 = (1 - t) * p0p1p2 + t * p2p3p4;

        return p1p2p3p4;
    }
}
