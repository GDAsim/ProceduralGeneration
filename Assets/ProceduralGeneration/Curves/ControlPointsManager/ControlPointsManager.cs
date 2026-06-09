using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

[ExecuteInEditMode]
public class ControlPointsManager : MonoBehaviour
{
    [HideInInspector] public List<Vector3> ControlPoints = new();

    [Header("Gizmo Settings")]
    [SerializeField] bool showGizmoPath = true;
    [SerializeField] float pointsSize = 0.1f;
    [SerializeField] Color pointsColor = Color.yellow;
    [SerializeField] Color pathColor = Color.cyan;

    [Header("Editor Settings")]
    [SerializeField] bool editInHierachy = true;
    [SerializeField, HideInInspector] List<GameObject> controlPointsGOs = new();

    void Update()
    {
        transform.position = Vector3.zero;

        if (editInHierachy)
        {
            CleanAndSpawnControlPointsGO();
            UpdateControlPointsGO();
            UpdateControlPointsUsingGO();
        }
        else
        {
            DespawnControlPointsGO();
        }
    }
    void CleanAndSpawnControlPointsGO()
    {
        // Destory null objects
        for (int i = controlPointsGOs.Count - 1; i >= 0; i--)
        {
            if (controlPointsGOs[i] == null)
            {
                controlPointsGOs.RemoveAt(i);
            }
        }

        // Spawn if not enough
        for (int i = controlPointsGOs.Count; i < ControlPoints.Count; i++)
        {
            var newGO = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            newGO.transform.SetParent(transform);
            newGO.transform.position = ControlPoints[i];
            controlPointsGOs.Add(newGO);
        }
    }
    void DespawnControlPointsGO()
    {
        for (int i = controlPointsGOs.Count - 1; i >= 0; i--)
        {
            DestroyImmediate(controlPointsGOs[i]);
            controlPointsGOs.RemoveAt(i);
        }
    }
    void UpdateControlPointsGO()
    {
        var scale = 2 * pointsSize * Vector3.one;
        for (int i = 0; i < controlPointsGOs.Count; i++)
        {
            var controlPointGO = controlPointsGOs[i];
            controlPointGO.name = $"Control Point {i}";
            controlPointGO.transform.localScale = scale;

            controlPointGO.GetComponent<MeshRenderer>().sharedMaterial.color = pointsColor;
        }
    }
    void UpdateControlPointsUsingGO()
    {
        for (int i = 0; i < controlPointsGOs.Count; i++)
        {
            ControlPoints[i] = controlPointsGOs[i].transform.position;
        }
    }

    public void AddControlPoint()
    {
        int targetIndex = Mathf.Max(0, ControlPoints.Count);
        AddControlPoint(targetIndex);
    }
    public void AddControlPoint(int index)
    {
        if (index < 0 || index > ControlPoints.Count) return;

        Vector3 newPoint = Vector3.zero;
        if (ControlPoints.Count == 0)
        {
            newPoint = Vector3.zero;
        }
        else if (ControlPoints.Count == 1)
        {
            if (index <= 0)
            {
                newPoint = ControlPoints[0] + new Vector3(-0.4f, 0, 0);
            }
            else if (index >= 1)
            {
                newPoint = ControlPoints[0] + new Vector3(0.4f, 0, 0);
            }
        }
        else //if (ControlPoints.Count > 1)
        {
            if (index <= 0)
            {
                newPoint = ControlPoints[0] + Vector3.Normalize(ControlPoints[0] - ControlPoints[1]) * 0.4f;
            }
            else if (index >= ControlPoints.Count)
            {
                newPoint = ControlPoints[ControlPoints.Count - 1] + Vector3.Normalize(ControlPoints[ControlPoints.Count - 1] - ControlPoints[ControlPoints.Count - 2]) * 0.4f;
            }
            else if (index < ControlPoints.Count)
            {
                newPoint = (ControlPoints[index - 1] + ControlPoints[index]) / 2;
            }
        }

        Undo.IncrementCurrentGroup();
        Undo.RecordObject(this, "Add Control Point");
        ControlPoints.Insert(index, newPoint);

        var newGO = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        newGO.transform.SetParent(transform);
        newGO.transform.position = newPoint;
        controlPointsGOs.Add(newGO);
        Undo.RegisterCreatedObjectUndo(newGO, "Save Curve");
    }
    public void RemovePoint()
    {
        int targetIndex = Mathf.Max(0, ControlPoints.Count);
        RemoveControlPoint(targetIndex);
    }
    public void RemoveControlPoint(int index)
    {
        if (index < 0 || index > ControlPoints.Count) return;

        Undo.IncrementCurrentGroup();
        Undo.RecordObject(this, "Add Control Point");
        ControlPoints.RemoveAt(index);

        if (controlPointsGOs.Count > index)
        {
            DestroyImmediate(controlPointsGOs[index]);
            controlPointsGOs.RemoveAt(index);
        }

        SceneView.RepaintAll();
    }

    void OnDrawGizmos()
    {
        // Draw Points
        if (!editInHierachy)
        {
            Gizmos.color = pointsColor;
            for (int i = 0; ControlPoints.Count > i; i++)
            {
                var controlPoint = ControlPoints[i];
                Gizmos.DrawSphere(controlPoint, pointsSize);
            }
        }

        // Draw path
        if (showGizmoPath)
        {
            Gizmos.color = pathColor;
            Gizmos.DrawLineStrip(ControlPoints.ToArray(), false);
        }
    }
}
