using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class TriangleMeshRenderer : MonoBehaviour
{
	private Mesh mesh;
	/*List<Vector3> vertices = new List<Vector3>();
	List<int> triangles = new List<int>();*/
	Vector3[] vertices = new Vector3[65535];
	private int[] triangles = new int[65535];

	//private int scansCount = 0;
	private int index = 0;
	private float vertexOffset = 0.5f;

	[SerializeField]
	private float triangleSize = 0.1f;

	private void Start()
	{
		mesh = GetComponent<MeshFilter>().mesh;
		UpdateMesh();
	}

	private void UpdateMesh()
	{
		mesh.Clear();
		mesh.vertices = vertices;
		mesh.triangles = triangles;
	}

	public void AddTriangle(Vector3 pos, Quaternion rotation)
	{
		vertices[index + 0] = (rotation * (new Vector3(0, -vertexOffset * triangleSize, 0)) + pos);
		vertices[index + 1] = (rotation * (new Vector3(-vertexOffset * triangleSize, vertexOffset * triangleSize, 0)) + pos);
		vertices[index + 2] = (rotation * (new Vector3(vertexOffset * triangleSize, vertexOffset * triangleSize, 0)) + pos);

		triangles[index + 0] = (index + 0);
		triangles[index + 1] = (index + 1);
		triangles[index + 2] = (index + 2);

		index += 3;
		UpdateMesh();
	}
}