using System;
using UnityEngine;

public class NaiveParametricmetric_Example : MonoBehaviour
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

        sampleresolution_U = 100;
        sampleresolution_V = 100;
        sampleresolution_W = 100;


        var mesh = GetComponent<MeshFilter>().mesh;

        NaiveParametric parametricMesh = new();
        parametricMesh.parametricFunction = ParametricFunc.Cube;
        parametricMesh.CreateParametricMesh(mesh,
                                usingU, usingV, usingW,
                                uMinDomain, uMaxDomain,
                                vMinDomain, vMaxDomain,
                                wMinDomain, wMaxDomain,
                                sampleresolution_U, sampleresolution_V, sampleresolution_W);
    }

    void Update()
    {
        //if (Input.GetKey(KeyCode.Space))
        //{
        //    var mesh = GetComponent<MeshFilter>().mesh;

        //    NaiveParametric parametricMesh = new();
        //    parametricMesh.parametricFunction = parameticCubeFunc;
        //    parametricMesh.CreateParametricMesh(mesh,
        //                            usingU, usingV, usingW,
        //                            uMinDomain, uMaxDomain,
        //                            vMinDomain, vMaxDomain,
        //                            wMinDomain, wMaxDomain,
        //                            sampleresolution_U, sampleresolution_V, sampleresolution_W);
        //}
    }

    
}
