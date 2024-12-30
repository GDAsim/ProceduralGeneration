/// <summary>
/// About:
/// Simple Mesh Crumple Script
/// 
/// How It Works:
/// Uses noise to modify all vertices of the mesh
/// 
/// How To Use:
/// Attach this script to a mesh
/// </summary>

using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class MeshCrumple : MonoBehaviour
{
    public float scale = 1.0f;
    public float speed = 1.0f;

    PerlinNoise1 noise;
    Mesh mesh;
    Vector3[] originalVertices;

    void Start()
    {
        noise = new PerlinNoise1();

        mesh = GetComponent<MeshFilter>().mesh;
        if(mesh)
        {
            originalVertices = mesh.vertices;
        }
    }

    void Update()
    {
        if (!mesh) return;

        Vector3[] vertices = new Vector3[originalVertices.Length];

        float timeX = Time.time * speed + 0.1365143f;
        float timeY = Time.time * speed + 1.21688f;
        float timeZ = Time.time * speed + 2.5564f;

        for (int i = 0; i < vertices.Length; i++)
        {
            Vector3 vertex = originalVertices[i];
            vertex.x += noise.Noise(timeX + vertex.x, timeX + vertex.y, timeX + vertex.z) * scale;
            vertex.y += noise.Noise(timeY + vertex.x, timeY + vertex.y, timeY + vertex.z) * scale;
            vertex.z += noise.Noise(timeZ + vertex.x, timeZ + vertex.y, timeZ + vertex.z) * scale;

            vertices[i] = vertex;
        }

        mesh.vertices = vertices;

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
    }
}