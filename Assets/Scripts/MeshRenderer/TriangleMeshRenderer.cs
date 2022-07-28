using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using Random = UnityEngine.Random;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class TriangleMeshRenderer : MonoBehaviour
{
	private Mesh _mesh;
	private Vector3[] _vertices;
	private int[] _triangles;
	private Color32[] _colors;

	//private int scansCount = 0;
	private int _index;
	private static readonly float triangleSize = 0.02f;
	private static readonly int maxTriangles = 21845; // allows up to 21845 triangles
	private static readonly int maxIndex = 3 * maxTriangles;

	private bool containsGeometry = false;

	private void Start()
	{
		_vertices = new Vector3[maxTriangles * 3];
		_triangles = new int[maxTriangles * 3];
		_colors = new Color32[maxTriangles * 3];
		_index = 0;

		_mesh = GetComponent<MeshFilter>().mesh;
		UpdateMesh();
		containsGeometry = false;
		this.gameObject.SetActive(false);
	}

	private void UpdateMesh()
	{
		_mesh.Clear();
		_mesh.vertices = _vertices;
		_mesh.triangles = _triangles;
		_mesh.colors32 = _colors;
	}

	public void AddTriangle(Vector3 pos, Quaternion rotation, Color32 color, float size = 1)
	{
		//if (CheckOverlap(pos)) {return;}
		if (_index >= maxIndex)
		{ _index = 0;}

		if (!containsGeometry)
		{
			containsGeometry = true;
			this.gameObject.SetActive(true);
		}

		rotation.eulerAngles = new Vector3(rotation.eulerAngles.x, rotation.eulerAngles.y, Random.Range(-180f, 180f));
		size *= triangleSize;

		_vertices[_index + 0] = (rotation * (new Vector3(0, -size, 0)) + pos);
		_vertices[_index + 1] = (rotation * (new Vector3(-size, size, 0)) + pos);
		_vertices[_index + 2] = (rotation * (new Vector3(size, size, 0)) + pos);

		_triangles[_index + 0] = (_index + 0);
		_triangles[_index + 1] = (_index + 2);
		_triangles[_index + 2] = (_index + 1);

		_colors[_index + 0] = color;
		_colors[_index + 1] = color;
		_colors[_index + 2] = color;

		_index += 3;
		UpdateMesh();
	}

	/*private bool CheckOverlap(Vector3 pos)
	{
		for (int i = 0; i < index; i += 3)
		{
			Vector3 otherPos = vertices[i];
			if (MathF.Abs(otherPos.x - pos.x) < 0.1f && MathF.Abs(otherPos.y - pos.y) < 0.1f && MathF.Abs(otherPos.z - pos.z) < 0.1f)
			{
				return true;
			}
		}
		return false;
	}*/
}