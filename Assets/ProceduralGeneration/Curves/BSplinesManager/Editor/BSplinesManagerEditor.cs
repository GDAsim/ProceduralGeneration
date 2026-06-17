using Codice.CM.Client.Differences;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BSplinesManager), true)]
[CanEditMultipleObjects]
public class BSplinesManagerEditor : Editor
{
    //// Settings
    //static float CenterCapSize = 0.15f;
    //static float ControlCapSize = 0.15f;
    //static Color LabelColor = Color.white;
    //static Color CenterCapColor = Color.red;
    //static Color ControlCapColor = Color.yellow;
    //static Color LineColor = Color.white;

    BSplinesManager bSplinesManager;

    void OnEnable()
    {
        Tools.hidden = true;

        bSplinesManager = (BSplinesManager) target;
    }
    void OnDisable()
    {
        Tools.hidden = false;
    }
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        serializedObject.Update();

        var bSpline = bSplinesManager.bSpline;
        var bSplinePoints = bSpline.bSplinePoints;

        if (GUILayout.Button("Add Point"))
        {
            bSpline.AddPoint(0);
        }
        if (bSplinePoints.Count > 0)
        {
            if (GUILayout.Button("Remove Point"))
            {
                bSpline.RemovePoint(0);
            }
        }

        serializedObject.ApplyModifiedProperties();
    }

    //protected virtual void OnSceneGUI()
    //{
    //    var bSplinePoint = (BSplinePoint) target;

    //    // Label
    //    Handles.color = LabelColor;
    //    Handles.Label(bSplinePoint.Position + new Vector3(0f, HandleUtility.GetHandleSize(bSplinePoint.Position) * 0.4f, 0f), bSplinePoint.gameObject.name);

    //    // Center
    //    Handles.color = CenterCapColor;
    //    var capsize = CenterCapSize;
    //    var pos = bSplinePoint.Position;
    //    var size = HandleUtility.GetHandleSize(bSplinePoint.Position) * capsize;
    //    var newPointPosition = Handles.FreeMoveHandle(pos, size, Vector3.zero, Handles.RectangleHandleCap);

    //    if (bSplinePoint.Position != newPointPosition)
    //    {
    //        Undo.RegisterCompleteObjectUndo(bSplinePoint.transform, "Move Point");
    //        bSplinePoint.Position = newPointPosition;
    //    }

    //    // Left
    //    Handles.color = LineColor;
    //    Handles.DrawLine(bSplinePoint.Position, bSplinePoint.LeftPos);

    //    Handles.color = ControlCapColor;
    //    capsize = ControlCapSize;
    //    pos = bSplinePoint.LeftPos;
    //    size = HandleUtility.GetHandleSize(bSplinePoint.LeftPos) * capsize;
    //    var newLeftPos = Handles.FreeMoveHandle(pos, size, Vector3.zero, Handles.CircleHandleCap);
    //    if (bSplinePoint.LeftPos != newLeftPos)
    //    {
    //        Undo.RegisterCompleteObjectUndo(bSplinePoint, "Move Left Handle");
    //        bSplinePoint.LeftPos = newLeftPos;
    //    }

    //    // Right
    //    Handles.color = LineColor;
    //    Handles.DrawLine(bSplinePoint.Position, bSplinePoint.RightPos);

    //    Handles.color = ControlCapColor;
    //    capsize = ControlCapSize;
    //    pos = bSplinePoint.RightPos;
    //    size = HandleUtility.GetHandleSize(bSplinePoint.RightPos) * capsize;
    //    var newRightPos = Handles.FreeMoveHandle(pos, size, Vector3.zero, Handles.CircleHandleCap);
    //    if (bSplinePoint.RightPos != newRightPos)
    //    {
    //        Undo.RegisterCompleteObjectUndo(bSplinePoint, "Move Right Handle");
    //        bSplinePoint.RightPos = newRightPos;
    //    }
    //}
}