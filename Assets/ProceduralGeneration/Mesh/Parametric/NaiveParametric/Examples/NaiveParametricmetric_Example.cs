using UnityEngine;

[ExecuteInEditMode]
public class NaiveParametricmetric_Example : MonoBehaviour
{
    public enum ParametricFunction
    {
        Plane,
        Cube,
        Sphere,
        Moebius,
        Torus,
        Horn,
        HelixCurve,
        ButterflyCurve,
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

        meshGO.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);

        Run();
    }
    void OnValidate()
    {
        if (meshGO == null)
        {
            meshGO = new($"Parametric Mesh");
            mesh = new Mesh();
            meshGO.transform.parent = transform;
            meshGO.AddComponent<MeshFilter>();
            meshGO.AddComponent<MeshRenderer>();
            meshGO.GetComponent<Renderer>().material = meshMaterial;
            meshGO.GetComponent<MeshFilter>().mesh = mesh;
        }

        meshGO.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);

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
            case ParametricFunction.Plane:
                uDomain = new Vector2(-1, 1);
                vDomain = new Vector2(-1, 1);
                sampleresolution_U = 100;
                sampleresolution_V = 100;
                parametricMesh.SetParametricFunction(ParametricFunc.Plane, true, true, false);
                break;
            case ParametricFunction.Cube:
                uDomain = new Vector2(-1, 1);
                vDomain = new Vector2(-1, 1);
                wDomain = new Vector2(-1, 1);
                sampleresolution_U = 100;
                sampleresolution_V = 100;
                sampleresolution_W = 100;
                parametricMesh.SetParametricFunction(ParametricFunc.Cube, true, true, true);
                break;
            case ParametricFunction.Sphere:
                uDomain = new Vector2(-Mathf.PI, Mathf.PI);
                vDomain = new Vector2(-Mathf.PI, Mathf.PI);
                sampleresolution_U = 100;
                sampleresolution_V = 100;
                parametricMesh.SetParametricFunction(ParametricFunc.Sphere, true, true, false);
                break;
            case ParametricFunction.Moebius:
                uDomain = new Vector2(-0.4f, 0.4f);
                vDomain = new Vector2(0, 2 * Mathf.PI);
                sampleresolution_U = 100;
                sampleresolution_V = 100;
                parametricMesh.SetParametricFunction(ParametricFunc.Moebius, true, true, false);
                break;
            case ParametricFunction.Torus:
                uDomain = new Vector2(0, 2 * Mathf.PI);
                vDomain = new Vector2(0, 2 * Mathf.PI);
                sampleresolution_U = 100;
                sampleresolution_V = 100;   
                parametricMesh.SetParametricFunction(ParametricFunc.Torus, true, true, false);
                break;
            case ParametricFunction.Horn:
                uDomain = new Vector2(0, 1);
                vDomain = new Vector2(0, 2 * Mathf.PI);
                sampleresolution_U = 100;
                sampleresolution_V = 100;
                parametricMesh.SetParametricFunction(ParametricFunc.Horn, true, true, false);
                break;
            case ParametricFunction.HelixCurve:
                uDomain = new Vector2(0, 6 * Mathf.PI);
                sampleresolution_U = 100;
                parametricMesh.SetParametricFunction(ParametricFunc.HelixCurve, true, false, false);
                break;
            case ParametricFunction.ButterflyCurve:
                uDomain = new Vector2(0, 12 * Mathf.PI);
                sampleresolution_U = 3000;
                parametricMesh.SetParametricFunction(ParametricFunc.ButterflyCurve, true, false, false);
                break;
            default:
                break;
        }

        parametricMesh.ModifyMesh(mesh,
            uDomain, vDomain, wDomain,
            sampleresolution_U, sampleresolution_V, sampleresolution_W);
    }
}
