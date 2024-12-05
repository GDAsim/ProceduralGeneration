using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// Refrences:
/// https://en.wikipedia.org/wiki/Marching_squares
/// https://jamie-wong.com/2014/08/19/metaballs-and-marching-squares/
/// https://catlikecoding.com/unity/tutorials/marching-squares/
/// 
/// 
/// </summary>
public class MarchingSquares_Test : MonoBehaviour
{
    [Header("Grid")]
    const int gridSize = 1;
    [SerializeField, Range(0, 10)] float gridResolution = 1;
    [SerializeField, Range(0, 1)] float gridPointSize = 0.5f; // Percentage of gridResolution

    [Header("Sampling")]
    public float binarythreshold = 0.5f;
    public bool UseSampling = true;

    [Header("Test")]
    [Range(0, 15)] public int CaseNum = 0;

    [SerializeField] MeshFilter meshFilter;

    float[,] bufferGrid;
    List<Vector3> vertices = new();
    List<int> indices = new();
    void OnDrawGizmos()
    {
        BuildDataGrid();

        var pos = transform.position;
        var pointSize = gridPointSize / 2 * gridResolution;
        for (int x = 0; x <= gridSize; x++)
        {
            for (int y = 0; y <= gridSize; y++)
            {
                var offset = new Vector3(x, y, 0) * gridResolution;
                Gizmos.color = new Color(1-bufferGrid[x, y], 1 - bufferGrid[x, y], 1 - bufferGrid[x, y]);
                Gizmos.DrawSphere(pos+offset, pointSize);
            }
        }

        if (meshFilter == null) return;

        MarchSquares(Vector3.zero);

        Mesh mesh = new();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = indices.ToArray();
        meshFilter.mesh = mesh;
    }

    void MarchSquares(Vector3 vertexOffset, bool interpolate = true)
    {
        vertices.Clear();
        indices.Clear();

        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
            {
                var bl_value = bufferGrid[x, y];
                var br_value = bufferGrid[x + 1, y];
                var tr_value = bufferGrid[x + 1, y + 1];
                var tl_value = bufferGrid[x, y + 1];

                var bl = bufferGrid[x, y] < binarythreshold ? 0 : 1;
                var br = bufferGrid[x + 1, y] < binarythreshold ? 0 : 1;
                var tr = bufferGrid[x + 1, y + 1] < binarythreshold ? 0 : 1;
                var tl = bufferGrid[x, y + 1] < binarythreshold ? 0 : 1;
                int bitflag = tl * 8 + tr * 4 + br * 2 + bl * 1;

                var pos = new Vector3(x, y, 0);

                int startIndex = vertices.Count;
                Vector3[] verts = new Vector3[6];
                int[] triangle = new int[6];

                float basevalue = binarythreshold;

                switch (bitflag)
                {
                    case 1:

                        verts = new Vector3[]
                        {
                            new Vector2(0, 0),
                            new Vector2(0, MathExtensions.remap(basevalue, bl_value, tl_value, 1, 0)),
                            new Vector2(MathExtensions.remap(basevalue, bl_value, br_value, 1, 0), 0),
                        };

                        triangle = new int[]
                        {
                            0, 1, 2
                        };

                        break;
                    case 2:
                        verts = new Vector3[]
                        {
                            new Vector2(MathExtensions.remap(basevalue, 1, 0, 0, 1), 0),
                            new Vector2(1, 0),
                            new Vector2(1, MathExtensions.remap(basevalue, 1, 0, 1, 0)),
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
                            new Vector2(0, MathExtensions.remap(basevalue, bl_value, tl_value, 1, 0)),
                            new Vector2(1, 0),
                            new Vector2(1, MathExtensions.remap(basevalue, br_value, tr_value, 1, 0)),
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
                            new Vector2(MathExtensions.remap(basevalue, tr_value, tl_value, 0, 1), 1),
                            new Vector2(1, MathExtensions.remap(basevalue, tr_value, bl_value, 0, 1)),
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
                            new Vector2(0, MathExtensions.remap(basevalue, bl_value, tl_value, 1, 0)),
                            new Vector2(MathExtensions.remap(basevalue, bl_value, br_value, 1, 0), 0),

                            new Vector2(MathExtensions.remap(basevalue, tr_value, tl_value, 0, 1), 1),
                            new Vector2(1, MathExtensions.remap(basevalue, tr_value, br_value, 0, 1)),
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
                            new(MathExtensions.remap(basevalue, br_value, bl_value, 0, 1), 0),
                            new(MathExtensions.remap(basevalue, tr_value, tl_value, 0, 1), 1),
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
                            new(0, MathExtensions.remap(basevalue, bl_value, tl_value, 1, 0)),
                            new(MathExtensions.remap(basevalue, tr_value, tl_value, 0, 1), 1),
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
                            new(0, MathExtensions.remap(basevalue, tl_value, bl_value, 0, 1)),
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
                            new(0, MathExtensions.remap(basevalue, tl_value, bl_value, 0, 1)),
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
                            new(0, MathExtensions.remap(basevalue, tl_value, bl_value, 0, 1)),
                            new(0, 1),
                            new(1, MathExtensions.remap(basevalue, tr_value, br_value, 0, 1)),
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
                    Vector3 newVert = (v + pos) * gridResolution;

                    vertices.Add(newVert + vertexOffset);
                }

                foreach (int tri in triangle)
                {
                    indices.Add(startIndex + tri);
                }
            }
        }
    }
    
