﻿using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Generate Mesh from Implicit Function using The Marching Cubes Algorithm
/// </summary>
public class MarchingCubes_Example : MonoBehaviour
{
    [Header("Data")]
    public bool UseSamplingFunction = false;

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
        if (Interpolate)
        {
            (vertices, indices) = mc.MarchCubesInterpolate(GridOriginOffset, BinaryThreshold, GridSize);
        }
        else
        {
            (vertices, indices) = mc.MarchCubes(GridOriginOffset, BinaryThreshold, GridSize);
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
        if (UseSamplingFunction)
        {
            for (int x = 0; x <= GridResolution; x++)
            {
                for (int y = 0; y <= GridResolution; y++)
                {
                    for (int z = 0; z <= GridResolution; z++)
                    {
                        var t = GridResolution + 1;
                        Vector3 offset = new Vector3(
                            (x - t / 2f) * GridSize/ GridResolution,
                            (y - t / 2f) * GridSize / GridResolution,
                            (z - t / 2f) * GridSize / GridResolution);
                        bufferGrid[x, y, z] = Sphere_Implicit(offset.x, offset.y, offset.z);
                    }
                }
            }
        }
        else
        {
            for (int x = 0; x <= GridResolution; x++)
            {
                for (int y = 0; y <= GridResolution; y++)
                {
                    for (int z = 0; z <= GridResolution; z++)
                    {
                        bufferGrid[x, y, z] = PerlinNoise3D(
                         Time.time + ((x + NoiseOffset.x + Mathf.Epsilon) * NoiseResolution),
                         Time.time + ((y + NoiseOffset.y + Mathf.Epsilon) * NoiseResolution),
                         Time.time + ((z + NoiseOffset.z + Mathf.Epsilon) * NoiseResolution));
                    }
                }
            }
        }

        float PerlinNoise3D(float x, float y, float z)
        {
            float xy = Mathf.PerlinNoise(x, y);
            float xz = Mathf.PerlinNoise(x, z);
            float yz = Mathf.PerlinNoise(y, z);
            float yx = Mathf.PerlinNoise(y, x);
            float zx = Mathf.PerlinNoise(z, x);
            float zy = Mathf.PerlinNoise(z, y);

            return (xy + xz + yz + yx + zx + zy) / 6 * 2 - 1;
        }

        float Sphere_Implicit(float posX, float posY,float posZ)
        {
            float r = 0.5f;
            float x = posX;
            float y = posY;
            float z = posZ;

            return (r * r - x * x - y * y - z * z);
        }
    }

    void OnDrawGizmos()
    {
         DrawDebugGrid(GridOriginOffset, GridSize);
    }

    void DrawDebugGrid(Vector3 originOffset, float size)
    {
        Gizmos.color = Color.red;
        var size3 = new Vector3(size, size, size);
        Gizmos.DrawWireCube(transform.position + size3 / 2, size3);
    }
}
