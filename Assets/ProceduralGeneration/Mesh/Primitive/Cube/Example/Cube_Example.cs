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

    void Start()
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
        if (!gameObject.GetComponent<MeshFilter>())
        {
            gameObject.AddComponent<MeshFilter>();
        }

        if (!gameObject.GetComponent<MeshRenderer>())
        {
            gameObject.AddComponent<MeshRenderer>();
        }

        mesh = Primitive_Cube.GenerateMesh(size);
        mesh.hideFlags = HideFlags.DontSave;
        gameObject.GetComponent<MeshFilter>().mesh = mesh;
        gameObject.GetComponent<MeshRenderer>().material = meshMaterial;
    }
}
