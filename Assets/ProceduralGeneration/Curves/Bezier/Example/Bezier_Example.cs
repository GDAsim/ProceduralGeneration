using UnityEngine;

public class Bezier_Example : MonoBehaviour
{
    [SerializeField] ControlPointsManager cpManager;

    [SerializeField] int LineSegmentCount = 100;

    void OnDrawGizmos()
    {
        if (LineSegmentCount < 2) return;

        if (!cpManager) return;

        DrawLinear(Color.blue);

        DrawQuad(Color.red);
        DrawQuadString(Color.blue);

        DrawCubic(Color.black);
    }
    void DrawLinear(Color color)
    {
        Gizmos.color = color;

        var p0 = Bezier.CalculateLinearCurve(cpManager.ControlPoints, 0f);
        for (int i = 1; i <= LineSegmentCount; i++)
        {
            var t = (float)i / LineSegmentCount;
            var p1 = Bezier.CalculateLinearCurve(cpManager.ControlPoints, t);

            Gizmos.DrawLine(p0, p1);

            p0 = p1;
        }
    }
    void DrawQuad(Color color)
    {
        Gizmos.color = color;

        var p0 = Bezier.CalculateQuadraticCurve(cpManager.ControlPoints, 0f);
        for (int i = 1; i <= LineSegmentCount; i++)
        {
            var t = (float)i / LineSegmentCount;
            var p1 = Bezier.CalculateQuadraticCurve(cpManager.ControlPoints, t);

            Gizmos.DrawLine(p0, p1);

            p0 = p1;
        }
    }
    void DrawQuadString(Color color)
    {
        Gizmos.color = color;

        for (int i = 0; i <= LineSegmentCount; i++)
        {
            var t = (float)i / LineSegmentCount;

            var p0 = cpManager.ControlPoints[0];
            var p1 = cpManager.ControlPoints[cpManager.ControlPoints.Count / 2];
            var p2 = cpManager.ControlPoints[cpManager.ControlPoints.Count - 1];

            var p0p1 = (1 - t) * p0 + t * p1;
            var p1p2 = (1 - t) * p1 + t * p2;

            Gizmos.DrawLine(p0p1, p1p2);
        }
    }
    void DrawCubic(Color color)
    {
        Gizmos.color = color;

        var p0 = Bezier.CalculateCubicCurve2(cpManager.ControlPoints, 0f);
        for (int i = 1; i <= LineSegmentCount; i++)
        {
            var t = (float)i / LineSegmentCount;
            var p1 = Bezier.CalculateCubicCurve2(cpManager.ControlPoints, t);

            Gizmos.DrawLine(p0, p1);

            p0 = p1;
        }
    }
}
