using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
// TODO: COMMENT THIS CODE
public class ChunkManager : MonoBehaviour
{
	[SerializeField]
	private Vector2Int gridSize = new(5, 5);
	[SerializeField]
	private int chunkSize = 20;
	[SerializeField]
	private GameObject chunkPrefab;

	private TriangleMeshRenderer[,] chunkList;

	private void Start()
	{
		chunkList = new TriangleMeshRenderer[gridSize.x * 2 + 1, gridSize.y * 2 + 1];
		for (int i = -gridSize.x; i <= gridSize.x; i++)
		{
			for (int j = -gridSize.y; j <= gridSize.y; j++)
			{
				chunkList[i + gridSize.x, j + gridSize.y] = Instantiate(chunkPrefab, new Vector3(i * chunkSize, 0, j * chunkSize), Quaternion.identity, transform).GetComponent<TriangleMeshRenderer>();
			}
		}
	}

	public void AddTriangle(Vector3 pos, Quaternion rotation, Color32 color, float size = 1)
	{
		Vector2Int chunkPos = getMeshAtPos(pos);
		chunkList[chunkPos.x, chunkPos.y].AddTriangle(pos - new Vector3((chunkPos.x - gridSize.x) * chunkSize, 0, (chunkPos.y - gridSize.y) * chunkSize), rotation, color, size);
	}

	public Vector2Int getMeshAtPos(Vector3 pos)
	{
		pos /= chunkSize;
		int x = (int)Math.Round(pos.x) + gridSize.x;
		int z = (int)Math.Round(pos.z) + gridSize.y;
		return new Vector2Int(x, z);
	}

#if UNITY_EDITOR
	void OnDrawGizmos()
	{
		if (Selection.activeGameObject != transform.gameObject) { return; }
		Gizmos.color = new Color(1, 0, 0, 0.5f);
		for (int i = -gridSize.x; i <= gridSize.x; i++)
		{
			for (int j = -gridSize.y; j <= gridSize.y; j++)
			{
				Gizmos.DrawWireCube(transform.position + new Vector3(i * chunkSize, 0, j * chunkSize), new Vector3(chunkSize, 0, chunkSize));
			}
		}
	}
#endif
}
