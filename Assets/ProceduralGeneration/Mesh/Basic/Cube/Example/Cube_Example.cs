using UnityEngine;

public class Cube_Example : MonoBehaviour
{
    [Header("Mesh")]
    [SerializeField] Material meshMaterial;

    [Header("Settings")]
    [SerializeField] Vector3 size = Vector3.one;

    GameObject meshGO;
    Mesh mesh;

    void Start()
    {
        meshGO = new($"Cube Mesh");
        mesh = new Mesh();
        mesh.hideFlags = HideFlags.DontSave;
        meshGO.transform.parent = transform;
        meshGO.AddComponent<MeshFilter>();
        meshGO.AddComponent<MeshRenderer>();
        meshGO.GetComponent<Renderer>().material = meshMaterial;
        meshGO.GetComponent<MeshFilter>().mesh = mesh;
    }
    void OnValidate()
    {
        if (meshGO == null)
        {
            meshGO = new($"Cube Mesh");
            mesh = new Mesh();
            mesh.hideFlags = HideFlags.DontSave;
            meshGO.transform.parent = transform;
            meshGO.AddComponent<MeshFilter>();
            meshGO.AddComponent<MeshRenderer>();
            meshGO.GetComponent<Renderer>().material = meshMaterial;
            meshGO.GetComponent<MeshFilter>().mesh = mesh;
        }

        var F_TL = new Vector3(-1, 1, -1);
        var F_TR = new Vector3(1, 1, -1);
        var F_BL = new Vector3(-1, -1, -1);
        var F_BR = new Vector3(1, -1, -1);
        var B_TL = new Vector3(-1, 1, 1);
        var B_TR = new Vector3(1, 1, 1);
        var B_BL = new Vector3(-1, -1, 1);
        var B_BR = new Vector3(1, -1, 1);

        F_TL = Vector3.Scale(F_TL, size);
        F_TR = Vector3.Scale(F_TR, size);
        F_BL = Vector3.Scale(F_BL, size);
        F_BR = Vector3.Scale(F_BR, size);
        B_TL = Vector3.Scale(B_TL, size);
        B_TR = Vector3.Scale(B_TR, size);
        B_BL = Vector3.Scale(B_BL, size);
        B_BR = Vector3.Scale(B_BR, size);

        var triangles = new int[]
        {
            0, 2, 1,   0, 3, 2,     // front
            4, 6, 5,   4, 7, 6,     // back
            8, 10, 9,  8, 11, 10,   // top
            12, 14, 13, 12, 15, 14, // bottom
            16, 18, 17, 16, 19, 18, // right
            20, 22, 21, 20, 23, 22  // left
        };

        var vertices = new Vector3[8]
        {
            F_TL,F_TR,F_BL,F_BR,
            B_TL,B_TR,B_BL,B_BR
        };

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
    }
}
