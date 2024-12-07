using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// Refrences:
/// https://en.wikipedia.org/wiki/Marching_squares
/// https://jamie-wong.com/2014/08/19/metaballs-and-marching-squares/
/// https://catlikecoding.com/unity/tutorials/marching-squares/
/// 
/// </summary>

[ExecuteInEditMode]
public class MarchingSquares_Example : MonoBehaviour
{
    [Header("Data")]
    public bool UseSamplingFunction = false;

    [Header("Marching Squares")]
    [Range(1, 100)] public int GridResolution = 100;
    [Range(0, 100)] public float GridSize = 1;
    public Vector2 GridOriginOffset = new Vector2(0, 0);
    [Range(0, 1)] public float BinaryThreshold = 0.5f;
    public bool Interpolate = false;

    [Header("Noise")]
    [Range(0.01f, 1f)] public float NoiseResolution = 0.1f;
    public Vector2 NoiseOffset = Vector2.zero;

    [Header("Mesh")]
    [SerializeField] MeshFilter meshFilter;

    [Header("Debug")]
    float gridPointSize = 0.5f; // Percentage of gridResolution

    MarchingSquares ms = new();

    float[,] bufferGrid;
    
    void Update()
    {
        BuildBuffer(GridOriginOffset);

        ms.Setup(GridResolution, bufferGrid);

        List<Vector3> vertices;
        List<int> indices;

        // Run
        if (Interpolate)
        {
            (vertices, indices) = ms.MarchSquaresInterpolate(GridOriginOffset, BinaryThreshold, GridSize);
        }
        else
        {
            (vertices, indices) = ms.MarchSquares(GridOriginOffset, BinaryThreshold, GridSize);
        }

        // Create Mesh
        if (meshFilter == null) return;

        if (vertices.Count == 0) return;
        if (indices.Count == 0) return;

        Mesh mesh = new();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = indices.ToArray();
        meshFilter.mesh = mesh;
    }

    void BuildBuffer(Vector2 offset)
    {
        bufferGrid = new float[GridResolution + 1, GridResolution + 1];

        // Set buffer
        if(UseSamplingFunction)
        {
            for (int x = 0; x <= GridResolution; x++)
            {
                for (int y = 0; y <= GridResolution; y++)
                {
                    var sampleX = x * (GridSize / GridResolution) + offset.x;
                    var sampleY = y * (GridSize / GridResolution) + offset.y;
                    bufferGrid[x, y] = Circle_Implicit(sampleX, sampleY);
                }
            }
        }
        else
        {
            for (int x = 0; x <= GridResolution; x++)
            {
                for (int y = 0; y <= GridResolution; y++)
                {
                    var sampleX = x * (GridSize / GridResolution) + offset.x;
                    var sampleY = y * (GridSize / GridResolution) + offset.y;
                    bufferGrid[x, y] = Mathf.PerlinNoise(
                         Time.time + ((sampleX + NoiseOffset.x + offset.x + Mathf.Epsilon) * NoiseResolution),
                         Time.time + ((sampleY + NoiseOffset.y + offset.y + Mathf.Epsilon) * NoiseResolution));
                }
            }
        }

        float Circle_Implicit(float posX, float posY)
        {
            float r = 5f;
            float x = posX;
            float y = posY;
            return (r * r - x * x - y * y);
        }
    }

    void OnDrawGizmos()
    {
        var pointSize = gridPointSize / 2 * (GridSize / GridResolution);

        DrawDebugGridDots(GridOriginOffset, pointSize, ms.GetBuffer());
    }

    void DrawDebugGridDots(Vector2 originOffset, float size, float[,] buffer)
    {
        if (buffer == null) return;

        for (int x = 0; x <= GridResolution; x++)
        {
            for (int y = 0; y <= GridResolution; y++)
            {
                Vector3 offset = new Vector2(x, y) * (GridSize / GridResolution) + originOffset;
                Gizmos.color = new Color(1 - buffer[x, y], 1 - buffer[x, y], 1 - buffer[x, y]);
                Gizmos.DrawSphere(transform.position + offset, size);
            }
        }
    }
}