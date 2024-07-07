using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

struct ParametricMeshVerticesJob : IJobFor
{
    //Read
    [ReadOnly] public float uMinDomain; [ReadOnly] public float uMaxDomain;
    [ReadOnly] public float vMinDomain; [ReadOnly] public float vMaxDomain;
    [ReadOnly] public float wMinDomain; [ReadOnly] public float wMaxDomain;

    [ReadOnly] public int sampleresolution_U;
    [ReadOnly] public int sampleresolution_V;
    [ReadOnly] public int sampleresolution_W;

    [ReadOnly] public bool isRightCoordinateSystem;

    public delegate void SamplingFunction(double u, double v, double w, out double x, out double y, out double z);
    public FunctionPointer<SamplingFunction> parametricFunction;

    //Write
    public NativeArray<Vector3> outputVertices;

    public void Execute(int index)
    {
        int k = index / (sampleresolution_U * sampleresolution_V);
        int j = index % (sampleresolution_U * sampleresolution_V) / sampleresolution_V;
        int i = index % sampleresolution_U;

        float w = uMinDomain + k * ((wMaxDomain - wMinDomain) / (sampleresolution_W - 1));
        float v = uMinDomain + j * ((vMaxDomain - vMinDomain) / (sampleresolution_V - 1));
        float u = uMinDomain + i * ((uMaxDomain - uMinDomain) / (sampleresolution_U - 1));

        parametricFunction.Invoke(u, v, w, out double x, out double y, out double z);

        if (isRightCoordinateSystem)
        {
            z *= -1;
        }

        outputVertices[index] = new Vector3((float)x, (float)y, (float)z);
    }

    //void parametricFunction(double u, double v, double w, out double x, out double y, out double z)
    //{
    //    x = u;
    //    y = v;
    //    z = w;
    //}
}