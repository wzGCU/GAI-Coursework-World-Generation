using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MeshGenerator
{
    public static MeshBlock GenerateMeshForTerrain(float[,] heightMap, float heightMultiplier, AnimationCurve _heightCurve, int levelOfDetail)
    {
        AnimationCurve heightCurve = new AnimationCurve(_heightCurve.keys);
        int width = heightMap.GetLength(0);
        int depth = heightMap.GetLength(1);

        //to start from the top left corner
        float topLeftX = (width - 1) / -2f; 
        float topleftZ = (depth - 1) / -2f;

        int meshSimplificationIncrement = (levelOfDetail ==0)?1:levelOfDetail * 2; //check if levelOfDetail is equal to 0 then set it to 1, otherwise multiply by 2
        int verticesPerLine = (width - 1) / meshSimplificationIncrement + 1;

        MeshBlock meshData = new MeshBlock(verticesPerLine, verticesPerLine);
        int vertexIndex = 0;

        for(int z=0; z < depth; z += meshSimplificationIncrement)
        {
            for(int x = 0; x < width; x += meshSimplificationIncrement)
            {
                meshData.vertices[vertexIndex] = new Vector3(topLeftX + x, heightCurve.Evaluate(heightMap[x, z]) * heightMultiplier, topleftZ - z);
                meshData.uvs[vertexIndex] = new Vector2(x / (float)width, z / (float)depth);

                if(x<width -1 && z < depth - 1)
                {
                    meshData.AddTriangle(vertexIndex, vertexIndex + verticesPerLine + 1, vertexIndex + verticesPerLine);
                    meshData.AddTriangle(vertexIndex + verticesPerLine + 1, vertexIndex, vertexIndex + 1);

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
