using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Generate Mesh from Implicit Function using The Marching Cubes Algorithm
/// </summary>
public class MarchingCubes_Example : MonoBehaviour
{
    public enum SamplingFunction
    {
        PerlinNoise,
        Sphere,
        Box,
        Box1,
        Box2,
        Box3,
        Box4,
        Box5,
        Box6,
        Box7,
    }

    [Header("Data")]
    public SamplingFunction samplingFunction = SamplingFunction.PerlinNoise;

    [Header("Marching Cubes")]
    [Range(1, 100)] public int GridResolution = 100;
    [Range(0, 100)] public float GridSize = 1;
    public Vector3 GridOriginOffset = new Vector3(0, 0, 0);
    [Range(0, 1)] public float BinaryThreshold = 0.5f;
    public bool Interpolate = false;

    [Header("Noise")]
    [Range(0.01f, 1f)] public float NoiseResolution = 0.1f;
    public Vector3 NoiseOffset = Vector3.zero;

    [Header("Mesh")]
    [SerializeField] Material meshMaterial;

    MarchingCubes2 mc = new();
    float[,,] bufferGrid;
    Vector3 gridOrigin;

    void Update()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            var sw = new System.Diagnostics.Stopwatch();
            sw.Start();

            Run();

            sw.Stop();
            Debug.LogFormat("Generation took {0} seconds", sw.Elapsed.TotalSeconds);
        }
        Run();
    }

    void Run()
    {
        BuildBuffer();

        List<Vector3> vertices;
        List<int> indices;

        mc.Setup(GridResolution, bufferGrid);

        // Run
        gridOrigin = transform.position + GridOriginOffset;
        if (Interpolate)
        {
            (vertices, indices) = mc.MarchCubesInterpolate(gridOrigin, BinaryThreshold, GridSize);
        }
        else
        {
            (vertices, indices) = mc.MarchCubes(gridOrigin, BinaryThreshold, GridSize);
        }

        // Create Mesh
        if (vertices.Count == 0) return;
        if (indices.Count == 0) return;

        foreach (Transform child in gameObject.transform)
        {
            Destroy(child.gameObject);
        }

        CreateMesh(vertices.ToArray());
    }

    void CreateMesh(Vector3[] verts, int vertsPerMesh = UInt16.MaxValue)
    {
        // Must be divisible by 3
        vertsPerMesh = 30000;

        int numMeshes = (verts.Length / vertsPerMesh) + 1;

        for (int i = 0; i < numMeshes; i++)
        {
            var splitVerts = new List<Vector3>();
            var splitIndices = new List<int>();

            for (int j = 0; j < vertsPerMesh; j++)
            {
                int idx = i * vertsPerMesh + j;

                if (idx < verts.Length)
                {
                    splitVerts.Add(verts[idx]);
                    splitIndices.Add(j);
                }
            }

            if (splitVerts.Count == 0) continue;

            Mesh mesh = new();
            mesh.SetVertices(splitVerts);
            mesh.SetTriangles(splitIndices, 0);
            mesh.RecalculateBounds();
            mesh.RecalculateNormals();

            GameObject go = new($"Implicit Mesh {i}");
            go.transform.parent = transform;
            go.AddComponent<MeshFilter>();
            go.AddComponent<MeshRenderer>();
            go.GetComponent<Renderer>().material = meshMaterial;
            go.GetComponent<MeshFilter>().mesh = mesh;
        }
    }

    void BuildBuffer()
    {
        bufferGrid = new float[GridResolution + 1, GridResolution + 1, GridResolution + 1];

        // Set buffer
        switch (samplingFunction)
        {
            case SamplingFunction.PerlinNoise:

                for (int x = 0; x <= GridResolution; x++)
                {
                    for (int y = 0; y <= GridResolution; y++)
                    {
                        for (int z = 0; z <= GridResolution; z++)
                        {
                            bufferGrid[x, y, z] = DensityFunc.PerlinNoise3D(
                             Time.time + ((x + NoiseOffset.x + Mathf.Epsilon) * NoiseResolution),
                             Time.time + ((y + NoiseOffset.y + Mathf.Epsilon) * NoiseResolution),
                             Time.time + ((z + NoiseOffset.z + Mathf.Epsilon) * NoiseResolution));
                        }
                    }
                }

                return;
            case SamplingFunction.Sphere:
                var radius = 0.5f;
                for (int x = 0; x <= GridResolution; x++)
                {
                    for (int y = 0; y <= GridResolution; y++)
                    {
                        for (int z = 0; z <= GridResolution; z++)
                        {
                            var t = GridResolution + 1;
                            Vector3 offset = new Vector3(
                                (x - t / 2f) * GridSize / GridResolution,
                                (y - t / 2f) * GridSize / GridResolution,
                                (z - t / 2f) * GridSize / GridResolution);
                            bufferGrid[x, y, z] = DensityFunc.Sphere(offset, radius);
                        }
                    }
                }

                return;
            case SamplingFunction.Box:
                var boxSize = new Vector3(0.45f, 0.45f, 0.45f);
                for (int x = 0; x <= GridResolution; x++)
                {
                    for (int y = 0; y <= GridResolution; y++)
                    {
                        for (int z = 0; z <= GridResolution; z++)
                        {
                            var t = GridResolution + 1;
                            Vector3 offset = new Vector3(
                                (x - t / 2f) * GridSize / GridResolution,
                                (y - t / 2f) * GridSize / GridResolution,
                                (z - t / 2f) * GridSize / GridResolution);
                            bufferGrid[x, y, z] = DensityFunc.Box(offset, boxSize);
                        }
                    }
                }

                return;

            case SamplingFunction.Box1:
                var boxSize2 = new Vector3(0.45f, 0.45f, 0.45f);
                for (int x = 0; x <= GridResolution; x++)
                {
                    for (int y = 0; y <= GridResolution; y++)
                    {
                        for (int z = 0; z <= GridResolution; z++)
                        {
                            var t = GridResolution + 1;
                            Vector3 offset = new Vector3(
                                (x - t / 2f) * GridSize / GridResolution,
                                (y - t / 2f) * GridSize / GridResolution,
                                (z - t / 2f) * GridSize / GridResolution);
                            bufferGrid[x, y, z] = sf(offset.x, offset.y, offset.z);
                        }
                    }
                }

                return;
            default:

                return;
        }

        
    }

    public float sf(float x, float y, float z)
    {
        return 2.0f * z * (z * z - 3.0f * x * x) * (1.0f - y * y) + Mathf.Pow((x * x + z * z), 2) - (2.0f * y * y - 1.0f) * (1.0f - y * y);
            
    }

    void OnDrawGizmos()
    {
        gridOrigin = transform.position + GridOriginOffset;
        DrawDebugGrid(gridOrigin, GridSize);
    }

    void DrawDebugGrid(Vector3 originOffset, float size)
    {
        Gizmos.color = Color.red;
        var size3 = new Vector3(size, size, size);
        Gizmos.DrawWireCube(originOffset + size3 / 2, size3);
    }
}
