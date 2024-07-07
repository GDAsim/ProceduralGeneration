using System.Collections.Generic;
using UnityEngine;

public class ParametricMesh
{
    public delegate void SamplingFunction(double u, double v, double w, out double x, out double y, out double z);
    public SamplingFunction parametricFunction;

    public Mesh CreateParametricMesh
        (Mesh inmesh,
        bool isusingU, bool isusingV, bool isusingW,
        float uMinDomain, float uMaxDomain,
        float vMinDomain, float vMaxDomain,
        float wMinDomain, float wMaxDomain,
        int sampleresolution_U, int sampleresolution_V, int sampleresolution_W,
        bool isRightCoordinateSystem = false)
    {
        // Some checks before creating mesh
        int numOfDimentions = 0;
        if (isusingU) numOfDimentions++;
        else sampleresolution_U = 0;
        if (isusingV) numOfDimentions++;
        else sampleresolution_V = 0;
        if (isusingW) numOfDimentions++;
        else sampleresolution_W = 0;

        Mesh mesh = inmesh;
        if (SystemInfo.supports32bitsIndexBuffer)
        {
            mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        }
        else
        {
            // Already using default Uint16
        }
        List<Vector3> vertices = new List<Vector3>();

        sampleresolution_U += 1;
        sampleresolution_V += 1;
        sampleresolution_W += 1;

        for (int k = 0; k < sampleresolution_W; k++)
        {
            float w = uMinDomain + k * ((wMaxDomain - wMinDomain) / (sampleresolution_W - 1));
            for (int j = 0; j < sampleresolution_V; j++)
            {
                float v = uMinDomain + j * ((vMaxDomain - vMinDomain) / (sampleresolution_V - 1));
                for (int i = 0; i < sampleresolution_U; i++)
                {
                    float u = uMinDomain + i * ((uMaxDomain - uMinDomain) / (sampleresolution_U - 1));

                    parametricFunction(u, v, w, out double x, out double y, out double z);

                    if(isRightCoordinateSystem)
                    {
                        z *= -1;
                    }

                    //if(k != 0 && k != sampleresolution_W -1)
                    //{
                    //    if(j != 0 && j != sampleresolution_V - 1 && i != 0 && i != sampleresolution_U - 1)
                    //    {
                    //        continue;
                    //    }
                    //}

                    vertices.Add(new Vector3((float)x, (float)y, (float)z));
                }
            }
        }
        var ic = (sampleresolution_U - 1) * (sampleresolution_V - 1) * 6 * 4;
        int[] indices = new int[ic];

        for (int index = 0; index < vertices.Count; index++)
        {
            int k = index / (sampleresolution_U * sampleresolution_V);
            int j = index % (sampleresolution_U * sampleresolution_V) / sampleresolution_V;
            int i = index % sampleresolution_U;

            int prow = index / (sampleresolution_U * sampleresolution_V + sampleresolution_U);
            int km = k>0 ? 1 : 0;
            int rowcount = index / sampleresolution_V;
            int a = (index - sampleresolution_U - rowcount + prow);
            int id = (a) * 4;

            #region 3 Variables Used: Algo to create a 8 vert box
            //Drawing Clockwise
            if (numOfDimentions == 3)
            {
                if (k == 0 && i >= 1 && j >= 1)//front
                {
                    //topright botright botleft topleft
                    indices[id] = index;
                    indices[id] = index - sampleresolution_U;
                    indices[id] = index - 1 - sampleresolution_U;
                    indices[id] = index - 1;
                }
                if (k == sampleresolution_W - 1 && i >= 1 && j >= 1)//back
                {
                    //topright botright botleft topleft
                    indices[id] = index - 1;
                    indices[id] = index - 1 - sampleresolution_U;
                    indices[id] = index - sampleresolution_U;
                    indices[id] = index;
                }
                if (k >= 1 && i == 0 && j >= 1) //left
                {
                    //topleft topright botright botleft
                    indices[id] = index;
                    indices[id] = index - sampleresolution_U * sampleresolution_V;
                    indices[id] = index - sampleresolution_U - sampleresolution_U * sampleresolution_V;
                    indices[id] = index - sampleresolution_U;
                }
                if (k >= 1 && i == sampleresolution_U - 1 && j >= 1) //right
                {
                    indices[id] = index - sampleresolution_U;
                    indices[id] = index - sampleresolution_U - sampleresolution_U * sampleresolution_V;
                    indices[id] = index - sampleresolution_U * sampleresolution_V;
                    indices[id] = index;
                }
                if (k >= 1 && j == 0 && i >= 1) //bot
                {
                    indices[id] = index;
                    indices[id] = index - 1;
                    indices[id] = index - 1 - sampleresolution_U * sampleresolution_V;
                    indices[id] = index - sampleresolution_U * sampleresolution_V;
                }
                if (k >= 1 && j == sampleresolution_V - 1 && i >= 1) //top
                {
                    indices[id] = index - sampleresolution_U * sampleresolution_V;
                    indices[id] = index - 1 - sampleresolution_U * sampleresolution_V;
                    indices[id] = index - 1;
                    indices[id] = index;
                }
            }
            #endregion

            #region 2 Variables Used
            //In order to allow for any parmetric u v w to be used in any order,
            //we need to know which is used/not used for generating curves
            //so that we can grab the correct loop index to do the triangle indexing
            else if (numOfDimentions == 2)
            {
                //i=u,j=v;k=w;
                int loopindex1 = i;
                int loopindex2 = j;
                int sampleres = sampleresolution_U;
                if (isusingU)
                {
                    loopindex1 = i;
                    sampleres = sampleresolution_U;
                    if (isusingV)
                    {
                        loopindex2 = j;
                    }
                    else if (isusingW)
                    {
                        loopindex2 = k;
                    }
                }
                else
                {
                    sampleres = sampleresolution_V;
                    loopindex1 = k;
                    loopindex2 = j;
                }
                if (loopindex1 >= 1 && loopindex2 >= 1)
                {
                    indices[id] = index - 1;
                    indices[id] = index;
                    indices[id] = index - sampleres;
                    indices[id] = index - 1 - sampleres;
                }
            }
            #endregion

            #region 1 Variables Used
            else if (numOfDimentions == 1)
            {
                indices[id] = index;
            }
            #endregion
        }

        mesh.SetVertices(vertices);
        if (numOfDimentions == 1)
        {
            mesh.SetIndices(indices, MeshTopology.LineStrip, 0);

        }
        else if (numOfDimentions == 2 || numOfDimentions == 3)
        {
            mesh.SetIndices(indices, MeshTopology.Quads, 0);
            mesh.RecalculateNormals();
        }
        mesh.RecalculateBounds();
        return mesh;
    }
}