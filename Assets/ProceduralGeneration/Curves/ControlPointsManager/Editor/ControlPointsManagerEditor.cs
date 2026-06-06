using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

[CustomEditor(typeof(ControlPointsManager), false)]
public class ControlPointsManagerEditor : Editor
{
    ReorderableList keyPoints;

    void OnEnable()
    {
        Tools.hidden = true;

        keyPoints = new ReorderableList(serializedObject, serializedObject.FindProperty("ControlPoints"), true, true, false, false);
        keyPoints.drawHeaderCallback = (Rect rect) =>
        {
               EditorGUI.LabelField(rect, string.Format("ControlPoints: {0}", keyPoints.serializedProperty.arraySize), EditorStyles.boldLabel);
        };
        keyPoints.drawElementCallback = DrawControlPointsList;
    }
    void OnDisable()
    {
        Tools.hidden = false;
    }
    void OnSceneGUI()
    {


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

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        serializedObject.Update();

        if (GUILayout.Button("Add Point"))
        {
            var controlPointManager = (ControlPointsManager)target;
            controlPointManager.AddControlPoint();
        }

        keyPoints.DoLayoutList();

        serializedObject.ApplyModifiedProperties();
    }

    void AddPoint()
    {
        AddPointAt(keyPoints.count);
    }
    void AddPointAt(int index)
    {
        var controlPointManager = (ControlPointsManager)target;
        controlPointManager.AddControlPoint();
    }

    void DrawControlPointsList(Rect rect, int index, bool isActive, bool isFocused)
    {
        var AddButtonWidth = 100;
        var RemoveButtonWidth = 100;

        var controlPoint = keyPoints.serializedProperty.GetArrayElementAtIndex(index);
        rect.y += 2;

        if (GUI.Button(new Rect(rect.x, rect.y, AddButtonWidth, EditorGUIUtility.singleLineHeight), new GUIContent("Add Before")))
        {
            //AddKeyPointAt(this.curve, index);
        }

        EditorGUI.PropertyField(new Rect(rect.x + AddButtonWidth + 5f, rect.y, rect.width - AddButtonWidth * 2f - 35f, EditorGUIUtility.singleLineHeight), controlPoint, GUIContent.none);

        if (GUI.Button(new Rect(rect.width - AddButtonWidth + 8f, rect.y, AddButtonWidth, EditorGUIUtility.singleLineHeight), new GUIContent("Add After")))
        {
            //AddKeyPointAt(this.curve, index + 1);
        }

        //if (this.curve.KeyPointsCount > 2)
        //{
        //    if (GUI.Button(new Rect(rect.width + 14f, rect.y, RemoveButtonWidth, EditorGUIUtility.singleLineHeight), new GUIContent("x")))
        //    {
        //        //RemoveKeyPointAt(this.curve, index);
        //    }
        //}
    }

}