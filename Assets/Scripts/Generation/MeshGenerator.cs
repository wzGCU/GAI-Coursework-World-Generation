using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MeshGenerator
{
    public static MeshBlock GenerateMeshForTerrain(float[,] heightMap)
    {
        int width = heightMap.GetLength(0);
        int depth = heightMap.GetLength(1);

        //to start from the top left corner
        float topLeftX = (width - 1) / -2f; 
        float topleftZ = (depth - 1) / -2f; 

        MeshBlock meshData = new MeshBlock(width, depth);
        int vertexIndex = 0;

        for(int z=0; z < depth; z++)
        {
            for(int x = 0; x < width; x++)
            {
                meshData.vertices[vertexIndex] = new Vector3(topLeftX + x, heightMap[x, z], topleftZ - z);
                meshData.uvs[vertexIndex] = new Vector2(x / (float)width, z / (float)depth);

                if(x<width -1 && z < depth - 1)
                {
                    meshData.AddTriangle(vertexIndex, vertexIndex + width + 1, vertexIndex + width);
                    meshData.AddTriangle(vertexIndex + width + 1, vertexIndex, vertexIndex + 1);

                }

                vertexIndex++;
            }
        }
        return meshData;
    }
}

public class MeshBlock
{
    public Vector3[] vertices;
    public int[] triangles;
    public Vector2[] uvs;

    int triangleIndex;

    public MeshBlock(int meshWidth, int meshDepth)
    {
        vertices = new Vector3[meshWidth * meshDepth];
        uvs = new Vector2[meshWidth * meshDepth];
        triangles = new int[(meshWidth - 1) * (meshDepth - 1) * 6];
    }

    public void AddTriangle(int x, int y, int z)
    {
        triangles[triangleIndex] = x;
        triangles[triangleIndex + 1] = y;
        triangles[triangleIndex + 2] = z;
        triangleIndex += 3;
    }

    public Mesh CreateGeneratedMesh()
    {
        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.RecalculateNormals();
        return mesh;
    }
}
