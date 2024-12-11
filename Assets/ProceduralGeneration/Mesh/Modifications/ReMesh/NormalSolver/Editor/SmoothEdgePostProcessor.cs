using UnityEngine;
using UnityEditor;

public class SmoothEdgePostProcessor : AssetPostprocessor {

	void OnPostprocessModel (GameObject g) {
		Apply(g.transform);
	}

	void Apply (Transform t) {
		MeshFilter filter = t.GetComponent<MeshFilter> ();
		if (filter != null && filter.sharedMesh != null)
			ModifyMesh (filter.sharedMesh);

		// Recurse
		foreach (Transform child in t)
			Apply(child);
	}

	void ModifyMesh (Mesh mesh) {
		NormalSolver.RecalculateNormals (mesh, 180, 0.5f);
	}
}
