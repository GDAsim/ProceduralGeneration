using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class Circle_Example : MonoBehaviour
{
    [Header("Mesh")]
    [SerializeField] Material meshMaterial;

    [Header("Settings")]
    [SerializeField] float radius = 1;
    [Range(3,100)]
    [SerializeField] int sideCount = 8;

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

        mesh = Primitive_Circle.GenerateMesh(radius, sideCount);
        mesh.hideFlags = HideFlags.DontSave;
        gameObject.GetComponent<MeshFilter>().mesh = mesh;
        gameObject.GetComponent<MeshRenderer>().material = meshMaterial;
    }
}
