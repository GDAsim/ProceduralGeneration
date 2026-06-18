using System;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

[CustomEditor(typeof(BSplinesManager), false)]
public class BSplinesManagerEditor : Editor
{
    ReorderableList bSplinePoints;

    BSplinesManager bSplinesManager;

    const float PIXELTOWORLDSCALE = 80;

    GUIStyle style;

    void OnEnable()
    {
        Tools.hidden = true;

        bSplinesManager = (BSplinesManager)target;

        bSplinePoints = new ReorderableList(serializedObject, serializedObject.FindProperty("bSpline.BSplinePoints"), true, true, false, false);

        bSplinePoints.drawHeaderCallback = (Rect rect) =>
        {
            EditorGUI.LabelField(rect, string.Format("ControlPoints: {0}", bSplinePoints.count), EditorStyles.boldLabel);
        };
        bSplinePoints.drawElementCallback = DrawControlPointsList;

        style = new GUIStyle();
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

        if (GUILayout.Button("Add Point"))
        {
            AddPoint();
        }
        if (bSplinePoints.count > 0)
        {
            if (GUILayout.Button("Remove Point"))
            {
                RemovePoint();
            }
        }

        bSplinePoints.DoLayoutList();

        serializedObject.ApplyModifiedProperties();
    }

    void OnSceneGUI()
    {
        var pointsSize = serializedObject.FindProperty("pointsSize").floatValue;
        var pointsColor = serializedObject.FindProperty("pointsColor").colorValue;

        Handles.color = pointsColor;
        style.normal.textColor = serializedObject.FindProperty("bSplinePointTextColor").colorValue;
        for (int i = 0; i < bSplinePoints.count; i++)
        {
            var controlPointProp = bSplinePoints.serializedProperty.GetArrayElementAtIndex(i);
            var controlPointPosProp = controlPointProp.FindPropertyRelative("position");

            var pos = controlPointPosProp.vector3Value;
            var size = pointsSize;

            var content = new GUIContent($"BSpline Point {i}");
            var contentSize = GUI.skin.label.CalcSize(content);
            var pixelsToWorld = HandleUtility.GetHandleSize(pos) / PIXELTOWORLDSCALE;
            var worldWidth = contentSize.x * pixelsToWorld;
            Handles.Label(pos - new Vector3(worldWidth * 0.5f, HandleUtility.GetHandleSize(pos) * 0.2f, 0), content.text, style);

            controlPointPosProp.vector3Value = Handles.FreeMoveHandle(pos, size, Vector3.zero, Handles.RectangleHandleCap);
        }

        // Label
        //Handles.color = LabelColor;
        //Handles.Label(controlPoint.Position + new Vector3(0f, HandleUtility.GetHandleSize(bSplinePoint.Position) * 0.4f, 0f), bSplinePoint.gameObject.name);

        // Center


        //// Left
        //Handles.color = LineColor;
        //Handles.DrawLine(bSplinePoint.Position, bSplinePoint.LeftPos);

        //Handles.color = ControlCapColor;
        //capsize = ControlCapSize;
        //pos = bSplinePoint.LeftPos;
        //size = HandleUtility.GetHandleSize(bSplinePoint.LeftPos) * capsize;
        //var newLeftPos = Handles.FreeMoveHandle(pos, size, Vector3.zero, Handles.CircleHandleCap);
        //if (bSplinePoint.LeftPos != newLeftPos)
        //{
        //    Undo.RegisterCompleteObjectUndo(bSplinePoint, "Move Left Handle");
        //    bSplinePoint.LeftPos = newLeftPos;
        //}

        //// Right
        //Handles.color = LineColor;
        //Handles.DrawLine(bSplinePoint.Position, bSplinePoint.RightPos);

        //Handles.color = ControlCapColor;
        //capsize = ControlCapSize;
        //pos = bSplinePoint.RightPos;
        //size = HandleUtility.GetHandleSize(bSplinePoint.RightPos) * capsize;
        //var newRightPos = Handles.FreeMoveHandle(pos, size, Vector3.zero, Handles.CircleHandleCap);
        //if (bSplinePoint.RightPos != newRightPos)
        //{
        //    Undo.RegisterCompleteObjectUndo(bSplinePoint, "Move Right Handle");
        //    bSplinePoint.RightPos = newRightPos;
        //}


        bSplinePoints.serializedProperty.serializedObject.ApplyModifiedProperties();
    }

    void DrawControlPointsList(Rect rect, int index, bool isActive, bool isFocused)
    {
        var AddButtonWidth = 100;
        var RemoveButtonWidth = 25;

        var controlPoint = bSplinePoints.serializedProperty.GetArrayElementAtIndex(index);
        rect.y += 2;

        if (GUI.Button(new Rect(rect.x, rect.y, AddButtonWidth, EditorGUIUtility.singleLineHeight), new GUIContent("Add Before")))
        {
            AddPointAt(index);
        }

        EditorGUI.PropertyField(new Rect(rect.x + AddButtonWidth + 5f, rect.y, rect.width - AddButtonWidth * 2f - 35f, EditorGUIUtility.singleLineHeight), controlPoint.FindPropertyRelative("position"), GUIContent.none);

        if (GUI.Button(new Rect(rect.width - AddButtonWidth + 8f, rect.y, AddButtonWidth, EditorGUIUtility.singleLineHeight), new GUIContent("Add After")))
        {
            AddPointAt(index + 1);
        }

        if (GUI.Button(new Rect(rect.width + 14f, rect.y, RemoveButtonWidth, EditorGUIUtility.singleLineHeight), new GUIContent("x")))
        {
            RemovePointAt(index);
        }
    }

    void AddPoint()
    {
        bSplinesManager.AddPoint();
    }
    void AddPointAt(int index)
    {
        bSplinesManager.AddPoint(index);
    }
    void RemovePoint()
    {
        bSplinesManager.RemovePoint();
    }
    void RemovePointAt(int index)
    {
        bSplinesManager.RemovePoint(index);
    }
}