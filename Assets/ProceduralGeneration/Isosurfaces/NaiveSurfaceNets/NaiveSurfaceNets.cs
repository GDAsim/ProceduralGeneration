using System.Collections.Generic;
using UnityEngine;

// Edited from Marching Cubes

public class SurfaceNets
{
    /// <summary>
    /// Data Grid / Voxel Grid / Sampling Buffer
    /// </summary>
    float[,,] bufferGrid;

    int gridResolution;

    public void Setup(int gridResolution, float[,,] inBuffer)
    {
        this.gridResolution = gridResolution;
        bufferGrid = inBuffer;
    }
    
    public (List<Vector3>, List<int>) Generate(Vector3 originOffset, float size)
    {
        var vertices = new List<Vector3>();
        var indices = new List<int>();

        var gridResolution_X = gridResolution;
        var gridResolution_Y = gridResolution;
        var gridResolution_Z = gridResolution;

        for (int z = 0; z < gridResolution_Z; z++)
        {
            for (int y = 0; y < gridResolution_Y; y++)
            {
                for (int x = 0; x < gridResolution_X; x++)
                {
                    var Zero = new Vector3Int(x, y, z);
                    var frontbl = CornerOffsetTable[0];
                    var frontbr = CornerOffsetTable[1];
                    var fronttr = CornerOffsetTable[2];
                    var fronttl = CornerOffsetTable[3];
                    var backbl = CornerOffsetTable[4];
                    var backbr = CornerOffsetTable[5];
                    var backtr = CornerOffsetTable[6];
                    var backtl = CornerOffsetTable[7];

                    var frontbl_value = bufferGrid[frontbl.x + Zero.x, frontbl.y + Zero.y, frontbl.z + Zero.z];
                    var frontbr_value = bufferGrid[frontbr.x + Zero.x, frontbr.y + Zero.y, frontbr.z + Zero.z];
                    var fronttr_value = bufferGrid[fronttr.x + Zero.x, fronttr.y + Zero.y, fronttr.z + Zero.z];
                    var fronttl_value = bufferGrid[fronttl.x + Zero.x, fronttl.y + Zero.y, fronttl.z + Zero.z];
                    var backbl_value  = bufferGrid[backbl.x + Zero.x, backbl.y + Zero.y, backbl.z + Zero.z];
                    var backbr_value  = bufferGrid[backbr.x + Zero.x, backbr.y + Zero.y, backbr.z + Zero.z];
                    var backtr_value  = bufferGrid[backtr.x + Zero.x, backtr.y + Zero.y, backtr.z + Zero.z];
                    var backtl_value  = bufferGrid[backtl.x + Zero.x, backtl.y + Zero.y, backtl.z + Zero.z];

                    int edgeIntersections = 0;
                    Vector3 aggregatedPos = Vector3.zero;

                    //Front Edges
                    if (!SameSign(frontbl_value, frontbr_value))
                    {
                        edgeIntersections++;

                        var midPoint = (Vector3)(frontbl + frontbr) * 0.5f;
                        aggregatedPos += midPoint;

                    }
                    if (!SameSign(frontbr_value, fronttr_value))
                    {
                        edgeIntersections++;

                        var midPoint = (Vector3)(frontbr + fronttr) * 0.5f;
                        aggregatedPos += midPoint;

                    }
                    if (!SameSign(fronttr_value, fronttl_value))
                    {
                        edgeIntersections++;

                        var midPoint = (Vector3)(fronttr + fronttl) * 0.5f;
                        aggregatedPos += midPoint;

                    }
                    if (!SameSign(fronttl_value, frontbl_value))
                    {
                        edgeIntersections++;

                        var midPoint = (Vector3)(fronttl + frontbl) * 0.5f;
                        aggregatedPos += midPoint;
                    }

                    //Back Edges
                    if (!SameSign(backbl_value, backbr_value))
                    {
                        edgeIntersections++;

                        var midPoint = (Vector3)(backbl + backbr) * 0.5f;
                        aggregatedPos += midPoint;

                    }
                    if (!SameSign(backbr_value, backtr_value))
                    {
                        edgeIntersections++;

                        var midPoint = (Vector3)(backbr + backtr) * 0.5f;
                        aggregatedPos += midPoint;

                    }
                    if (!SameSign(backtr_value, backtl_value))
                    {
                        edgeIntersections++;

                        var midPoint = (Vector3)(backtr + backtl) * 0.5f;
                        aggregatedPos += midPoint;

                    }
                    if (!SameSign(backtl_value, backbl_value))
                    {
                        edgeIntersections++;

                        var midPoint = (Vector3)(backtl + backbl) * 0.5f;
                        aggregatedPos += midPoint;
                    }

                    //Front->Back Edges
                    if (!SameSign(frontbl_value, backbl_value))
                    {
                        edgeIntersections++;

                        var midPoint = (Vector3)(frontbl + backbl) * 0.5f;
                        aggregatedPos += midPoint;

                    }
                    if (!SameSign(frontbr_value, backbr_value))
                    {
                        edgeIntersections++;

                        var midPoint = (Vector3)(frontbr + backbr) * 0.5f;
                        aggregatedPos += midPoint;

                    }
                    if (!SameSign(fronttr_value, backtr_value))
                    {
                        edgeIntersections++;

                        var midPoint = (Vector3)(fronttr + backtr) * 0.5f;
                        aggregatedPos += midPoint;

                    }
                    if (!SameSign(fronttl_value, backtl_value))
                    {
                        edgeIntersections++;

                        var midPoint = (Vector3)(fronttl + backtl) * 0.5f;
                        aggregatedPos += midPoint;
                    }

                    if (edgeIntersections == 0) //??
                    {
                        var averagePos = new Vector3(x + 0.5f, y + 0.5f, z + 0.5f);
                        Vector3 newVert = (averagePos + Zero) * (size / gridResolution) + originOffset;
                        vertices.Add(newVert);
                    }
                    else
                    {
                        var averagePos = aggregatedPos / edgeIntersections;
                        Vector3 newVert = (averagePos + Zero) * (size / gridResolution) + originOffset;
                        vertices.Add(newVert);
                    }
                }
            }
        }

        // Generate Triangles by connecting manually check
        for (int z = 1; z < gridResolution_Z; z++)
        {
            for (int y = 1; y < gridResolution_Y; y++)
            {
                for (int x = 1; x < gridResolution_X; x++)
                {
                    var vIndex = x + (y * gridResolution_X) + (z * gridResolution_X * gridResolution_Y);

                    //if x, connect y z?
                    var botEdgeP1 = bufferGrid[x, y, z];
                    var botEdgeP2 = bufferGrid[x + 1, y, z];
                    if (!SameSign(botEdgeP1, botEdgeP2))
                    {
                        // if not same sign means we need to connnect, but which order?
                        // check sign and flip accordingly?
                        if (botEdgeP1 < 0)
                        {
                            indices.Add(vIndex);
                            indices.Add(vIndex - gridResolution_X * gridResolution_Y);
                            indices.Add(vIndex - gridResolution_X - gridResolution_X * gridResolution_Y);
                            indices.Add(vIndex - gridResolution_X);
                        }
                        else
                        {
                            indices.Add(vIndex);
                            indices.Add(vIndex - gridResolution_X);
                            indices.Add(vIndex - gridResolution_X - gridResolution_X * gridResolution_Y);
                            indices.Add(vIndex - gridResolution_X * gridResolution_Y);
                        }
                    }

                    //if y, connect x z?
                    var leftEdgeP1 = bufferGrid[x, y, z];
                    var leftEdgeP2 = bufferGrid[x, y + 1, z];
                    if (!SameSign(leftEdgeP1, leftEdgeP2))
                    {
                        // if not same sign means we need to connnect, but which order?
                        // check sign and flip accordingly?
                        if (leftEdgeP1 < 0)
                        {
                            indices.Add(vIndex);
                            indices.Add(vIndex - 1);
                            indices.Add(vIndex - gridResolution_X * gridResolution_Y - 1);
                            indices.Add(vIndex - gridResolution_X * gridResolution_Y);
                        }
                        else
                        {
                            indices.Add(vIndex);
                            indices.Add(vIndex - gridResolution_X * gridResolution_Y);
                            indices.Add(vIndex - gridResolution_X * gridResolution_Y - 1);
                            indices.Add(vIndex - 1);
                        }
                    }

                    //if z, connect x y?
                    var forwardEdgeP1 = bufferGrid[x, y, z];
                    var forwardEdgeP2 = bufferGrid[x, y, z + 1];
                    if (!SameSign(forwardEdgeP1, forwardEdgeP2))
                    {
                        // if not same sign means we need to connnect, but which order?
                        // check sign and flip accordingly?
                        if (forwardEdgeP1 < 0)
                        {
                            indices.Add(vIndex);
                            indices.Add(vIndex - gridResolution_X);
                            indices.Add(vIndex - gridResolution_X - 1);
                            indices.Add(vIndex - 1);
                        }
                        else
                        {
                            indices.Add(vIndex);
                            indices.Add(vIndex - 1);
                            indices.Add(vIndex - gridResolution_X - 1);
                            indices.Add(vIndex - gridResolution_X);
                        }
                    }
                }
            }
        }

        return (vertices, indices);
    }

