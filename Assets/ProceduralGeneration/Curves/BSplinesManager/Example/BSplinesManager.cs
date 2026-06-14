using System.Collections.Generic;
using UnityEngine;

public class BSplinesManager : MonoBehaviour
{
    public List<BSplinePoint> BSplinePoints = new();

    void OnDrawGizmos()
    {
        // Draw Points
        //for (int i = 0; BSplinePoints.Count > i; i++)
        //{
        //    var bSplinePoint = BSplinePoints[i];
        //    Gizmos.DrawSphere(bSplinePoint.Position, 0.2f);
        //}
    }
}
