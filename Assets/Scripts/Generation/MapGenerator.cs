using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public enum DrawMode{ NoiseMap, ColourMap, Mesh };
    public DrawMode drawMode;

    public const int mapChunkSize = 241; //most optimized number for the amount of vertices generated per mesh (so it is divisible by all even number from 2 to 12)
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

    Queue<MapThreadInfo<MapData>> mapDataThreadInfoQueue = new Queue<MapThreadInfo<MapData>>();
    Queue<MapThreadInfo<MeshBlock>> meshBlockThreadInfoQueue = new Queue<MapThreadInfo<MeshBlock>>();

    public void DrawMapInEditor()
    {
        MapData mapData = GenerateMapData();
        MapDisplay display = FindObjectOfType<MapDisplay>();
        if (drawMode == DrawMode.NoiseMap)
        {
            display.DrawTexture(TextureGenerator.HeightMapTexture(mapData.heightMap));
        }
        else if (drawMode == DrawMode.ColourMap)
        {
            display.DrawTexture(TextureGenerator.ColourMapTexture(mapData.colourMap, mapChunkSize, mapChunkSize));
        }
        else if (drawMode == DrawMode.Mesh)
        {
            display.DrawMesh(MeshGenerator.GenerateMeshForTerrain(mapData.heightMap, meshHeightMultiplier, meshHeightCurve, levelOfDetail), TextureGenerator.ColourMapTexture(mapData.colourMap, mapChunkSize, mapChunkSize));
        }
    }

    public void RequestMapData(Action<MapData> callback)
    {
        ThreadStart threadStart = delegate
        {
            MapDataThread(callback);
        };
        new Thread(threadStart).Start();
    }

    void MapDataThread(Action<MapData> callback)
    {
        MapData mapData = GenerateMapData();
        lock (mapDataThreadInfoQueue) //when one thread reaches this point while its executing, no other thread can execute
        {
            mapDataThreadInfoQueue.Enqueue(new MapThreadInfo<MapData>(callback, mapData));
        }
        
    }

    public void RequestMeshBlockData(MapData mapData, Action<MeshBlock> callback)
    {
        ThreadStart threadStart = delegate
        {
            MeshBlockDataThread(mapData, callback);
        };

        new Thread(threadStart).Start();
    }

    void MeshBlockDataThread(MapData mapData, Action<MeshBlock> callback)
    {
        AnimationCurve curve = new AnimationCurve(meshHeightCurve.keys);
        MeshBlock meshData = MeshGenerator.GenerateMeshForTerrain(mapData.heightMap, meshHeightMultiplier, curve, levelOfDetail);
        lock (meshBlockThreadInfoQueue)
        {
            meshBlockThreadInfoQueue.Enqueue(new MapThreadInfo<MeshBlock>(callback, meshData));
        }
    }

    private void Update()
    {
        if (mapDataThreadInfoQueue.Count > 0)
        {
            for (int i = 0; i < mapDataThreadInfoQueue.Count; i++)
            {
                MapThreadInfo<MapData> threadInfo = mapDataThreadInfoQueue.Dequeue();
                threadInfo.callback(threadInfo.parameter);
            }
        }

        if(meshBlockThreadInfoQueue.Count > 0)
        {
            for (int i = 0; i < meshBlockThreadInfoQueue.Count; i++)
            {
                MapThreadInfo<MeshBlock> threadInfo = meshBlockThreadInfoQueue.Dequeue();
                threadInfo.callback(threadInfo.parameter);
            }
        }
    }
    MapData GenerateMapData()
    {
        float[,] noiseMap = PerlinNoise.GeneratePerlinMap(mapChunkSize, mapChunkSize, seed, noiseScale, octaves, persistance, lacunarity, offset);

        Color[] colourMap = new Color[mapChunkSize * mapChunkSize];
        for(int y=0; y < mapChunkSize; y++)
        {
            for (int x = 0; x<mapChunkSize; x++)
            {
                float currentHeight = noiseMap[x, y];
                //looping thru regions to know where is this one
                for (int i=0; i < biomes.Length; i++)
                {
                    if(currentHeight <= biomes[i].height)
                    {
                        colourMap[y * mapChunkSize + x] = biomes[i].colour;
                        break;
                    }
                }
            }
        }
        return new MapData(noiseMap, colourMap);
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

    struct MapThreadInfo<T> //making it generic so it can handle both mapData and meshData
    {
        public readonly Action<T> callback;
        public readonly T parameter;

        public MapThreadInfo(Action<T> callback, T parameter)
        {
            this.callback = callback;
            this.parameter = parameter;
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

public struct MapData
{
    public readonly float[,] heightMap;
    public readonly Color[] colourMap;

    public MapData(float[,] heightMap, Color[] colourMap)
    {
        this.heightMap = heightMap;
        this.colourMap = colourMap;
    }
}