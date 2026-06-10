using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

[CustomEditor(typeof(ControlPointsManager), false)]
public class ControlPointsManagerEditor : Editor
{
    ReorderableList controlPoints;

    const float PIXELTOWORLDSCALE = 80;

    GUIStyle style;

    void OnEnable()
    {
        Tools.hidden = true;

        controlPoints = new ReorderableList(serializedObject, serializedObject.FindProperty("ControlPoints"), true, true, false, false);
        controlPoints.drawHeaderCallback = (Rect rect) =>
        {
            EditorGUI.LabelField(rect, string.Format("ControlPoints: {0}", controlPoints.serializedProperty.arraySize), EditorStyles.boldLabel);
        };
        controlPoints.drawElementCallback = DrawControlPointsList;

        style = new GUIStyle();
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
        style.normal.textColor = serializedObject.FindProperty("controlPointTextColor").colorValue;
        for (int i = 0; i < controlPoints.count; i++)
        {
            var controlPointProp = controlPoints.serializedProperty.GetArrayElementAtIndex(i);
            var controlPoint = controlPointProp.vector3Value;

            var pos = controlPoint;
            var size = pointsSize;

            var content = new GUIContent($"Control Point {i}");
            var contentSize = GUI.skin.label.CalcSize(content);
            var pixelsToWorld = HandleUtility.GetHandleSize(pos) / PIXELTOWORLDSCALE;
            var worldWidth = contentSize.x * pixelsToWorld;
            Handles.Label(pos - new Vector3(worldWidth * 0.5f, HandleUtility.GetHandleSize(pos) * 0.2f, 0), content.text, style);

            controlPointProp.vector3Value = Handles.FreeMoveHandle(pos, size, Vector3.zero, Handles.CircleHandleCap);
        }

        style.normal.textColor = serializedObject.FindProperty("lengthTextColor").colorValue;
        for (int i = 0; i < controlPoints.count - 1; i++)
        {
            var controlPoint_1 = controlPoints.serializedProperty.GetArrayElementAtIndex(i).vector3Value;
            var controlPoint_2 = controlPoints.serializedProperty.GetArrayElementAtIndex(i + 1).vector3Value;

            var pos = (controlPoint_1 + controlPoint_2) / 2;
            var dir = controlPoint_2 - controlPoint_1;

            var right = Vector3.Cross(Vector3.up, dir).normalized;
            var up = Vector3.Cross(dir, right).normalized;
            pos += up * 0.1f;

            Handles.BeginGUI();
            var oldMatrix = GUI.matrix;

            var angle = -Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            if (angle > 90f) angle -= 180f;
            if (angle < -90f) angle += 180f;
            GUIUtility.RotateAroundPivot(angle, HandleUtility.WorldToGUIPoint(pos));

            var content = new GUIContent($"Len {Vector3.Distance(controlPoint_1, controlPoint_2)}");
            var size = GUI.skin.label.CalcSize(content);
            var pixelsToWorld = HandleUtility.GetHandleSize(pos) / PIXELTOWORLDSCALE;
            var worldWidth = size.x * pixelsToWorld;

            Handles.Label(pos - new Vector3(worldWidth * 0.5f, 0, 0), content.text, style);

            GUI.matrix = oldMatrix;
            Handles.EndGUI();
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
        var RemoveButtonWidth = 25;

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

        if (GUI.Button(new Rect(rect.width + 14f, rect.y, RemoveButtonWidth, EditorGUIUtility.singleLineHeight), new GUIContent("x")))
        {
            RemoveControlPointAt(index);
        }
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
    void RemoveControlPointAt(int index)
    {
        var controlPointManager = (ControlPointsManager)target;
        controlPointManager.RemoveControlPoint(index);
    }

}