    void BuildDataGrid()
    {
        bufferGrid = new float[gridSize+1, gridSize+1];

        switch (CaseNum)
        {
            case 0:
                bufferGrid[0, 0] = 0;
                bufferGrid[1, 0] = 0;
                bufferGrid[1, 1] = 0;
                bufferGrid[0, 1] = 0;
                break;
            case 1:
                bufferGrid[0, 0] = 1;
                bufferGrid[1, 0] = 0;
                bufferGrid[1, 1] = 0;
                bufferGrid[0, 1] = 0;
                break;
            case 2:
                bufferGrid[0, 0] = 0;
                bufferGrid[1, 0] = 1;
                bufferGrid[1, 1] = 0;
                bufferGrid[0, 1] = 0;
                break;
            case 3:
                bufferGrid[0, 0] = 1;
                bufferGrid[1, 0] = 1;
                bufferGrid[1, 1] = 0;
                bufferGrid[0, 1] = 0;
                break;
            case 4:
                bufferGrid[0, 0] = 0;
                bufferGrid[1, 0] = 0;
                bufferGrid[1, 1] = 1;
                bufferGrid[0, 1] = 0;
                break;
            case 5:
                bufferGrid[0, 0] = 1;
                bufferGrid[1, 0] = 0;
                bufferGrid[1, 1] = 1;
                bufferGrid[0, 1] = 0;
                break;
            case 6:
                bufferGrid[0, 0] = 0;
                bufferGrid[1, 0] = 1;
                bufferGrid[1, 1] = 1;
                bufferGrid[0, 1] = 0;
                break;
            case 7:
                bufferGrid[0, 0] = 1;
                bufferGrid[1, 0] = 1;
                bufferGrid[1, 1] = 1;
                bufferGrid[0, 1] = 0;
                break;
            case 8:
                bufferGrid[0, 0] = 0;
                bufferGrid[1, 0] = 0;
                bufferGrid[1, 1] = 0;
                bufferGrid[0, 1] = 1;
                break;
            case 9:
                bufferGrid[0, 0] = 1;
                bufferGrid[1, 0] = 0;
                bufferGrid[1, 1] = 0;
                bufferGrid[0, 1] = 1;
                break;
            case 10:
                bufferGrid[0, 0] = 0;
                bufferGrid[1, 0] = 1;
                bufferGrid[1, 1] = 0;
                bufferGrid[0, 1] = 1;
                break;
            case 11:
                bufferGrid[0, 0] = 1;
                bufferGrid[1, 0] = 1;
                bufferGrid[1, 1] = 0;
                bufferGrid[0, 1] = 1;
                break;
            case 12:
                bufferGrid[0, 0] = 0;
                bufferGrid[1, 0] = 0;
                bufferGrid[1, 1] = 1;
                bufferGrid[0, 1] = 1;
                break;
            case 13:
                bufferGrid[0, 0] = 1;
                bufferGrid[1, 0] = 0;
                bufferGrid[1, 1] = 1;
                bufferGrid[0, 1] = 1;
                break;
            case 14:
                bufferGrid[0, 0] = 0;
                bufferGrid[1, 0] = 1;
                bufferGrid[1, 1] = 1;
                bufferGrid[0, 1] = 1;
                break;
            case 15:
                bufferGrid[0, 0] = 1;
                bufferGrid[1, 0] = 1;
                bufferGrid[1, 1] = 1;
                bufferGrid[0, 1] = 1;
                break;
        }
       
    }
}
