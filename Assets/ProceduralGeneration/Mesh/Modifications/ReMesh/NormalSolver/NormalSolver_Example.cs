using UnityEngine;

public class NormalSolver_Example : MonoBehaviour
{
    [SerializeField] MeshFilter meshFilter;
    void Start()
    {
        if (meshFilter == null) return;

        meshFilter.mesh.RecalculateNormals(180, 0.1f);
    }
}
