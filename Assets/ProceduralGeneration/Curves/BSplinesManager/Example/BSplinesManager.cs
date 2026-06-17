using UnityEngine;

public class BSplinesManager : MonoBehaviour
{
    [HideInInspector] public BSpline bSpline = new();

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
