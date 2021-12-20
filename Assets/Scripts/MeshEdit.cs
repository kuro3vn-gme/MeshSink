using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshEdit : MonoBehaviour
{
	[SerializeField] private float range;
	[SerializeField] private float pow;
	[SerializeField] private MeshFilter meshFilter;
	[SerializeField] private List<Transform> referenceTransforms;

	private Mesh originalMesh;
	private Vector3[] vertices;

	private void Start()
	{
		originalMesh = new Mesh();
		originalMesh.vertices = meshFilter.sharedMesh.vertices;
		originalMesh.normals = meshFilter.sharedMesh.normals;
		vertices = new Vector3[originalMesh.vertices.Length];
		StartCoroutine(TestCoroutine());
	}

	private IEnumerator TestCoroutine()
	{
		while (true)
		{
			Edit();
			yield return null;
		}
	}

	private void Edit()
	{
		var mesh = meshFilter.mesh;
		for(var i = 0; i < originalMesh.vertices.Length; i++)
		{
			var vertice = originalMesh.vertices[i];
			if (vertice == Vector3.zero)
			{
				vertices[i] = vertice;
				continue;
			}

			var newVertice = vertice;
			var normal = originalMesh.normals[i];
			foreach (var referenceTransform in referenceTransforms)
			{
				var refVec = referenceTransform.position - transform.position;
				var dot = Vector3.Dot(refVec.normalized, normal);
				if (dot < range) continue;

				if (newVertice.sqrMagnitude - refVec.sqrMagnitude < 0) continue;

				var sinkAmmount = newVertice.magnitude - refVec.magnitude;
				var vec = -normal;
				var rate = Mathf.Clamp01((dot - range) / (1 - range));
				var ammount = sinkAmmount * Mathf.Pow(rate, pow);

				newVertice = newVertice + vec.normalized * ammount;
			}
			vertices[i] = newVertice;
		}

		mesh.vertices = vertices;
		mesh.RecalculateNormals();
	}
}
