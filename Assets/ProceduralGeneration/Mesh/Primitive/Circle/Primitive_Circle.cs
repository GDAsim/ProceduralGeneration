using UnityEngine;

public static class Primitive_Circle
{
    public static Mesh GenerateMesh(float radius, int sideCount)
    {
        return ModifyMesh(new Mesh(), radius, sideCount);
    }
    public static Mesh ModifyMesh(Mesh mesh, float radius, int sideCount)
    {
        var vertices = new Vector3[sideCount + 1];

        // Center 
        vertices[0] = Vector3.zero;

        // Position Other Vertices
        for (int i = 0; i < sideCount; i++)
        {
            float angle = (i / (float)sideCount) * 2f * Mathf.PI;

            float x = radius * Mathf.Cos(angle);
            float z = radius * Mathf.Sin(angle);

            vertices[i + 1] = new Vector3(x, 0, z);
        }

        // Set Triangles
        int[] triangles = new int[sideCount * 3];
        int index = 0;
        for (int i = 0; i < sideCount; i++)
        {
            triangles[index++] = 0;
            triangles[index++] = (i + 1) % (sideCount) + 1;
            triangles[index++] = (i + 1);
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        return mesh;
    }
}
