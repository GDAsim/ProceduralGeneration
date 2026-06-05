using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ControlPointsManager : MonoBehaviour
{
    [SerializeField] int count;
    [SerializeField] List<Vector3> controlPoints = new();

    [Header("Editor Settings")]
    [SerializeField] bool editInHierachy = true;
    [SerializeField] bool showGizmoPath = true;
    [SerializeField] GameObject controlPointPrefab;
    [SerializeField] Color pointsColor = Color.yellow;
    [SerializeField] Color pathColor = Color.cyan;

    [SerializeField, HideInInspector] List<GameObject> controlPointsGOs = new();

    public List<Vector3> ControlPoints
    {
        get => controlPoints;
        set
        {
            controlPoints = value;

            count = controlPoints.Count;

            SpawnDespawnControlPointsGO();
            EditControlPointsGO();

            for (int i = 0; i < controlPointsGOs.Count; i++)
            {
                controlPointsGOs[i].transform.position = controlPoints[i];
            }
        }
    }

    void Start()
    {
        controlPointsGOs = new();
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
            for (int i = controlPointsGOs.Count - 1; i >= 0; i--)
            {
                DestroyImmediate(controlPointsGOs[i]);
                controlPointsGOs.RemoveAt(i);
            }
            return;
        }
        for (int i = controlPointsGOs.Count; i < controlPoints.Count; i++)
        {
            var newGO = Instantiate(controlPointPrefab, transform);
            newGO.transform.position = controlPoints[i];

            controlPointsGOs.Add(newGO);
        }
        for (int i = controlPointsGOs.Count - 1; i >= controlPoints.Count && i >= 0; i--)
        {
            DestroyImmediate(controlPointsGOs[i]);
            controlPointsGOs.RemoveAt(i);
        }
    }
    void EditControlPointsGO()
    {
        var flag = HideFlags.None;
        if (!editInHierachy)
        {
            flag |= HideFlags.HideInHierarchy;
        }
        for (int i = 0; i < controlPointsGOs.Count; i++)
        {
            var cp = controlPointsGOs[i];
            cp.name = $"Control Point {i}";
            cp.hideFlags = flag;
            cp.SetActive(editInHierachy);
        }
    }
    void UpdateControlPoints()
    {
        for (int i = 0; i < controlPointsGOs.Count; i++)
        {
            controlPoints[i] = controlPointsGOs[i].transform.position;
        }
    }
}
