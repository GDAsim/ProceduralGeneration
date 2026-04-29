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
        SphereSDF,
        Box,
        SchwarzP,
        Hyperbolic,
        SixTori,
        CubeCutOutSphere,
        SphereToCubeImplicit,
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
    Vector3 gridScale;

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
        gridOrigin = GridOriginOffset;
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
            go.transform.localScale = Vector3.one;
            go.transform.localPosition = Vector3.zero;
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
                SampleGridWithFunction(new Vector3(GridResolution, GridResolution, GridResolution), (pos) =>
                {
                    return DensityFunc.PerlinNoise3D(
                             Time.time + ((pos.x + NoiseOffset.x + Mathf.Epsilon) * NoiseResolution),
                             Time.time + ((pos.y + NoiseOffset.y + Mathf.Epsilon) * NoiseResolution),
                             Time.time + ((pos.z + NoiseOffset.z + Mathf.Epsilon) * NoiseResolution));
                });
                return;
            case SamplingFunction.Sphere:
                SampleGridWithFunction(new Vector3(GridResolution, GridResolution, GridResolution), (pos) =>
                {
                    var t = GridResolution + 1;
                    Vector3 offset = new(
                        (pos.x - t / 2f) * GridSize / GridResolution,
                        (pos.y - t / 2f) * GridSize / GridResolution,
                        (pos.z - t / 2f) * GridSize / GridResolution);
                    return DensityFunc.SphereImplicit(offset, 0.5f);
                });
                return;
            case SamplingFunction.SphereSDF:
                SampleGridWithFunction(new Vector3(GridResolution, GridResolution, GridResolution), (pos) =>
                {
                    var t = GridResolution + 1;
                    Vector3 offset = new(
                        (pos.x - t / 2f) * GridSize / GridResolution,
                        (pos.y - t / 2f) * GridSize / GridResolution,
                        (pos.z - t / 2f) * GridSize / GridResolution);
                    return DensityFunc.SphereSDF(offset, 0.5f);
                });
                return;
            case SamplingFunction.Box:
                var boxSize = new Vector3(0.45f, 0.45f, 0.45f);
                SampleGridWithFunction(new Vector3(GridResolution, GridResolution, GridResolution), (pos) =>
                {
                    var t = GridResolution + 1;
                    Vector3 offset = new(
                       (pos.x - t / 2f) * GridSize / GridResolution,
                       (pos.y - t / 2f) * GridSize / GridResolution,
                       (pos.z - t / 2f) * GridSize / GridResolution);
                    return DensityFunc.BoxSDF(offset, boxSize);
                });
                return;
            case SamplingFunction.SchwarzP:
                SampleGridWithFunction(new Vector3(GridResolution, GridResolution, GridResolution), (pos) =>
                {
                    var t = GridResolution + 1;
                    Vector3 offset = new(
                       (pos.x - t / 2f) * GridSize / GridResolution,
                       (pos.y - t / 2f) * GridSize / GridResolution,
                       (pos.z - t / 2f) * GridSize / GridResolution);
                    return DensityFunc.SchwartzP(offset);
                });
                return;
            case SamplingFunction.Hyperbolic:
                SampleGridWithFunction(new Vector3(GridResolution, GridResolution, GridResolution), (pos) =>
                {
                    var t = GridResolution + 1;
                    Vector3 offset = new(
                       (pos.x - t / 2f) * GridSize / GridResolution,
                       (pos.y - t / 2f) * GridSize / GridResolution,
                       (pos.z - t / 2f) * GridSize / GridResolution);
                    return DensityFunc.Hyperbolic(offset);
                });
                return;
            case SamplingFunction.SixTori:
                SampleGridWithFunction(new Vector3(GridResolution, GridResolution, GridResolution), (pos) =>
                {
                    var t = GridResolution + 1;
                    Vector3 offset = new(
                       (pos.x - t / 2f) * GridSize / GridResolution,
                       (pos.y - t / 2f) * GridSize / GridResolution,
                       (pos.z - t / 2f) * GridSize / GridResolution);
                    return DensityFunc.SixTori(offset);
                });
                return;
            case SamplingFunction.CubeCutOutSphere:
                SampleGridWithFunction(new Vector3(GridResolution, GridResolution, GridResolution), (pos) =>
                {
                    var t = GridResolution + 1;
                    Vector3 offset = new(
                       (pos.x - t / 2f) * GridSize / GridResolution,
                       (pos.y - t / 2f) * GridSize / GridResolution,
                       (pos.z - t / 2f) * GridSize / GridResolution);
                    return DensityFunc.CubeCutOutSphere(offset);
                });
                return;
            case SamplingFunction.SphereToCubeImplicit:
                SampleGridWithFunction(new Vector3(GridResolution, GridResolution, GridResolution), (pos) =>
                {
                    var t = GridResolution + 1;
                    Vector3 offset = new(
                       (pos.x - t / 2f) * GridSize / GridResolution,
                       (pos.y - t / 2f) * GridSize / GridResolution,
                       (pos.z - t / 2f) * GridSize / GridResolution);
                    return DensityFunc.SphereToCubeImplict(offset);
                });
                return;
            default:
                return;
        }


    }
    void SampleGridWithFunction(Vector3 GridResolution, Func<Vector3, float> samplingFunction)
    {
        for (int x = 0; x <= GridResolution.x; x++)
        {
            for (int y = 0; y <= GridResolution.y; y++)
            {
                for (int z = 0; z <= GridResolution.z; z++)
                {
                    var pos = new Vector3(x, y, z);
                    bufferGrid[x, y, z] = samplingFunction(pos);
                }
            }
        }
    }

    void OnDrawGizmos()
    {
        gridOrigin = transform.position + GridOriginOffset;
        gridScale = transform.localScale;
        DrawDebugGrid(gridOrigin, GridSize, gridScale);
    }

    void DrawDebugGrid(Vector3 originOffset, float size, Vector3 scale)
    {
        Gizmos.color = Color.red;
        var size3 = scale * size;
        Gizmos.DrawWireCube(originOffset + size3 / 2, scale * size);
    }
}
