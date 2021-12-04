﻿using System.Collections.Generic;
using UnityEngine;
public class PCGTerrain : MonoBehaviour
{
	[SerializeField]
	private float width = 10.0f;

	[SerializeField]
	private float depth = 10.0f;

	[SerializeField]
	private float maxHeight = 10.0f;

	[SerializeField]
	private int cellsX1 = 100;

	[SerializeField]
	private int cellsZ1 = 100;

	[SerializeField]
	private float perlinStepSizeX = 0.1f;

	[SerializeField]
	private float perlinStepSizeZ = 0.1f;


	private Vector3[] vertices;
	private Color[] colours;
	private Vector2[] uvs;
	private Vector3[] normals;
	private int[] triangles;

	private Mesh mesh;
	float certainMaxHeight;

	private bool updateTerrain = true;
	float a;

	/// <summary>
	/// This function gets called when changes are made to the properties of this class in the inspector
	/// </summary>
	private void OnValidate()
	{
		updateTerrain = true;
	}

	private void Update()
	{
		if (updateTerrain)
		{
			updateTerrain = false;
			CreateTerrain();
		}
		if (Input.GetKeyDown("space"))
		{

			CreateTerrain();
		}
	}

	/// <summary>
	/// Responsible for creating the mesh
	/// </summary>
	private void CreateTerrain()
	{
		if (mesh == null)
		{
			mesh = new Mesh();
			MeshFilter meshFilter = gameObject.GetComponent<MeshFilter>();
			meshFilter.mesh = mesh;
		}

		int verticesRowCount = cellsX1 + 1;
		int verticesCount = verticesRowCount * (cellsZ1 + 1);
		int trianglesCount = 6 * cellsX1 * cellsZ1;

		vertices = new Vector3[verticesCount];
		uvs = new Vector2[verticesCount];
		colours = new Color[verticesCount];
		normals = new Vector3[verticesCount];
		triangles = new int[trianglesCount];

		// Set the vertices of the mesh
		int vertexIndex = 0;
		for (int z = 0; z <= cellsZ1; ++z)
		{
			float percentageZ = (float)z / (float)cellsZ1;
			float startZ = percentageZ * depth;

			for (int x = 0; x <= cellsX1; ++x)
			{
				float percentageX = (float)x / (float)cellsX1;
				float startX = percentageX * width;


				if (a == 6)
				{
					Debug.Log("Mountain");
					certainMaxHeight =  maxHeight;
				}
				if (a == 5)
				{
					Debug.Log("High");
					certainMaxHeight = 0.95f * maxHeight;
				}
				if (a == 4)
				{
					Debug.Log("Land");
					certainMaxHeight = 0.85f * maxHeight;
				}
				if (a == 3)
				{
					Debug.Log("Swamp");
					certainMaxHeight = 0.65f * maxHeight;
				}
				if (a == 2)
				{
					Debug.Log("Shell");
					certainMaxHeight = 0.45f * maxHeight;
				}
				if (a == 1)
				{
					Debug.Log("Lake");
					certainMaxHeight = 0.25f * maxHeight;
				}
				if (a == 0)
				{
					Debug.Log("Ocean");
					certainMaxHeight = 0.15f * maxHeight;
				}

				// CHANGE ME! This height variable controls the height of each vertex in the generated grid.
				// If you want to see different heights per vertex you will need to change this in each iteration of these loops
				//Default:
				//float height = maxHeight;
				//Random:
				//float height = Random.RandomRange(0,maxHeight);
				//Perlin:
				float height = Mathf.PerlinNoise(x * perlinStepSizeX, z * perlinStepSizeZ) * certainMaxHeight;

				Color colour = Color.green;
				if (height > 0.5 * maxHeight)
				{
					colour = Color.Lerp(Color.green, Color.yellow, (float)height / maxHeight);
				}
				else
				{
					colour = Color.Lerp(Color.blue, Color.green, (float)height * 2 / maxHeight);
				}

				vertices[vertexIndex] = new Vector3(startX, height, startZ);
				colours[vertexIndex] = colour;
				uvs[vertexIndex] = new Vector2();       // No texturing so just set to zero
				normals[vertexIndex] = Vector3.up;      // These should be set based on heights of terrain but we can use Recalulated normals on mesh to calculate for us
				++vertexIndex;
			}
		}

		// Setup the indexes so they are in the correct order and will render correctly
		vertexIndex = 0;
		int trianglesIndex = 0;
		for (int z = 0; z < cellsZ1; ++z)
		{
			for (int x = 0; x < cellsX1; ++x)
			{
				vertexIndex = x + (verticesRowCount * z);

				triangles[trianglesIndex++] = vertexIndex;
				triangles[trianglesIndex++] = vertexIndex + verticesRowCount;
				triangles[trianglesIndex++] = (vertexIndex + 1) + verticesRowCount;
				triangles[trianglesIndex++] = (vertexIndex + 1) + verticesRowCount;
				triangles[trianglesIndex++] = vertexIndex + 1;
				triangles[trianglesIndex++] = vertexIndex;
			}
		}

		// Assign all of the data that has been created to the mesh and update it
		mesh.vertices = vertices;
		mesh.uv = uvs;
		mesh.triangles = triangles;
		mesh.colors = colours;
		mesh.normals = normals;
		mesh.RecalculateNormals();
		mesh.UploadMeshData(false);
	}
}