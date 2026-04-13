using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Creates a mesh using parametric Function as input, using naive;y perfrom 3 loop to define vertices and create triangles
/// Added custom parameters to create curves/surfaces or solids , Meshes*
/// </summary>
public class NaiveParametric
{
    SamplingFunction samplingFunction;
    bool usingU;
    bool usingV;
    bool usingW;

    public delegate void SamplingFunction(double u, double v, double w, out double x, out double y, out double z);

    public void SetParametricFunction(SamplingFunction samplingFunction,
        bool usingU, bool usingV, bool usingW)
    {
        this.samplingFunction = samplingFunction;
        this.usingU = usingU;
        this.usingV = usingV;
        this.usingW = usingW;
    }
    public Mesh ModifyMesh(Mesh inmesh,
        Vector2 uDomainRange, Vector2 vDomainRange, Vector2 wDomainRange,
        int samplingResolution_U, int samplingResolution_V, int samplingResolution_W)
    {
        // Some checks before creating mesh
        var numOfDimensions = 0;
        if (usingU) numOfDimensions++;
        else samplingResolution_U = 0;
        if (usingV) numOfDimensions++;
        else samplingResolution_V = 0;
        if (usingW) numOfDimensions++;
        else samplingResolution_W = 0;

        List<Vector3> vertices = new List<Vector3>();
        List<int> indices = new List<int>();

        samplingResolution_U += 1;
        samplingResolution_V += 1;
        samplingResolution_W += 1;

        var uMin = uDomainRange.x;
        var uMax = uDomainRange.y;
        var vMin = vDomainRange.x;
        var vMax = vDomainRange.y;
        var wMin = wDomainRange.x;
        var wMax = wDomainRange.y;
        for (int k = 0; k < samplingResolution_W; k++)
        {
            float w = uMin + k * ((wMax - wMin) / (samplingResolution_W - 1));
            for (int j = 0; j < samplingResolution_V; j++)
            {
                float v = uMin + j * ((vMax - vMin) / (samplingResolution_V - 1));
                for (int i = 0; i < samplingResolution_U; i++)
                {
                    float u = uMin + i * ((uMax - uMin) / (samplingResolution_U - 1));

                    samplingFunction(u, v, w, out double x, out double y, out double z);

                    vertices.Add(new Vector3((float)x, (float)y, (float)z));

                    #region 3 Variables Used: Algo to create a 8 vert box
                    //Drawing Clockwise
                    if (numOfDimensions == 3)
                    {
                        if (k == 0 && i >= 1 && j >= 1)//front
                        {
                            //topright botright botleft topleft
                            indices.Add(vertices.Count - 1);
                            indices.Add(vertices.Count - 1 - samplingResolution_U);
                            indices.Add(vertices.Count - 1 - 1 - samplingResolution_U);
                            indices.Add(vertices.Count - 1 - 1);
                        }
                        if (k == samplingResolution_W - 1 && i >= 1 && j >= 1)//back
                        {
                            //topright botright botleft topleft
                            indices.Add(vertices.Count - 1 - 1);
                            indices.Add(vertices.Count - 1 - 1 - samplingResolution_U);
                            indices.Add(vertices.Count - 1 - samplingResolution_U);
                            indices.Add(vertices.Count - 1);
                        }
                        if (k >= 1 && i == 0 && j >= 1) //left
                        {
                            //topleft topright botright botleft
                            indices.Add(vertices.Count - 1);
                            indices.Add(vertices.Count - 1 - samplingResolution_U * samplingResolution_V);
                            indices.Add(vertices.Count - 1 - samplingResolution_U - samplingResolution_U * samplingResolution_V);
                            indices.Add(vertices.Count - 1 - samplingResolution_U);
                        }
                        if (k >= 1 && i == samplingResolution_U - 1 && j >= 1) //right
                        {
                            indices.Add(vertices.Count - 1 - samplingResolution_U);
                            indices.Add(vertices.Count - 1 - samplingResolution_U - samplingResolution_U * samplingResolution_V);
                            indices.Add(vertices.Count - 1 - samplingResolution_U * samplingResolution_V);
                            indices.Add(vertices.Count - 1);
                        }
                        if (k >= 1 && j == 0 && i >= 1) //bot
                        {
                            indices.Add(vertices.Count - 1);
                            indices.Add(vertices.Count - 1 - 1);
                            indices.Add(vertices.Count - 1 - 1 - samplingResolution_U * samplingResolution_V);
                            indices.Add(vertices.Count - 1 - samplingResolution_U * samplingResolution_V);
                        }
                        if (k >= 1 && j == samplingResolution_V - 1 && i >= 1) //top
                        {
                            indices.Add(vertices.Count - 1 - samplingResolution_U * samplingResolution_V);
                            indices.Add(vertices.Count - 1 - 1 - samplingResolution_U * samplingResolution_V);
                            indices.Add(vertices.Count - 1 - 1);
                            indices.Add(vertices.Count - 1);
                        }
                    }
                    #endregion

                    #region 2 Variables Used
                    //In order to allow for any parmetric u v w to be used in any order,
                    //we need to know which is used/not used for generating curves
                    //so that we can grab the correct loop index to do the triangle indexing
                    else if (numOfDimensions == 2)
                    {
                        //i=u,j=v;k=w;
                        int loopindex1 = i;
                        int loopindex2 = j;
                        int sampleres = samplingResolution_U;
                        if (usingU)
                        {
                            loopindex1 = i;
                            sampleres = samplingResolution_U;
                            if (usingV)
                            {
                                loopindex2 = j;
                            }
                            else if (usingW)
                            {
                                loopindex2 = k;
                            }
                        }
                        else
                        {
                            sampleres = samplingResolution_V;
                            loopindex1 = k;
                            loopindex2 = j;
                        }
                        if (loopindex1 >= 1 && loopindex2 >= 1)
                        {
                            indices.Add(vertices.Count - 1 - 1);
                            indices.Add(vertices.Count - 1);
                            indices.Add(vertices.Count - 1 - sampleres);
                            indices.Add(vertices.Count - 1 - 1 - sampleres);
                        }
                    }
                    #endregion

                    #region 1 Variables Used
                    else if (numOfDimensions == 1)
                    {
                        indices.Add(vertices.Count - 1);
                    }
                    #endregion
                }
            }
        }

        Mesh mesh = inmesh;
        if (SystemInfo.supports32bitsIndexBuffer)
        {
            mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        }
        else
        {
            // Already using default Uint16
        }
        mesh.SetVertices(vertices);
        if (numOfDimensions == 1)
        {
            mesh.SetIndices(indices.ToArray(), MeshTopology.LineStrip, 0);

        }
        else if (numOfDimensions == 2 || numOfDimensions == 3)
        {
            mesh.SetIndices(indices.ToArray(), MeshTopology.Quads, 0);
            mesh.RecalculateNormals();
        }
        mesh.RecalculateBounds();
        return mesh;
    }
}