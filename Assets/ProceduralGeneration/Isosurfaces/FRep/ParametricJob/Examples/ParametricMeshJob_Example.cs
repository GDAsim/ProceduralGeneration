using System.Runtime.InteropServices;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

/// <summary>
/// Mesh job is done in two sub jobs
/// MeshVerticesJob - calculate vertices position using sampling function
/// MeshIndicesJob - set indices based on the meshvertices job
/// </summary>
public class ParametricMeshJob_Example : MonoBehaviour
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
        if (SystemInfo.supports32bitsIndexBuffer)
        {
            mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        }
    }

    void Update()
    {
        if(Input.GetKey(KeyCode.Space))
        {
            var mesh = GetComponent<MeshFilter>().mesh;

            StartMeshJob(usingU, usingV, usingW,
               uMinDomain, uMaxDomain,
               vMinDomain, vMaxDomain,
               wMinDomain, wMaxDomain,
               sampleresolution_U, sampleresolution_V, sampleresolution_W);

            EndGOLJob(mesh);
        }
    }


    #region ParametricMeshJob Job
    ParametricMeshVerticesJob parametricMeshVerticesJob;
    JobHandle sheduleParralelJobHandle;
    NativeArray<Vector3> outputVertices;

    ParametricMeshIndicesJob parametricMeshIndicesJob;
    JobHandle sheduleParralelJobHandle2;
    NativeList<int> outputIndices;

    void parametricFunction(double u, double v, double w, out double x, out double y, out double z)
    {
        x = u;
        y = v;
        z = w;
    }

    void StartMeshJob(bool isusingU, bool isusingV, bool isusingW,
        float uMinDomain, float uMaxDomain,
        float vMinDomain, float vMaxDomain,
        float wMinDomain, float wMaxDomain,
        int sampleresolution_U, int sampleresolution_V, int sampleresolution_W,
        bool isRightCoordinateSystem = false)
    {
        parametricMeshVerticesJob = new ParametricMeshVerticesJob();
        parametricMeshVerticesJob.uMinDomain = uMinDomain;
        parametricMeshVerticesJob.uMaxDomain = uMaxDomain;
        parametricMeshVerticesJob.vMinDomain = vMinDomain;
        parametricMeshVerticesJob.vMaxDomain = vMaxDomain;
        parametricMeshVerticesJob.wMinDomain = wMinDomain;
        parametricMeshVerticesJob.wMaxDomain = wMaxDomain;

        //Some preprocessing
        int numOfDimentions = 0;
        if (isusingU) numOfDimentions++;
        else sampleresolution_U = 0;
        if (isusingV) numOfDimentions++;
        else sampleresolution_V = 0;
        if (isusingW) numOfDimentions++;
        else sampleresolution_W = 0;

        sampleresolution_U += 1;
        sampleresolution_V += 1;
        sampleresolution_W += 1;

        parametricMeshVerticesJob.sampleresolution_U = sampleresolution_U;
        parametricMeshVerticesJob.sampleresolution_V = sampleresolution_V;
        parametricMeshVerticesJob.sampleresolution_W = sampleresolution_W;
        parametricMeshVerticesJob.isRightCoordinateSystem = isRightCoordinateSystem;
        parametricMeshVerticesJob.parametricFunction = new FunctionPointer<ParametricMeshVerticesJob.SamplingFunction>(Marshal.GetFunctionPointerForDelegate((ParametricMeshVerticesJob.SamplingFunction)parametricFunction));


        outputVertices = new NativeArray<Vector3>(sampleresolution_U * sampleresolution_V * sampleresolution_W, Allocator.TempJob);
        parametricMeshVerticesJob.outputVertices = outputVertices;

        sheduleParralelJobHandle = parametricMeshVerticesJob.ScheduleParallel(outputVertices.Length, 64, new JobHandle());


        parametricMeshIndicesJob = new ParametricMeshIndicesJob();
        parametricMeshIndicesJob.isusingU = isusingU;
        parametricMeshIndicesJob.isusingV = isusingV;
        parametricMeshIndicesJob.isusingW = isusingW;
        parametricMeshIndicesJob.sampleresolution_U = sampleresolution_U;
        parametricMeshIndicesJob.sampleresolution_V = sampleresolution_V;
        parametricMeshIndicesJob.sampleresolution_W = sampleresolution_W;
        parametricMeshIndicesJob.numOfDimentions = numOfDimentions;

        outputIndices = new NativeList<int>(Allocator.TempJob);
        parametricMeshIndicesJob.outputIndices = outputIndices;

        sheduleParralelJobHandle2 = parametricMeshIndicesJob.Schedule(outputVertices.Length, new JobHandle());
    }
    Mesh EndGOLJob(Mesh mesh)
    {
        JobHandle.CompleteAll(ref sheduleParralelJobHandle, ref sheduleParralelJobHandle2);

        mesh.SetVertices(parametricMeshVerticesJob.outputVertices);

        int numOfDimentions = parametricMeshIndicesJob.numOfDimentions;
        if (numOfDimentions == 1)
        {
            mesh.SetIndices(parametricMeshIndicesJob.outputIndices.AsArray(), MeshTopology.LineStrip, 0);

        }
        else if (numOfDimentions == 2 || numOfDimentions == 3)
        {
            mesh.SetIndices(parametricMeshIndicesJob.outputIndices.AsArray(), MeshTopology.Quads, 0);
        }

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        outputVertices.Dispose();
        outputIndices.Dispose();

        return mesh;
    }
    #endregion
}