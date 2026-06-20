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

    //// Settings
    static float CenterCapSize = 0.15f;
    static float ControlCapSize = 0.15f;
    static Color LabelColor = Color.white;
    static Color CenterCapColor = Color.red;
    static Color ControlCapColor = Color.yellow;
    static Color LineColor = Color.white;

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

        if (GUILayout.Button("Add Point"))
        {
            AddPointAt(bSplinePoints.count);
        }
        if (bSplinePoints.count > 0)
        {
            if (GUILayout.Button("Remove Point"))
            {
                AddPointAt(bSplinePoints.count - 1);
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
        for (int i = 0; i < bSplinesManager.bSpline.BSplinePoints.Count; i++)
        {
            var bp = bSplinesManager.bSpline.BSplinePoints[i];

            var pos = bp.Position;
            var size = pointsSize;

            var content = new GUIContent($"BSpline Point {i}");
            var contentSize = GUI.skin.label.CalcSize(content);
            var pixelsToWorld = HandleUtility.GetHandleSize(pos) / PIXELTOWORLDSCALE;
            var worldWidth = contentSize.x * pixelsToWorld;

            Handles.Label(pos - new Vector3(worldWidth * 0.5f, HandleUtility.GetHandleSize(pos) * 0.2f, 0), content.text, style);

            // Center
            var newPos = Handles.FreeMoveHandle(pos, size, Vector3.zero, Handles.RectangleHandleCap);
            if (bp.Position != newPos)
            {
                Undo.RegisterCompleteObjectUndo(target, "Move");
                bp.Position = newPos;
            }

            //Left
            Handles.color = LineColor;
            Handles.DrawLine(bp.Position, bp.LeftPos);

            Handles.color = ControlCapColor;
            var capsize = ControlCapSize;
            pos = bp.LeftPos;
            size = HandleUtility.GetHandleSize(bp.LeftPos) * capsize;
            var newLeftPos = Handles.FreeMoveHandle(pos, size, Vector3.zero, Handles.CircleHandleCap);
            if (bp.LeftPos != newLeftPos)
            {
                Undo.RegisterCompleteObjectUndo(target, "Move Left Handle");
                bp.LeftPos = newLeftPos;
            }

            // Right
            Handles.color = LineColor;
            Handles.DrawLine(bp.Position, bp.RightPos);

            Handles.color = ControlCapColor;
            capsize = ControlCapSize;
            pos = bp.RightPos;
            size = HandleUtility.GetHandleSize(bp.RightPos) * capsize;
            var newRightPos = Handles.FreeMoveHandle(pos, size, Vector3.zero, Handles.CircleHandleCap);
            if (bp.RightPos != newRightPos)
            {
                Undo.RegisterCompleteObjectUndo(target, "Move Right Handle");
                bp.RightPos = newRightPos;
            }
        }

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

    void AddPointAt(int index)
    {
        Undo.IncrementCurrentGroup();
        Undo.RecordObject(target, "Add Control Point");
        bSplinesManager.AddPointAt(index);

        SceneView.RepaintAll();
    }
    void RemovePointAt(int index)
    {
        Undo.IncrementCurrentGroup();
        Undo.RecordObject(target, "Remove Control Point");
        bSplinesManager.RemovePointAt(index);

        SceneView.RepaintAll();
    }
}