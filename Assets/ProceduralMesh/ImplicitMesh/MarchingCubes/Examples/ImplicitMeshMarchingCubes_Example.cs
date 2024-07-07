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

            MarchingCubes marchingCube = new MarchingCubes();
            marchingCube.implicitFunction = implicitFunction;

            Vector3 MarchingBoundingBoxSize = new Vector3(1.1f, 1.1f, 1.1f);
            Vector3 MarchingBoundingBoxCenter = new Vector3(0, 0, 0);
            Vector3 BoundingBoxResolution = new Vector3(100, 100, 100);
            marchingCube.MarchChunk(MarchingBoundingBoxCenter, MarchingBoundingBoxSize, BoundingBoxResolution, false);

            Vector3[] verts = marchingCube.GetVertices();

            //A mesh in unity can only be made up of 65000 verts.
            //Need to split the verts between multiple meshes.

            int maxVertsPerMesh = 3000; //must be divisible by 3, ie 3 verts == 1 triangle - set this value higher max 65000
            int numMeshes = verts.Length / maxVertsPerMesh + 1;

            for (int i = 0; i < numMeshes; i++)
            {
                List<Vector3> splitVerts = new List<Vector3>();
                List<int> splitIndices = new List<int>();

                for (int j = 0; j < maxVertsPerMesh; j++)
                {
                    int idx = i * maxVertsPerMesh + j;

                    if (idx < verts.Length)
                    {
                        splitVerts.Add(verts[idx]);
                        splitIndices.Add(j);
                    }
                }

                if (splitVerts.Count == 0) continue;

                Mesh mesh = new Mesh();
                mesh.SetVertices(splitVerts);
                mesh.SetTriangles(splitIndices, 0);
                mesh.RecalculateBounds();
                mesh.RecalculateNormals();

                GameObject go = new GameObject($"Impllcit Mesh {i}");
                go.transform.parent = transform;
                go.AddComponent<MeshFilter>();
                go.AddComponent<MeshRenderer>();
                go.GetComponent<Renderer>().material = material;
                go.GetComponent<MeshFilter>().mesh = mesh;
            }

            sw.Stop();
            Debug.LogFormat("Generation took {0} seconds", sw.Elapsed.TotalSeconds);
        }
    }

    double implicitFunction(Vector3 position)
    {
        double r = 0.5;
        double x = position.x;
        double y = position.y;
        double z = position.z;

        //using >= 0
        //E.g.
        //For the Implicit Function of a sphere : x^2+y^2+z^2 = r^2
        //Will be rearrange such as such : r*2-x^2-y^2-z^2 = 0
        return (r * r - x * x - y * y - z * z);
    }
}
