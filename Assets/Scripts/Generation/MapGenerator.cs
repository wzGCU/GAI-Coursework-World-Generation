using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public enum DrawMode{ NoiseMap, ColourMap, Mesh };
    public DrawMode drawMode;
    public int mapWidth;
    public int mapHeight;
    public float noiseScale;

    public int octaves; //number of details
    [Range(0,1)]
    public float persistance;
    public float lacunarity; //smoothing

    public int seed;
    public Vector2 offset;

    public BiomeType[] biomes;
    public void GenerateMap()
    {
        float[,] noiseMap = PerlinNoise.GeneratePerlinMap(mapWidth, mapHeight, seed, noiseScale, octaves, persistance, lacunarity, offset);

        Color[] colourMap = new Color[mapWidth * mapHeight];
        for(int y=0; y < mapHeight; y++)
        {
            for (int x = 0; x<mapWidth; x++)
            {
                float currentHeight = noiseMap[x, y];
                //looping thru regions to know where is this one
                for (int i=0; i < biomes.Length; i++){
                    if(currentHeight <= biomes[i].height)
                    {
                        colourMap[y * mapWidth + x] = biomes[i].colour;
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
            display.DrawTexture(TextureGenerator.ColourMapTexture(colourMap, mapWidth, mapHeight));
        }
        else if(drawMode == DrawMode.Mesh)
        {
            display.DrawMesh(MeshGenerator.GenerateMeshForTerrain(noiseMap), TextureGenerator.ColourMapTexture(colourMap, mapWidth, mapHeight));
        }
    }

    //Method that fixes if the map height/width is 0 or less
    private void OnValidate()
    {
        if (mapWidth < 1)
        {
            mapWidth = 1;
        }
        if (mapHeight < 1)
        {
            mapHeight = 1;
        }
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
