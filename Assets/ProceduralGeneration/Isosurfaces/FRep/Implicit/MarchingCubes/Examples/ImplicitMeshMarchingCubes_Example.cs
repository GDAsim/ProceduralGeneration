using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Generate Mesh from Implicit Function using The Marching Cubes Algorithm
/// </summary>
public class ImplicitMeshMarchingCubes_Example : MonoBehaviour
{
    public Material material;

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
    }

    void Run()
    {
        // Setup
        var marchingCube = new MarchingCubes
        {
            implicitFunction = Sphere_Implicit
        };
        var MarchingBoundingBoxSize = new Vector3(1.1f, 1.1f, 1.1f);
        var MarchingBoundingBoxCenter = new Vector3(0, 0, 0);
        var BoundingBoxResolution = new Vector3(100, 100, 100);

        // Run
        marchingCube.MarchChunk(MarchingBoundingBoxCenter, MarchingBoundingBoxSize, BoundingBoxResolution, false);

        // Extract Data
        Vector3[] verts = marchingCube.GetVertices();

        // Create Mesh
        CreateMesh(verts);
    }

    void CreateMesh(Vector3[] verts, int vertsPerMesh = UInt16.MaxValue)
    {
        // Must be divisible by 3
        vertsPerMesh = 3000;

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
            go.GetComponent<Renderer>().material = material;
            go.GetComponent<MeshFilter>().mesh = mesh;
        }
    }

    /// <summary>
    /// The Input Sampling Implicit Function to be passed to the Marching Cube Algorithm in the form of
    /// f(x,y,z) >= 0
    /// Position refers to the current 3D Coordinates of the world, (similar like verts input in shader languages)
    /// 
    /// This is an Implicit Function of a sphere : x² + y² + z² = r²
    /// Rearranged to return the result of f(x,y,z) : r² - x² - y² - z² = 0
    static double Sphere_Implicit(Vector3 position)
    {
        double r = 0.5;
        double x = position.x;
        double y = position.y;
        double z = position.z;

        return (r * r - x * x - y * y - z * z);
    }
}
