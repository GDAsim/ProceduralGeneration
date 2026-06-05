using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ControlPointsManager), false)]
public class ControlPointsManagerEditor : Editor
{
    void OnEnable()
    {
        Tools.hidden = true;
    }
    void OnDisable()
    {
        Tools.hidden = false;
    }
    void OnSceneGUI()
    {
        // Draw path
        var pathColor = serializedObject.FindProperty("pathColor").colorValue;
        var showGizmoPathProp = serializedObject.FindProperty("showGizmoPath");
        if (showGizmoPathProp.boolValue)
        {
            var controlPointsProp = serializedObject.FindProperty("controlPoints");
            Handles.color = pathColor;
            var polyLines = new List<Vector3>();
            for (int i = 0; i < controlPointsProp.arraySize - 1; i++)
            {
                polyLines.Add(controlPointsProp.GetArrayElementAtIndex(i).vector3Value);

                Handles.DrawLine(controlPointsProp.GetArrayElementAtIndex(i).vector3Value, controlPointsProp.GetArrayElementAtIndex(i + 1).vector3Value);
            }
        }

        var isEditInHeirachy = serializedObject.FindProperty("editInHierachy");
        if (isEditInHeirachy.boolValue) return;

        var controlPointsGOsProp = serializedObject.FindProperty("controlPointsGOs");
        var pointsColor = serializedObject.FindProperty("pointsColor").colorValue;
        Handles.color = pointsColor;
        for (int i = 0; i < controlPointsGOsProp.arraySize; i++)
        {
            var controlPointGO = controlPointsGOsProp.GetArrayElementAtIndex(i);
            var go = controlPointGO?.objectReferenceValue as GameObject;
            if (go == null) return;

            var pos = go.transform.position;
            var size = HandleUtility.GetHandleSize(pos) * 0.1f;

            Handles.Label(pos + new Vector3(0f, HandleUtility.GetHandleSize(pos) * 0.4f, 0f), go.name);

            EditorGUI.BeginChangeCheck();
            var newPos = Handles.FreeMoveHandle(pos, size, Vector3.zero, Handles.CircleHandleCap);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(go.transform, "Move Object");
                go.transform.position = newPos;
            }
        }
    }
}