using UnityEngine;

[ExecuteInEditMode]
[RequireComponent (typeof(MeshFilter), typeof(MeshRenderer))]
public class Cube_Example : MonoBehaviour
{
    [Header("Mesh")]
    [SerializeField] Material meshMaterial;

    [Header("Settings")]
    [SerializeField] Vector3 size = Vector3.one;

    Mesh mesh;

    void Update()
    {
        if (!gameObject.GetComponent<MeshFilter>())
        {
            gameObject.AddComponent<MeshFilter>();
            gameObject.GetComponent<MeshFilter>().mesh = mesh;
        }
        
        if (!gameObject.GetComponent<MeshRenderer>())
        {
            gameObject.AddComponent<MeshRenderer>();
            gameObject.GetComponent<MeshRenderer>().material = meshMaterial;
        }

        
    }
    void OnValidate()
    {
        mesh = new Mesh();
        mesh.hideFlags = HideFlags.DontSave;
        gameObject.GetComponent<MeshFilter>().mesh = mesh;
        gameObject.GetComponent<MeshRenderer>().material = meshMaterial;

        var F_TL = new Vector3(-1, 1, -1);
        var F_BL = new Vector3(-1, -1, -1);
        var F_BR = new Vector3(1, -1, -1);
        var F_TR = new Vector3(1, 1, -1);
        var B_TL = new Vector3(-1, 1, 1);
        var B_BL = new Vector3(-1, -1, 1);
        var B_BR = new Vector3(1, -1, 1);
        var B_TR = new Vector3(1, 1, 1);

        var s = size / 2f;
        F_TL = Vector3.Scale(F_TL, s);
        F_BL = Vector3.Scale(F_BL, s);
        F_BR = Vector3.Scale(F_BR, s);
        F_TR = Vector3.Scale(F_TR, s);
        B_TL = Vector3.Scale(B_TL, s);
        B_BL = Vector3.Scale(B_BL, s);
        B_BR = Vector3.Scale(B_BR, s);
        B_TR = Vector3.Scale(B_TR, s);

        var vertices = new Vector3[8]
        {
            F_TL,F_BL,F_BR,F_TR,
            B_TL,B_BL,B_BR,B_TR
        };
        var triangles = new int[]
        {
            0, 2, 1,   0, 3, 2,     // front
            4, 3, 0,   4, 7, 3,     // top
            4, 1, 5,   4, 0, 1,     // left
            3, 6, 2,   3, 7, 6,     // right
            1, 6, 5,   1, 2, 6,     // bottom
            7, 5, 6,   7, 4, 5      // back
        };
        var uvs = new Vector2[8]
        {
            new Vector2(0, 1), // F_TL
            new Vector2(0, 0), // F_BL
            new Vector2(1, 0), // F_BR
            new Vector2(1, 1), // F_TR
            new Vector2(0, 1), // B_TL
            new Vector2(0, 0), // B_BL
            new Vector2(1, 0), // B_BR
            new Vector2(1, 1)  // B_TR
        };


        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
    }
}
