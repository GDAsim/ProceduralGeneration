using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

[CustomEditor(typeof(ControlPointsManager), false)]
public class ControlPointsManagerEditor : Editor
{
    ReorderableList controlPoints;

    void OnEnable()
    {
        Tools.hidden = true;

        controlPoints = new ReorderableList(serializedObject, serializedObject.FindProperty("ControlPoints"), true, true, false, false);
        controlPoints.drawHeaderCallback = (Rect rect) =>
        {
               EditorGUI.LabelField(rect, string.Format("ControlPoints: {0}", controlPoints.serializedProperty.arraySize), EditorStyles.boldLabel);
        };
        controlPoints.drawElementCallback = DrawControlPointsList;
    }
    void OnDisable()
    {
        Tools.hidden = false;
    }
    void OnSceneGUI()
    {
        var isEditInHeirachy = serializedObject.FindProperty("editInHierachy");
        if (isEditInHeirachy.boolValue) return;

        var pointsSize = serializedObject.FindProperty("pointsSize").floatValue;
        var pointsColor = serializedObject.FindProperty("pointsColor").colorValue;
        Handles.color = pointsColor;
        for (int i = 0; i < controlPoints.count; i++)
        {
            var controlPointProp = controlPoints.serializedProperty.GetArrayElementAtIndex(i);
            var controlPoint = controlPointProp.vector3Value;

            var pos = controlPoint;
            var size = pointsSize;

            Handles.Label(pos + new Vector3(0f, HandleUtility.GetHandleSize(pos) * 0.4f, 0f), $"Control Point {i}");
            controlPointProp.vector3Value = Handles.FreeMoveHandle(pos, size, Vector3.zero, Handles.CircleHandleCap);
        }
        controlPoints.serializedProperty.serializedObject.ApplyModifiedProperties();
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        serializedObject.Update();

        if (GUILayout.Button("Add Point"))
        {
            AddControlPoint();
        }

        controlPoints.DoLayoutList();

        serializedObject.ApplyModifiedProperties();
    }

    void DrawControlPointsList(Rect rect, int index, bool isActive, bool isFocused)
    {
        var AddButtonWidth = 100;
        var RemoveButtonWidth = 100;

        var controlPoint = controlPoints.serializedProperty.GetArrayElementAtIndex(index);
        rect.y += 2;

        if (GUI.Button(new Rect(rect.x, rect.y, AddButtonWidth, EditorGUIUtility.singleLineHeight), new GUIContent("Add Before")))
        {
            AddControlPointAt(index);
        }

        EditorGUI.PropertyField(new Rect(rect.x + AddButtonWidth + 5f, rect.y, rect.width - AddButtonWidth * 2f - 35f, EditorGUIUtility.singleLineHeight), controlPoint, GUIContent.none);

        if (GUI.Button(new Rect(rect.width - AddButtonWidth + 8f, rect.y, AddButtonWidth, EditorGUIUtility.singleLineHeight), new GUIContent("Add After")))
        {
            AddControlPointAt(index + 1);
        }

        //if (this.curve.KeyPointsCount > 2)
        //{
        //    if (GUI.Button(new Rect(rect.width + 14f, rect.y, RemoveButtonWidth, EditorGUIUtility.singleLineHeight), new GUIContent("x")))
        //    {
        //        //RemoveKeyPointAt(this.curve, index);
        //    }
        //}
    }

    void AddControlPoint()
    {
        var controlPointManager = (ControlPointsManager)target;
        controlPointManager.AddControlPoint();
    }
    void AddControlPointAt(int index)
    {
        var controlPointManager = (ControlPointsManager)target;
        controlPointManager.AddControlPoint(index);
    }

}