using System;
using System.Collections.Generic;
using Unity.VisualScripting;
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
    [Range(0, 100)] public int GridResolution = 100;
    [Range(0, 10)] public float GridSize = 1;
    public Vector3 GridPos = new Vector3(0, 0, 0);
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
        BuildBuffer();

        ms.Setup(GridResolution, bufferGrid);

        List<Vector3> vertices;
        List<int> indices;

        // Run
        if (Interpolate)
        {
            (vertices, indices) = ms.MarchSquaresInterpolate(GridPos, BinaryThreshold, GridSize);
        }
        else
        {
            (vertices, indices) = ms.MarchSquares(GridPos, BinaryThreshold, GridSize);
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

    void BuildBuffer()
    {
        bufferGrid = new float[GridResolution + 1, GridResolution + 1];

        // Set buffer
        if(UseSamplingFunction)
        {
            for (int x = 0; x <= GridResolution; x++)
            {
                for (int y = 0; y <= GridResolution; y++)
                {
                    bufferGrid[x, y] = Circle_Implicit(x, y);
                }
            }
        }
        else
        {
            for (int x = 0; x <= GridResolution; x++)
            {
                for (int y = 0; y <= GridResolution; y++)
                {
                    bufferGrid[x, y] = Mathf.PerlinNoise(
                         Time.time + ((x + NoiseOffset.x + Mathf.Epsilon) * NoiseResolution),
                         Time.time + ((y + NoiseOffset.y + Mathf.Epsilon) * NoiseResolution));
                }
            }
        }

        float Circle_Implicit(float posX, float posY)
        {
            posX -= GridResolution / 2;
            posY -= GridResolution / 2;

            float r = 5f;
            float x = posX;
            float y = posY;

            return (r * r - x * x - y * y);
        }
    }

    void OnDrawGizmos()
    {
        var pos = transform.position;
        var pointSize = gridPointSize / 2 * GridSize;

        DrawDebugGridDots(pos, pointSize, ms.GetBuffer());
    }

    void DrawDebugGridDots(Vector3 pos, float size, float[,] buffer)
    {
        if (buffer == null) return;

        for (int x = 0; x <= GridResolution; x++)
        {
            for (int y = 0; y <= GridResolution; y++)
            {
                var offset = new Vector3(x, y, 0) * GridSize;
                Gizmos.color = new Color(1 - buffer[x, y], 1 - buffer[x, y], 1 - buffer[x, y]);
                Gizmos.DrawSphere(pos + offset, size);
            }
        }
    }
}
