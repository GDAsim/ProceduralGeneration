using System.Collections.Generic;
using UnityEngine;
using static BSplinePoint;

public class BSplinesManager : MonoBehaviour
{
    [HideInInspector] public BSpline bSpline = new();

    [Header("Gizmo Settings")]  
    public bool showGizmoPath = true;
    public float pointsSize = 0.1f;
    public Color pointsColor = Color.yellow;
    public Color pathColor = Color.cyan;

    [Header("Editor Settings")]
    [SerializeField] bool editInHierachy = true;
    [SerializeField] Color bSplinePointTextColor = Color.cyan;
    [SerializeField] Color lengthTextColor = Color.cyan;
    [SerializeField, HideInInspector] List<GameObject> bSplinePointsGOs = new();

    void OnDrawGizmos()
    {
        // Draw Points
        //for (int i = 0; BSplinePoints.Count > i; i++)
        //{
        //    var bSplinePoint = BSplinePoints[i];
        //    Gizmos.DrawSphere(bSplinePoint.Position, 0.2f);
        //}
    }

    public void AddPoint()
    {
        bSpline.AddPoint(bSpline.BSplinePoints.Count);
    }
    public void AddPointAt(int index)
    {
        bSpline.AddPoint(index);
    }
    public void RemovePoint()
    {
        bSpline.AddPoint(bSpline.BSplinePoints.Count - 1);
    }
    public void RemovePointAt(int index)
    {
        bSpline.RemovePoint(index);
    }
}
