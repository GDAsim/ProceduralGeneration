using System.Collections.Generic;
using UnityEngine;

public class Splines_Example : MonoBehaviour
{
    [SerializeField] ControlPointsManager cpManager;

    [SerializeField] int LineSegmentCount = 100;

    public enum Select
    {
        Quadratic,
        Cubic,
    }
    public Select select = Select.Quadratic;

    void OnDrawGizmos()
    {
        if (LineSegmentCount < 2) return;

        if (!cpManager) return;

        switch (select)
        {
            case Select.Quadratic:
                DrawQuad(Color.white);
                break;
            case Select.Cubic:
                DrawCubic(Color.white);
                break;
        }
    }
    void DrawQuad(Color color)
    {
        Gizmos.color = color;

        
    }
    void DrawCubic(Color color)
    {
        Gizmos.color = color;

        var p0 = cpManager.ControlPoints[0];
        for (int i = 1; i <= LineSegmentCount; i++)
        {
            var segmentCount = (cpManager.ControlPoints.Count - 1) / 3;
            var t = (float)i / LineSegmentCount * segmentCount; // 0 - 2

            var currentSegment = Mathf.CeilToInt(t) - 1;

            var i3 = currentSegment * 3;
            var segmentP0 = cpManager.ControlPoints[0 + i3];
            var segmentP1 = cpManager.ControlPoints[1 + i3];
            var segmentP2 = cpManager.ControlPoints[2 + i3];
            var segmentP3 = cpManager.ControlPoints[3 + i3];

            var ss = new List<Vector3>() { segmentP0, segmentP1, segmentP2, segmentP3 };
            var p1 = Bezier.CalculateCubicCurve(ss, t - currentSegment);

            Gizmos.DrawLine(p0, p1);

            p0 = p1;
        }
    }
}
