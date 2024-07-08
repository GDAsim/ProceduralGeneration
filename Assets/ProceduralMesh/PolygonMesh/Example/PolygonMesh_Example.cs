using UnityEngine;

[RequireComponent( typeof( PolygonCollider2D ), typeof( MeshFilter ), typeof( MeshRenderer ) )]
[ExecuteInEditMode]
public class PolyWall : MonoBehaviour 
{
	Vector2[] prevPoints;
    MeshFilter meshFilter;
    PolygonCollider2D polygonCollider2D;

    void Awake()
    {
        meshFilter = GetComponent<MeshFilter>();
        polygonCollider2D = GetComponent<PolygonCollider2D>();
    }

    void Update()
    {
		if (prevPoints != polygonCollider2D.points)
		{
            meshFilter.mesh = PolygonToMesh();
            prevPoints = polygonCollider2D.points;
		}
	}

    Mesh PolygonToMesh()
	{
		var points = polygonCollider2D.points;
		Vector3[] vertices = new Vector3[points.Length];
		for(int i = 0; i < vertices.Length; i++)
        {
			vertices[i] = points[i];
		}

        int[] triangles = TriangulatePoints.Triangulate(points);

        Mesh mesh = new();
        mesh.vertices = vertices;
		mesh.triangles = triangles;
		return mesh;
	}
}