    public (List<Vector3>, List<int>) GenerateInterpolate(Vector3 originOffset, float size)
    {
        var vertices = new List<Vector3>();
        var indices = new List<int>();

        var gridResolution_X = gridResolution;
        var gridResolution_Y = gridResolution;
        var gridResolution_Z = gridResolution;

        for (int z = 0; z < gridResolution_Z; z++)
        {
            for (int y = 0; y < gridResolution_Y; y++)
            {
                for (int x = 0; x < gridResolution_X; x++)
                {
                    var Zero = new Vector3Int(x, y, z);
                    var frontbl = CornerOffsetTable[0];
                    var frontbr = CornerOffsetTable[1];
                    var fronttr = CornerOffsetTable[2];
                    var fronttl = CornerOffsetTable[3];
                    var backbl = CornerOffsetTable[4];
                    var backbr = CornerOffsetTable[5];
                    var backtr = CornerOffsetTable[6];
                    var backtl = CornerOffsetTable[7];

                    var frontbl_value = bufferGrid[frontbl.x + Zero.x, frontbl.y + Zero.y, frontbl.z + Zero.z];
                    var frontbr_value = bufferGrid[frontbr.x + Zero.x, frontbr.y + Zero.y, frontbr.z + Zero.z];
                    var fronttr_value = bufferGrid[fronttr.x + Zero.x, fronttr.y + Zero.y, fronttr.z + Zero.z];
                    var fronttl_value = bufferGrid[fronttl.x + Zero.x, fronttl.y + Zero.y, fronttl.z + Zero.z];
                    var backbl_value = bufferGrid[backbl.x + Zero.x, backbl.y + Zero.y, backbl.z + Zero.z];
                    var backbr_value = bufferGrid[backbr.x + Zero.x, backbr.y + Zero.y, backbr.z + Zero.z];
                    var backtr_value = bufferGrid[backtr.x + Zero.x, backtr.y + Zero.y, backtr.z + Zero.z];
                    var backtl_value = bufferGrid[backtl.x + Zero.x, backtl.y + Zero.y, backtl.z + Zero.z];

                    int edgeIntersections = 0;
                    Vector3 aggregatedPos = Vector3.zero;

                    //Front Edges
                    if (!SameSign(frontbl_value, frontbr_value))
                    {
                        edgeIntersections++;

                        var p1value = frontbl_value;
                        var p2value = frontbr_value;
                        float delta = p1value - p2value;
                        float weight = p1value / delta;
                        var midPoint = frontbl + weight * (Vector3)(frontbr - frontbl); // Lerp formula

                        aggregatedPos += midPoint;

                    }
                    if (!SameSign(frontbr_value, fronttr_value))
                    {
                        edgeIntersections++;

                        var p1value = frontbr_value;
                        var p2value = fronttr_value;
                        float delta = p1value - p2value;
                        float weight = p1value / delta;
                        var midPoint = frontbr + weight * (Vector3)(fronttr - frontbr); // Lerp formula

                        aggregatedPos += midPoint;

                    }
                    if (!SameSign(fronttr_value, fronttl_value))
                    {
                        edgeIntersections++;

                        var p1value = fronttr_value;
                        var p2value = fronttl_value;
                        float delta = p1value - p2value;
                        float weight = p1value / delta;
                        var midPoint = fronttr + weight * (Vector3)(fronttl - fronttr); // Lerp formula

                        aggregatedPos += midPoint;
                    }
                    if (!SameSign(fronttl_value, frontbl_value))
                    {
                        edgeIntersections++;

                        var p1value = fronttl_value;
                        var p2value = frontbl_value;
                        float delta = p1value - p2value;
                        float weight = p1value / delta;
                        var midPoint = fronttl + weight * (Vector3)(frontbl - fronttl); // Lerp formula

                        aggregatedPos += midPoint;
                    }

                    //Back Edges
                    if (!SameSign(backbl_value, backbr_value))
                    {
                        edgeIntersections++;

                        var p1value = backbl_value;
                        var p2value = backbr_value;
                        float delta = p1value - p2value;
                        float weight = p1value / delta;
                        var midPoint = backbl + weight * (Vector3)(backbr - backbl); // Lerp formula

                        aggregatedPos += midPoint;

                    }
                    if (!SameSign(backbr_value, backtr_value))
                    {
                        edgeIntersections++;

                        var p1value = backbr_value;
                        var p2value = backtr_value;
                        float delta = p1value - p2value;
                        float weight = p1value / delta;
                        var midPoint = backbr + weight * (Vector3)(backtr - backbr); // Lerp formula

                        aggregatedPos += midPoint;

                    }
                    if (!SameSign(backtr_value, backtl_value))
                    {
                        edgeIntersections++;

                        var p1value = backtr_value;
                        var p2value = backtl_value;
                        float delta = p1value - p2value;
                        float weight = p1value / delta;
                        var midPoint = backtr + weight * (Vector3)(backtl - backtr); // Lerp formula

                        aggregatedPos += midPoint;

                    }
                    if (!SameSign(backtl_value, backbl_value))
                    {
                        edgeIntersections++;

                        var p1value = backtl_value;
                        var p2value = backbl_value;
                        float delta = p1value - p2value;
                        float weight = p1value / delta;
                        var midPoint = backtl + weight * (Vector3)(backbl - backtl); // Lerp formula

                        aggregatedPos += midPoint;
                    }

                    //Front->Back Edges
                    if (!SameSign(frontbl_value, backbl_value))
                    {
                        edgeIntersections++;

                        var p1value = frontbl_value;
                        var p2value = backbl_value;
                        float delta = p1value - p2value;
                        float weight = p1value / delta;
                        var midPoint = frontbl + weight * (Vector3)(backbl - frontbl); // Lerp formula

                        aggregatedPos += midPoint;

                    }
                    if (!SameSign(frontbr_value, backbr_value))
                    {
                        edgeIntersections++;

                        var p1value = frontbr_value;
                        var p2value = backbr_value;
                        float delta = p1value - p2value;
                        float weight = p1value / delta;
                        var midPoint = frontbr + weight * (Vector3)(backbr - frontbr); // Lerp formula

                        aggregatedPos += midPoint;

                    }
                    if (!SameSign(fronttr_value, backtr_value))
                    {
                        edgeIntersections++;

                        var p1value = fronttr_value;
                        var p2value = backtr_value;
                        float delta = p1value - p2value;
                        float weight = p1value / delta;
                        var midPoint = fronttr + weight * (Vector3)(backtr - fronttr); // Lerp formula

                        aggregatedPos += midPoint;

                    }
                    if (!SameSign(fronttl_value, backtl_value))
                    {
                        edgeIntersections++;

                        var p1value = fronttl_value;
                        var p2value = backtl_value;
                        float delta = p1value - p2value;
                        float weight = p1value / delta;
                        var midPoint = fronttl + weight * (Vector3)(backtl - fronttl); // Lerp formula

                        aggregatedPos += midPoint;
                    }

                    if (edgeIntersections == 0) //??
                    {
                        var averagePos = new Vector3(x + 0.5f, y + 0.5f, z + 0.5f);
                        Vector3 newVert = (averagePos + Zero) * (size / gridResolution) + originOffset;
                        vertices.Add(newVert);
                    }
                    else
                    {
                        var averagePos = aggregatedPos / edgeIntersections;
                        Vector3 newVert = (averagePos + Zero) * (size / gridResolution) + originOffset;
                        vertices.Add(newVert);
                    }
                }
            }
        }

        // Generate Triangles by connecting manually check
        for (int z = 1; z < gridResolution_Z; z++)
        {
            for (int y = 1; y < gridResolution_Y; y++)
            {
                for (int x = 1; x < gridResolution_X; x++)
                {
                    var vIndex = x + (y * gridResolution_X) + (z * gridResolution_X * gridResolution_Y);

                    //if x, connect y z?
                    var botEdgeP1 = bufferGrid[x, y, z];
                    var botEdgeP2 = bufferGrid[x + 1, y, z];
                    if (!SameSign(botEdgeP1, botEdgeP2))
                    {
                        // if not same sign means we need to connnect, but which order?
                        // check sign and flip accordingly?
                        if (botEdgeP1 < 0)
                        {
                            indices.Add(vIndex);
                            indices.Add(vIndex - gridResolution_X * gridResolution_Y);
                            indices.Add(vIndex - gridResolution_X - gridResolution_X * gridResolution_Y);
                            indices.Add(vIndex - gridResolution_X);
                        }
                        else
                        {
                            indices.Add(vIndex);
                            indices.Add(vIndex - gridResolution_X);
                            indices.Add(vIndex - gridResolution_X - gridResolution_X * gridResolution_Y);
                            indices.Add(vIndex - gridResolution_X * gridResolution_Y);
                        }
                    }

                    //if y, connect x z?
                    var leftEdgeP1 = bufferGrid[x, y, z];
                    var leftEdgeP2 = bufferGrid[x, y + 1, z];
                    if (!SameSign(leftEdgeP1, leftEdgeP2))
                    {
                        // if not same sign means we need to connnect, but which order?
                        // check sign and flip accordingly?
                        if (leftEdgeP1 < 0)
                        {
                            indices.Add(vIndex);
                            indices.Add(vIndex - 1);
                            indices.Add(vIndex - gridResolution_X * gridResolution_Y - 1);
                            indices.Add(vIndex - gridResolution_X * gridResolution_Y);
                        }
                        else
                        {
                            indices.Add(vIndex);
                            indices.Add(vIndex - gridResolution_X * gridResolution_Y);
                            indices.Add(vIndex - gridResolution_X * gridResolution_Y - 1);
                            indices.Add(vIndex - 1);
                        }
                    }

                    //if z, connect x y?
                    var forwardEdgeP1 = bufferGrid[x, y, z];
                    var forwardEdgeP2 = bufferGrid[x, y, z + 1];
                    if (!SameSign(forwardEdgeP1, forwardEdgeP2))
                    {
                        // if not same sign means we need to connnect, but which order?
                        // check sign and flip accordingly?
                        if (forwardEdgeP1 < 0)
                        {
                            indices.Add(vIndex);
                            indices.Add(vIndex - gridResolution_X);
                            indices.Add(vIndex - gridResolution_X - 1);
                            indices.Add(vIndex - 1);
                        }
                        else
                        {
                            indices.Add(vIndex);
                            indices.Add(vIndex - 1);
                            indices.Add(vIndex - gridResolution_X - 1);
                            indices.Add(vIndex - gridResolution_X);
                        }
                    }
                }
            }
        }

        return (vertices, indices);
    }


    public float [,,] GetBuffer()
    {
        return bufferGrid;
    }

    /*  This table represents all 8 corners of a cube as local offsets
	* 
	*  y         z
	*  ^        /     
	*  |
	*    7----6
	*   /|   /|
	*  3----2 |
	*  | 4--|-5
	*  |/   |/
	*  0----1   --> x
	* 
	*/
    static readonly Vector3Int[] CornerOffsetTable = new Vector3Int[8]
    {
        new Vector3Int(0, 0, 0),
        new Vector3Int(1, 0, 0),
        new Vector3Int(1, 1, 0),
        new Vector3Int(0, 1, 0),
        new Vector3Int(0, 0, 1),
        new Vector3Int(1, 0, 1),
        new Vector3Int(1, 1, 1),
        new Vector3Int(0, 1, 1)
    };


    bool SameSign(float a,float b)
    {
        return Mathf.Sign(a) == Mathf.Sign(b);
    }
}
