using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public enum DrawMode{ NoiseMap, ColourMap, Mesh };
    public DrawMode drawMode;

    const int mapChunkSize = 241; //most optimized number for the amount of vertices generated per mesh (so it is divisible by all even number from 2 to 12)
    [Range(0,6)]
    public int levelOfDetail; //6 levels available cause 240 is divisible by 
    public float noiseScale;

    public int octaves; //number of details
    [Range(0,1)]
    public float persistance;
    public float lacunarity; //smoothing

    public int seed;
    public Vector2 offset;
    public float meshHeightMultiplier;
    public AnimationCurve meshHeightCurve; //for water flattering

    public BiomeType[] biomes;
    public void GenerateMap()
    {
        float[,] noiseMap = PerlinNoise.GeneratePerlinMap(mapChunkSize, mapChunkSize, seed, noiseScale, octaves, persistance, lacunarity, offset);

        Color[] colourMap = new Color[mapChunkSize * mapChunkSize];
        for(int y=0; y < mapChunkSize; y++)
        {
            for (int x = 0; x<mapChunkSize; x++)
            {
                float currentHeight = noiseMap[x, y];
                //looping thru regions to know where is this one
                for (int i=0; i < biomes.Length; i++){
                    if(currentHeight <= biomes[i].height)
                    {
                        colourMap[y * mapChunkSize + x] = biomes[i].colour;
                        break;
                    }
        }
            }
        }
        MapDisplay display = FindObjectOfType<MapDisplay>();
        if(drawMode== DrawMode.NoiseMap)
        {
            display.DrawTexture(TextureGenerator.HeightMapTexture(noiseMap));
        }
        else if(drawMode == DrawMode.ColourMap)
        {
            display.DrawTexture(TextureGenerator.ColourMapTexture(colourMap, mapChunkSize, mapChunkSize));
        }
        else if(drawMode == DrawMode.Mesh)
        {
            display.DrawMesh(MeshGenerator.GenerateMeshForTerrain(noiseMap, meshHeightMultiplier, meshHeightCurve, levelOfDetail), TextureGenerator.ColourMapTexture(colourMap, mapChunkSize, mapChunkSize));
        }
    }

    //Method that fixes if the map height/width is 0 or less
    private void OnValidate()
    {
        
        if (lacunarity < 1)
        {
            lacunarity = 1;
        }
        if (octaves < 0)
        {
            octaves = 0;
        }
    }
}

[System.Serializable]
public struct BiomeType
{
    public string name;
    public float height;
    public Color colour;
}
