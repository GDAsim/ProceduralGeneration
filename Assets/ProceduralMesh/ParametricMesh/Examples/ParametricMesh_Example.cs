using System;
using System.Collections.Generic;
using UnityEngine;

public class ParametricMesh_Example : MonoBehaviour
{
    [SerializeField] MeshFilter meshfilter;

    bool usingU;
    bool usingV;
    bool usingW;

    float uMinDomain;
    float uMaxDomain;

    float vMinDomain;
    float vMaxDomain;

    float wMinDomain;
    float wMaxDomain;

    int sampleresolution_U;
    int sampleresolution_V;
    int sampleresolution_W;

    void Start()
    {
        usingU = true;
        usingV = true;
        usingW = true;

        uMinDomain = -1;
        uMaxDomain = 1;

        vMinDomain = -1;
        vMaxDomain = 1;

        wMinDomain = -1;
        wMaxDomain = 1;

        sampleresolution_U = 2;
        sampleresolution_V = 2;
        sampleresolution_W = 2;
    }

    List<Vector3> verts = new List<Vector3>();
    void Update()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            var mesh = GetComponent<MeshFilter>().mesh;

            ParametricMesh parametricMesh = new();
            parametricMesh.parametricFunction = parameticCubeFunc;
            parametricMesh.CreateParametricMesh(mesh,
                                    usingU, usingV, usingW,
                                    uMinDomain, uMaxDomain,
                                    vMinDomain, vMaxDomain,
                                    wMinDomain, wMaxDomain,
                                    sampleresolution_U, sampleresolution_V, sampleresolution_W);

            mesh.GetVertices(verts);
        }
    }
    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        foreach (var v in verts)
        {
            Gizmos.DrawSphere(v, 0.05f);
        }
    }

    void parameticCubeFunc(double u, double v, double w, out double x, out double y, out double z)
    {
        x = u;
        y = v;
        z = w;
    }
    void parametricSphereFunc(double u, double v, double w, out double x, out double y, out double z)
    {
        x = Math.Cos(u) * Math.Cos(v);
        y = Math.Sin(u) * Math.Cos(v);
        z = Math.Sin(v);
    }
}
