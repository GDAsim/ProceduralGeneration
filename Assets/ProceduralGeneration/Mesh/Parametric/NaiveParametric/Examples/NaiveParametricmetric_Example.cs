using UnityEngine;
using UnityEngine.XR;

[ExecuteInEditMode]
public class NaiveParametricmetric_Example : MonoBehaviour
{
    public enum ParametricFunction
    {
        Cube,
        Sphere,
    }

    [Header("Data")]
    public ParametricFunction parametricFunction = ParametricFunction.Cube;

    [Header("Parametric")]
    [SerializeField] Vector2 uDomain = new Vector2(-1, 1);
    [SerializeField] Vector2 vDomain = new Vector2(-1, 1);
    [SerializeField] Vector2 wDomain = new Vector2(-1, 1);

    [SerializeField] int sampleresolution_U = 100;
    [SerializeField] int sampleresolution_V = 100;
    [SerializeField] int sampleresolution_W = 100;

    [Header("Mesh")]
    [SerializeField] Material meshMaterial;

    NaiveParametric parametricMesh = new();
    GameObject meshGO;
    Mesh mesh;
    void Start()
    {
        meshGO = new($"Parametric Mesh");
        mesh = new Mesh();
        meshGO.transform.parent = transform;
        meshGO.AddComponent<MeshFilter>();
        meshGO.AddComponent<MeshRenderer>();
        meshGO.GetComponent<Renderer>().material = meshMaterial;
        meshGO.GetComponent<MeshFilter>().mesh = mesh;

        Run();
    }
    void OnValidate()
    {
        if (meshGO == null)
        {
            meshGO = new($"Parametric Mesh");
            mesh = new Mesh();
            meshGO.transform.parent = transform;
            meshGO.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            meshGO.AddComponent<MeshFilter>();
            meshGO.AddComponent<MeshRenderer>();
            meshGO.GetComponent<Renderer>().material = meshMaterial;
            meshGO.GetComponent<MeshFilter>().mesh = mesh;
        }

        Run();
    }
    void Update()
    {
        // Clean up 
        foreach (Transform child in transform)
        {
            if (child != meshGO.transform)
            {
                if(Application.isPlaying)
                {
                    Destroy(child.gameObject);
                }
                else
                {
                    DestroyImmediate(child.gameObject);
                }
            }
        }
    }
    void Run()
    {
        switch (parametricFunction)
        {
            case ParametricFunction.Cube:
                uDomain = new Vector2(-1, 1);
                vDomain = new Vector2(-1, 1);
                wDomain = new Vector2(-1, 1);
                parametricMesh.SetParametricFunction(ParametricFunc.Cube, true, true, true);
                break;
            case ParametricFunction.Sphere:
                uDomain = new Vector2(-Mathf.PI, Mathf.PI);
                vDomain = new Vector2(-Mathf.PI, Mathf.PI);
                parametricMesh.SetParametricFunction(ParametricFunc.Sphere, true, true, false);
                break;
            default:
                break;
        }

        parametricMesh.ModifyMesh(mesh,
            uDomain, vDomain, wDomain,
            sampleresolution_U, sampleresolution_V, sampleresolution_W);
    }
}
