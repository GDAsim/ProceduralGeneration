using System.Collections.Generic;
using UnityEngine;

public class MarchingSquares
{
    /// <summary>
    /// Data Grid / Voxel Grid / Sampling Buffer
    /// </summary>
    float[,] bufferGrid; 

    int gridResolution;

    public void Setup(int gridResolution, float[,] inBuffer)
    {
        this.gridResolution = gridResolution;
        bufferGrid = inBuffer;
    }
    public List<(Vector3 p1,Vector3 p2)> MarchSquares_Lines(Vector3 vertexOffset, float binaryThreshold, float scale)
    {
        var lines = new List<(Vector3 p1, Vector3 p2)>();

        for (int x = 0; x < gridResolution; x++)
        {
            for (int y = 0; y < gridResolution; y++)
            {
                var bl = bufferGrid[x, y] < binaryThreshold ? 0 : 1;
                var br = bufferGrid[x + 1, y] < binaryThreshold ? 0 : 1;
                var tr = bufferGrid[x + 1, y + 1] < binaryThreshold ? 0 : 1;
                var tl = bufferGrid[x, y + 1] < binaryThreshold ? 0 : 1;
                int bitflag = tl * 8 + tr * 4 + br * 2 + bl * 1;

                var pos = new Vector3(x, y, 0);

                List<(Vector3 p1, Vector3 p2)> localLines = new List<(Vector3 p1, Vector3 p2)>();
                switch (bitflag)
                {
                    case 1:
                        localLines.Add((new Vector3(0, 0.5f, 0), new Vector3(0.5f, 0, 0)));
                        break;
                    case 2:
                        localLines.Add((new Vector3(0.5f, 0), new Vector3(1f, 0.5f)));
                        break;
                    case 3:
                        localLines.Add((new Vector3(0f, 0.5f), new Vector3(1f, 0.5f)));
                        break;
                    case 4:
                        localLines.Add((new Vector3(0.5f, 1f), new Vector3(1f, 0.5f)));
                        break;
                    case 5:
                        localLines.Add((new Vector3(0f, 0.5f), new Vector3(0.5f, 0f)));
                        localLines.Add((new Vector3(0.5f, 1f), new Vector3(1f, 0.5f)));
                        break;
                    case 6:
                        localLines.Add((new Vector3(0.5f, 0f), new Vector3(0.5f, 1f)));
                        break;
                    case 7:
                        localLines.Add((new Vector3(0f, 0.5f), new Vector3(0.5f, 1f)));
                        break;
                    case 8:
                        localLines.Add((new Vector3(0f, 0.5f), new Vector3(0.5f, 1f)));
                        break;
                    case 9:
                        localLines.Add((new Vector3(0.5f, 0f), new Vector3(0.5f, 1f)));
                        break;
                    case 10:
                        localLines.Add((new Vector3(0f, 0.5f), new Vector3(0.5f, 1f)));
                        localLines.Add((new Vector3(0.5f, 0f), new Vector3(1f, 0.5f)));
                        break;
                    case 11:
                        localLines.Add((new Vector3(0.5f, 1f), new Vector3(1f, 0.5f)));
                        break;
                    case 12:
                        localLines.Add((new Vector3(0f, 0.5f), new Vector3(1f, 0.5f)));
                        break;
                    case 13:
                        localLines.Add((new Vector3(0.5f, 0), new Vector3(1f, 0.5f)));
                        break;
                    case 14:
                        localLines.Add((new Vector3(0f, 0.5f), new Vector3(0.5f, 0f)));
                        break;
                    case 15:
                        break;
                }

                foreach (var localline in localLines)
                {
                    var line = localline;
                    line.p1 = (line.p1 + pos) * scale;
                    line.p2 = (line.p2 + pos) * scale;
                    lines.Add(line);
                }
            }
        }

        return lines;
    }
    public List<(Vector3 p1,Vector3 p2)> MarchSquaresInterpolate_Lines(Vector3 vertexOffset, float binaryThreshold, float scale)
    {
        var lines = new List<(Vector3 p1, Vector3 p2)>();

        for (int x = 0; x < gridResolution; x++)
        {
            for (int y = 0; y < gridResolution; y++)
            {
                var bl_value = bufferGrid[x, y];
                var br_value = bufferGrid[x + 1, y];
                var tr_value = bufferGrid[x + 1, y + 1];
                var tl_value = bufferGrid[x, y + 1];

                var bl = bufferGrid[x, y] < binaryThreshold ? 0 : 1;
                var br = bufferGrid[x + 1, y] < binaryThreshold ? 0 : 1;
                var tr = bufferGrid[x + 1, y + 1] < binaryThreshold ? 0 : 1;
                var tl = bufferGrid[x, y + 1] < binaryThreshold ? 0 : 1;
                int bitflag = tl * 8 + tr * 4 + br * 2 + bl * 1;

                var pos = new Vector3(x, y, 0);

                float basevalue = binaryThreshold;

                List<(Vector3 p1, Vector3 p2)> localLines = new List<(Vector3 p1, Vector3 p2)>();
                switch (bitflag)
                {
                    case 1 or 14:
                        localLines.Add(
                            (new Vector3(0, MathExtensions.remap(basevalue, bl_value, tl_value, 0, 1), 0),
                            new Vector3(MathExtensions.remap(basevalue, bl_value, br_value, 0, 1), 0, 0)));
                        break;
                    case 2 or 13:
                        localLines.Add(
                            (new Vector3(MathExtensions.remap(basevalue, br_value, bl_value, 1, 0), 0),
                            new Vector3(1f, MathExtensions.remap(basevalue, br_value, tr_value, 0, 1))));
                        break;
                    case 3 or 12:
                        localLines.Add(
                            (new Vector3(0f, MathExtensions.remap(basevalue, bl_value, tl_value, 0, 1)),
                            new Vector3(1f, MathExtensions.remap(basevalue, br_value, tr_value, 0, 1))));
                        break;
                    case 4 or 11:
                        localLines.Add(
                            (new Vector3(MathExtensions.remap(basevalue, tr_value, tl_value, 1, 0), 1f),
                            new Vector3(1f, MathExtensions.remap(basevalue, tr_value, br_value, 1, 0))));
                        break;
                    case 5:
                        localLines.Add(
                            (new Vector3(0f, MathExtensions.remap(basevalue, bl_value, tl_value, 0, 1)),
                            new Vector3(MathExtensions.remap(basevalue, bl_value, br_value, 0, 1), 0f)));

                        localLines.Add(
                            (new Vector3(MathExtensions.remap(basevalue, tr_value, tl_value, 1, 0), 1f),
                            new Vector3(1f, MathExtensions.remap(basevalue, tr_value, br_value, 1, 0))));
                        break;
                    case 6 or 9:
                        localLines.Add(
                            (new Vector3(MathExtensions.remap(basevalue, br_value, bl_value, 1, 0), 0f),
                            new Vector3(MathExtensions.remap(basevalue, tr_value, tl_value, 1, 0), 1f)));
                        break;
                    case 7 or 8:
                        localLines.Add(
                            (new Vector3(0f, MathExtensions.remap(basevalue, bl_value, tl_value, 0, 1)),
                            new Vector3(MathExtensions.remap(basevalue, tr_value, tl_value, 1, 0), 1f)));
                        break;
                    case 10:
                        localLines.Add(
                            (new Vector3(0f, MathExtensions.remap(basevalue, tl_value, bl_value, 1, 0)),
                            new Vector3(MathExtensions.remap(basevalue, br_value, bl_value, 1, 0), 1f)));

                        localLines.Add(
                            (new Vector3(MathExtensions.remap(basevalue, tl_value, tr_value, 0, 1), 0f),
                            new Vector3(1f, MathExtensions.remap(basevalue, br_value, tr_value, 0, 1))));
                        break;
                    case 15:
                        break;
                }

                foreach (var localline in localLines)
                {
                    var line = localline;
                    line.p1 = (line.p1 + pos) * scale;
                    line.p2 = (line.p2 + pos) * scale;
                    lines.Add(line);
                }
            }
        }

        return lines;
    }
    public (List<Vector3> verts,List<int> indices) MarchSquares(Vector3 vertexOffset,float binaryThreshold, float scale)
    {
        var vertices = new List<Vector3>();
        var indices = new List<int>();

        for (int x = 0; x < gridResolution; x++)
        {
            for (int y = 0; y < gridResolution; y++)
            {
                // Calculate Bitflag
                var bl = bufferGrid[x, y] < binaryThreshold ? 0 : 1;
                var br = bufferGrid[x + 1, y] < binaryThreshold ? 0 : 1;
                var tr = bufferGrid[x + 1, y + 1] < binaryThreshold ? 0 : 1;
                var tl = bufferGrid[x, y + 1] < binaryThreshold ? 0 : 1;
                int bitflag = tl * 8 + tr * 4 + br * 2 + bl * 1;

                var pos = new Vector3(x, y, 0);

                int startIndex = vertices.Count;
                Vector3[] verts = new Vector3[6];
                int[] triangle = new int[6];

                switch (bitflag)
                {
                    case 0 or 15:
                        break;
                    case 1:
                        verts = new Vector3[]
                        {
                            new(0, 0),
                            new(0, 0.5f),
                            new(0.5f, 0)
                        };

                        triangle = new int[]
                        {
                            0, 1, 2
                        };

                        break;
                    case 2:
                        verts = new Vector3[]
                        {
                            new(0.5f, 0),
                            new(1, 0f),
                            new(1f, 0.5f)
                        };

                        triangle = new int[]
                        {
                            0, 2, 1
                        };

                        break;
                    case 3:
                        verts = new Vector3[]
                        {
                            new(0, 0f),
                            new(0, 0.5f),
                            new(1f, 0f),
                            new(1f, 0.5f)
                        };

                        triangle = new int[]
                        {
                            0, 1, 2,
                            2, 1, 3
                        };

                        break;
                    case 4:
                        verts = new Vector3[]
                        {
                            new(0.5f, 1f),
                            new(1, 0.5f),
                            new(1f, 1f),
                        };

                        triangle = new int[]
                        {
                            0, 2, 1
                        };

                        break;
                    case 5:
                        verts = new Vector3[]
                         {
                            new(0, 0),
                            new(0, 0.5f),
                            new(0.5f, 0),

                            new(0.5f, 1),
                            new(1, 0.5f),
                            new(1, 1),
                         };

                        triangle = new int[]
                        {
                            0, 1, 2,
                            3, 5, 4
                        };

                        break;
                    case 6:
                        verts = new Vector3[]
                        {
                            new(0.5f, 0),
                            new(0.5f, 1),
                            new(1, 0),
                            new(1, 1)
                        };

                        triangle = new int[]
                        {
                            0, 1, 2,
                            2, 1, 3
                        };

                        break;
                    case 7:
                        verts = new Vector3[]
                        {
                            new(0, 0),
                            new(0, 0.5f),
                            new(0.5f, 1f),
                            new(1, 0f),
                            new(1f, 1f),
                        };

                        triangle = new int[]
                        {
                            3, 0, 1,
                            3, 1, 2,
                            3, 2, 4
                        };

                        break;
                    case 8:
                        verts = new Vector3[]
                        {
                            new(0, 0.5f),
                            new(0, 1),
                            new(0.5f, 1)
                        };

                        triangle = new int[]
                        {
                            0, 1, 2
                        };

                        break;
                    case 9:
                        verts = new Vector3[]
                        {
                            new(0, 0),
                            new(0, 1),
                            new(0.5f, 0),
                            new(0.5f, 1),
                        };

                        triangle = new int[]
                        {
                            0, 1, 3,
                            3, 2, 0
                        };

                        break;
                    case 10:
                        verts = new Vector3[]
                        {
                            new(0, 0.5f),
                            new(0, 1),
                            new(0.5f, 0),
                            new(0.5f, 1),
                            new(1, 0),
                            new(1, 0.5f)
                        };

                        triangle = new int[]
                        {
                            0, 1, 3,
                            2, 5, 4
                        };


                        break;
                    case 11:
                        verts = new Vector3[]
                        {
                            new(0, 0),
                            new(0, 1),
                            new(0.5f, 1),
                            new(1f, 0),
                            new(1, 0.5f),
                        };

                        triangle = new int[]
                        {
                            0, 1, 2,
                            0, 2, 4,
                            0, 4, 3
                        };

                        break;
                    case 12:
                        verts = new Vector3[]
                        {
                            new(0, 0.5f),
                            new(0, 1),
                            new(1, 0.5f),
                            new(1, 1f)
                        };

                        triangle = new int[]
                        {
                            0, 1, 3,
                            3, 2, 0
                        };

                        break;

                    case 13:
                        verts = new Vector3[]
                        {
                            new(0, 0),
                            new(0, 1),
                            new(0.5f, 0),
                            new(1, 0.5f),
                            new(1, 1)
                        };

                        triangle = new int[]
                        {
                            1, 2, 0,
                            1, 3, 2,
                            1, 4, 3,
                        };

                        break;

                    case 14:
                        verts = new Vector3[]
                        {
                            new(0, 0.5f),
                            new(0, 1f),
                            new(0.5f, 0),
                            new(1f, 0f),
                            new(1f, 1f)
                        };

                        triangle = new int[]
                        {
                            4, 3, 2,
                            4, 2, 0,
                            4, 0, 1
                        };

                        break;
                    case 15:
                        verts = new Vector3[]
                        {
                            new Vector3(0, 0),
                            new Vector3(0, 1),
                            new Vector3(1, 0),
                            new Vector3(1, 1)
                        };

                        triangle = new int[]
                        {
                            0, 1, 3,
                            3, 2, 0
                        };

                        break;
                }

                foreach (Vector3 v in verts)
                {
                    Vector3 newVert = (v + pos) * scale;

                    vertices.Add(newVert + vertexOffset);
                }

                foreach (int tri in triangle)
                {
                    indices.Add(startIndex + tri);
                }
            }
        }

        return (vertices, indices);
    }
    public (List<Vector3> verts, List<int> indices) MarchSquaresInterpolate(Vector3 vertexOffset, float binaryThreshold, float scale)
    {
        var vertices = new List<Vector3>();
        var indices = new List<int>();

        for (int x = 0; x < gridResolution; x++)
        {
            for (int y = 0; y < gridResolution; y++)
            {
                var bl_value = bufferGrid[x, y];
                var br_value = bufferGrid[x + 1, y];
                var tr_value = bufferGrid[x + 1, y + 1];
                var tl_value = bufferGrid[x, y + 1];

                var bl = bufferGrid[x, y] < binaryThreshold ? 0 : 1;
                var br = bufferGrid[x + 1, y] < binaryThreshold ? 0 : 1;
                var tr = bufferGrid[x + 1, y + 1] < binaryThreshold ? 0 : 1;
                var tl = bufferGrid[x, y + 1] < binaryThreshold ? 0 : 1;
                int bitflag = tl * 8 + tr * 4 + br * 2 + bl * 1;

                var pos = new Vector3(x, y, 0);

                int startIndex = vertices.Count;
                Vector3[] verts = new Vector3[6];
                int[] triangle = new int[6];

                float basevalue = binaryThreshold;

                switch (bitflag)
                {
                    case 1:

                        verts = new Vector3[]
                        {
                            new Vector2(0, 0),
                            new Vector2(0, MathExtensions.remap(basevalue, bl_value, tl_value, 0, 1)),
                            new Vector2(MathExtensions.remap(basevalue, bl_value, br_value, 0, 1), 0),
                        };

                        triangle = new int[]
                        {
                            0, 1, 2
                        };

                        break;
                    case 2:
                        verts = new Vector3[]
                        {
                            new Vector2(MathExtensions.remap(basevalue, br_value, bl_value, 1, 0), 0),
                            new Vector2(1, 0),
                            new Vector2(1, MathExtensions.remap(basevalue, br_value, tr_value, 0, 1)),
                        };

                        triangle = new int[]
                        {
                            0, 2, 1
                        };

                        break;
                    case 3:
                        verts = new Vector3[]
                        {
                            new Vector2(0, 0),
                            new Vector2(0, MathExtensions.remap(basevalue, bl_value, tl_value, 0, 1)),
                            new Vector2(1, 0),
                            new Vector2(1, MathExtensions.remap(basevalue, br_value, tr_value, 0, 1)),
                        };

                        triangle = new int[]
                        {
                            0, 1, 2,
                            2, 1, 3
                        };

                        break;
                    case 4:
                        verts = new Vector3[]
                        {
                            new Vector2(MathExtensions.remap(basevalue, tr_value, tl_value, 1, 0), 1),
                            new Vector2(1, MathExtensions.remap(basevalue, tr_value, br_value, 1, 0)),
                            new Vector2(1, 1),
                        };

                        triangle = new int[]
                        {
                            0, 2, 1
                        };

                        break;
                    case 5:
                        verts = new Vector3[]
                        {
                            new Vector2(0, 0),
                            new Vector2(0, MathExtensions.remap(basevalue, bl_value, tl_value, 0, 1)),
                            new Vector2(MathExtensions.remap(basevalue, bl_value, br_value, 0, 1), 0),

                            new Vector2(MathExtensions.remap(basevalue, tr_value, tl_value, 1, 0), 1),
                            new Vector2(1, MathExtensions.remap(basevalue, tr_value, br_value, 1, 0)),
                            new Vector2(1, 1),
                        };

                        triangle = new int[]
                        {
                            0, 1, 2,
                            3, 5, 4
                        };

                        break;
                    case 6:
                        verts = new Vector3[]
                        {
                            new(MathExtensions.remap(basevalue, br_value, bl_value, 1, 0), 0),
                            new(MathExtensions.remap(basevalue, tr_value, tl_value, 1, 0), 1),
                            new(1, 0),
                            new(1, 1)
                        };

                        triangle = new int[]
                        {
                            0, 1, 2,
                            2, 1, 3
                        };

                        break;
                    case 7:
                        verts = new Vector3[]
                        {
                            new(0, 0),
                            new(0, MathExtensions.remap(basevalue, bl_value, tl_value, 0, 1)),
                            new(MathExtensions.remap(basevalue, tr_value, tl_value, 1, 0), 1),
                            new(1, 0),
                            new(1, 1),
                        };

                        triangle = new int[]
                        {
                            3, 0, 1,
                            3, 1, 2,
                            3, 2, 4
                        };

                        break;
                    case 8:
                        verts = new Vector3[]
                        {
                            new(0, MathExtensions.remap(basevalue, tl_value, bl_value, 1, 0)),
                            new(0, 1),
                            new(MathExtensions.remap(basevalue, tl_value, tr_value, 0, 1), 1)
                        };

                        triangle = new int[]
                        {
                            0, 1, 2
                        };

                        break;
                    case 9:
                        verts = new Vector3[]
                        {
                            new(0, 0),
                            new(0, 1),
                            new(MathExtensions.remap(basevalue, bl_value, br_value, 0, 1), 0),
                            new(MathExtensions.remap(basevalue, tl_value, tr_value, 0, 1), 1),
                        };

                        triangle = new int[]
                        {
                            0, 1, 3,
                            3, 2, 0
                        };

                        break;
                    case 10:
                        verts = new Vector3[]
                        {
                            new(0, MathExtensions.remap(basevalue, tl_value, bl_value, 1, 0)),
                            new(0, 1),
                            new(MathExtensions.remap(basevalue, br_value, bl_value, 1, 0), 0),

                            new(MathExtensions.remap(basevalue, tl_value, tr_value, 0, 1), 1),
                            new(1, 0),
                            new(1, MathExtensions.remap(basevalue, br_value, tr_value, 0, 1))
                        };

                        triangle = new int[]
                        {
                            0, 1, 3,
                            2, 5, 4
                        };


                        break;
                    case 11:
                        verts = new Vector3[]
                        {
                            new(0, 0),
                            new(0, 1),
                            new(MathExtensions.remap(basevalue, tl_value, tr_value, 0, 1), 1),
                            new(1, 0),
                            new(1, MathExtensions.remap(basevalue, br_value, tr_value, 0, 1)),
                        };

                        triangle = new int[]
                        {
                            0, 1, 2,
                            0, 2, 4,
                            0, 4, 3
                        };

                        break;
                    case 12:
                        verts = new Vector3[]
                        {
                            new(0, MathExtensions.remap(basevalue, tl_value, bl_value, 1, 0)),
                            new(0, 1),
                            new(1, MathExtensions.remap(basevalue, tr_value, br_value, 1, 0)),
                            new(1, 1f)
                        };

                        triangle = new int[]
                        {
                            0, 1, 3,
                            3, 2, 0
                        };

                        break;

                    case 13:
                        verts = new Vector3[]
                        {
                            new(0, 0),
                            new(0, 1),
                            new(MathExtensions.remap(basevalue, bl_value, br_value, 0, 1), 0),
                            new(1, MathExtensions.remap(basevalue, tr_value, br_value, 1, 0)),
                            new(1, 1)
                        };

                        triangle = new int[]
                        {
                            1, 2, 0,
                            1, 3, 2,
                            1, 4, 3,
                        };

                        break;

                    case 14:
                        verts = new Vector3[]
                        {
                            new(0, MathExtensions.remap(basevalue, tl_value, bl_value, 1, 0)),
                            new(0, 1),
                            new(MathExtensions.remap(basevalue, br_value, bl_value, 1, 0), 0),
                            new(1, 0),
                            new(1, 1)
                        };

                        triangle = new int[]
                        {
                            4, 3, 2,
                            4, 2, 0,
                            4, 0, 1
                        };

                        break;
                    case 15:
                        verts = new Vector3[]
                        {
                            new Vector3(0, 0),
                            new Vector3(0, 1),
                            new Vector3(1, 0),
                            new Vector3(1, 1)
                        };

                        triangle = new int[]
                        {
                            0, 1, 3,
                            3, 2, 0
                        };

                        break;
                }

                foreach (Vector3 v in verts)
                {
                    Vector3 newVert = (v + pos) * scale;

                    vertices.Add(newVert + vertexOffset);
                }

                foreach (int tri in triangle)
                {
                    indices.Add(startIndex + tri);
                }
            }
        }

        return (vertices, indices);
    }

    public float[,] GetBuffer()
    {
        return bufferGrid;
    }
}
