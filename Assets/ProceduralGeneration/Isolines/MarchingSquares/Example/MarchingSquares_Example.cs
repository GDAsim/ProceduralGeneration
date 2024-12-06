using UnityEngine;

/// <summary>
/// 
/// Refrences:
/// https://en.wikipedia.org/wiki/Marching_squares
/// https://jamie-wong.com/2014/08/19/metaballs-and-marching-squares/
/// https://catlikecoding.com/unity/tutorials/marching-squares/
/// 
/// </summary>

[ExecuteInEditMode]
public class MarchingSquares_Example : MonoBehaviour
{
    [Header("Marching Squares")]
    [Range(0, 100)] public int gridResolution = 100;
    [Range(0, 10)] public float gridSize = 1;
    [Range(0, 1)] public float BinaryThreshold = 0.5f;
    public bool Interpolate = false;

    [Header("Noise")]
    [Range(0.01f, 1f)] public float noiseResolution = 1;

    [Header("Mesh")]
    [SerializeField] MeshFilter meshFilter;

    [Header("Debug")]
    float gridPointSize = 0.5f; // Percentage of gridResolution

    MarchingSquares ms = new();

    void Update()
    {
        float[,] inBuffer;
        inBuffer = new float[gridResolution + 1, gridResolution + 1];
        for (int x = 0; x <= gridResolution; x++)
        {
            for (int y = 0; y <= gridResolution; y++)
            {
                inBuffer[x, y] = Mathf.PerlinNoise(Time.time + ((x + Mathf.Epsilon) * noiseResolution ), Time.time + ((y + Mathf.Epsilon) * noiseResolution ));
            }
        }

        ms.Setup(gridResolution, inBuffer);

        //ms.Setup(gridResolution, new Vector2(Time.time, Time.time), 0.2f);

        //ms.Setup(gridResolution, (float posX, float posY) =>
        //{
        //    posX -= gridResolution / 2;
        //    posY -= gridResolution / 2;

        //    float r = 5f;
        //    float x = posX;
        //    float y = posY;

        //    return (r * r - x * x - y * y);
        //});

        // Run
        if (Interpolate)
        {
            ms.MarchSquaresInterpolate(Vector3.zero, BinaryThreshold, gridSize);
        }
        else
        {
            ms.MarchSquares(Vector3.zero, BinaryThreshold, gridSize);
        }

        // Extract
        var vertices = ms.GetVerticies();
        var indices = ms.GetIndices();

        // Create Mesh
        if (meshFilter == null) return;

        Mesh mesh = new();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = indices.ToArray();
        meshFilter.mesh = mesh;
    }

    void OnDrawGizmos()
    {
        var pos = transform.position;
        var pointSize = gridPointSize / 2 * gridSize;

        DrawDebugGridDots(pos, pointSize, ms.GetBuffer());
    }

    void DrawDebugGridDots(Vector3 pos, float size, float[,] buffer)
    {
        if (buffer == null) return;

        for (int x = 0; x <= gridResolution; x++)
        {
            for (int y = 0; y <= gridResolution; y++)
            {
                var offset = new Vector3(x, y, 0) * gridSize;
                Gizmos.color = new Color(1 - buffer[x, y], 1 - buffer[x, y], 1 - buffer[x, y]);
                Gizmos.DrawSphere(pos + offset, size);
            }
        }
    }
}
