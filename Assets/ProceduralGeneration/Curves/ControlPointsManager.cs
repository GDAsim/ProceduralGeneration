using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ControlPointsManager : MonoBehaviour
{
    [SerializeField] bool editInHierachy = true;
    [SerializeField] bool showGizmoPath = true;
    [SerializeField] bool isPathClosed = true;
    [SerializeField] GameObject controlPointPrefab;
    List<GameObject> controlPointsGO = new();

    [SerializeField] int count;

    List<Vector3> controlPoints = new();
    public List<Vector3> ControlPoints
    {
        get => controlPoints;
        set
        {
            controlPoints = value;

            count = controlPoints.Count;

            SpawnDespawnControlPointsGO();
            EditControlPointsGO();

            for (int i = 0; i < controlPointsGO.Count; i++)
            {
                controlPointsGO[i].transform.position = controlPoints[i];
            }
        }
    }

    void Update()
    {
        transform.position = Vector3.zero;

        SpawnDespawnControlPoints();

        SpawnDespawnControlPointsGO();
        EditControlPointsGO();

        UpdateControlPoints();
    }

    void SpawnDespawnControlPoints()
    {
        for (int i = controlPoints.Count; i < count; i++)
        {
            controlPoints.Add(new());
        }
        for (int i = controlPoints.Count - 1; i >= count && i >= 0; i--)
        {
            controlPoints.RemoveAt(i);
        }
    }
    void SpawnDespawnControlPointsGO()
    {
        if (!controlPointPrefab)
        {
            for (int i = controlPointsGO.Count - 1; i >= 0; i--)
            {
                DestroyImmediate(controlPointsGO[i]);
                controlPointsGO.RemoveAt(i);
            }
            return;
        }
        for (int i = controlPointsGO.Count; i < controlPoints.Count; i++)
        {
            var newGO = Instantiate(controlPointPrefab, transform);
            newGO.transform.position = controlPoints[i];

            controlPointsGO.Add(newGO);
        }
        for (int i = controlPointsGO.Count - 1; i >= controlPoints.Count && i >= 0; i--)
        {
            DestroyImmediate(controlPointsGO[i]);
            controlPointsGO.RemoveAt(i);
        }
    }
    void EditControlPointsGO()
    {
        var flag = HideFlags.DontSaveInBuild;
        if (!editInHierachy)
        {
            flag |= HideFlags.HideInHierarchy;
        }
        for (int i = 0; i < controlPointsGO.Count; i++)
        {
            var cp = controlPointsGO[i];
            cp.name = $"Control Point {i}";
            cp.hideFlags = flag;
            cp.SetActive(editInHierachy);
        }
    }
    void UpdateControlPoints()
    {
        for (int i = 0; i < controlPointsGO.Count; i++)
        {
            controlPoints[i] = controlPointsGO[i].transform.position;
        }
    }

    void OnDrawGizmos()
    {
        if (!showGizmoPath) return;

        // Draw points
        Gizmos.color = Color.yellow;
        foreach (var p in controlPoints)
        {
            Gizmos.DrawSphere(p, 0.1f);
        }

        // Draw path
        Gizmos.color = Color.cyan;
        for (int i = 0; i < controlPoints.Count - 1; i++)
        {
            Gizmos.DrawLine(controlPoints[i], controlPoints[i + 1]);
        }
        if (isPathClosed && controlPoints.Count > 2)
        {
            Gizmos.DrawLine(controlPoints[controlPoints.Count - 1], controlPoints[0]);
        }
    }
}
