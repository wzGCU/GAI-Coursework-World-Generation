using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfiniteTerrainGen : MonoBehaviour
{
    public const float maxViewDistance = 450;
    public Transform viewer;
    public Material mapMaterial;

    public static Vector2 viewerPosition;
    static MapGenerator mapGenerator;
    private int chunkSize;
    private int chunksInDistance;

    Dictionary<Vector2, TerrainChunk> terrainChunkDictionary = new Dictionary<Vector2, TerrainChunk>();
    List<TerrainChunk> terrainChunksVisibleLast = new List<TerrainChunk>();
    // Start is called before the first frame update
    void Start()
    {
        mapGenerator = FindObjectOfType<MapGenerator>();
        chunkSize = MapGenerator.mapChunkSize - 1;
        chunksInDistance = Mathf.RoundToInt(maxViewDistance / chunkSize);
    }

    private void Update()
    {
        viewerPosition = new Vector2(viewer.position.x, viewer.position.z);
        ReloadVisibleChunks();
    }

    void ReloadVisibleChunks()
    {
        for(int i=0; i<terrainChunksVisibleLast.Count; i++)
        {
            terrainChunksVisibleLast[i].SetVisible(false);
        }
        terrainChunksVisibleLast.Clear();
        int currentChunkCoordX = Mathf.RoundToInt(viewerPosition.x / chunkSize);
        int currentChunkCoordZ = Mathf.RoundToInt(viewerPosition.y / chunkSize);

        for (int zOffset =-chunksInDistance; zOffset <= chunksInDistance; zOffset++)
        {
            for (int xOffset = -chunksInDistance; xOffset <= chunksInDistance; xOffset++)
            {
                Vector2 viewedChunkCoord = new Vector2(currentChunkCoordX + xOffset, currentChunkCoordZ + zOffset);

                if (terrainChunkDictionary.ContainsKey(viewedChunkCoord))
                {
                    terrainChunkDictionary[viewedChunkCoord].UpdateChunk();
                    if (terrainChunkDictionary[viewedChunkCoord].IsVisible())
                    {
                        terrainChunksVisibleLast.Add(terrainChunkDictionary[viewedChunkCoord]);
                    }
                }
                else
                {
                    terrainChunkDictionary.Add(viewedChunkCoord, new TerrainChunk(viewedChunkCoord, chunkSize,transform, mapMaterial));
                }
            }
        }
    }

    public class TerrainChunk
    {
        GameObject meshObject;
        Vector2 position;
        Bounds bounds; //using to get the square distance method between given point and boundingBox;

        MapData mapData;
        MeshRenderer meshRenderer;
        MeshFilter meshFilter;

        public TerrainChunk(Vector2 coordinates, int size, Transform parent, Material material)
        {
            position = coordinates * size;
            bounds = new Bounds(position, Vector2.one * size);
            Vector3 position3D = new Vector3(position.x, 0, position.y);

            meshObject = new GameObject("Terrain Chunk");
            meshRenderer = meshObject.AddComponent<MeshRenderer>();
            meshFilter = meshObject.AddComponent<MeshFilter>();
            meshRenderer.material = material;

            meshObject = GameObject.CreatePrimitive(PrimitiveType.Plane);
            meshObject.transform.position = position3D;
            meshObject.transform.parent = parent;
            SetVisible(false); //first the chunk is not invisible

            mapGenerator.RequestMapData(OnMapDataReceived);
        }

        void OnMapDataReceived(MapData mapData)
        {
            mapGenerator.RequestMeshBlockData(mapData, OnMeshDataReceived);
        }

        void OnMeshDataReceived(MeshBlock meshData)
        {
            meshFilter.mesh = meshData.CreateGeneratedMesh();
        }

        //Update method checks whether the chunnk should be visible based on distance
        public void UpdateChunk()
        {
            float viewerDistanceFromNearestEdge = bounds.SqrDistance(viewerPosition);
            bool visible = viewerDistanceFromNearestEdge <= maxViewDistance*maxViewDistance; //Mathf.Sqrt() is quite an expensive operation if you're doing it alot, so you might want to keep a reference to maxViewDst^2 
            SetVisible(visible);
        }

        public void SetVisible(bool visible)
        {
            meshObject.SetActive(visible);
        }
        public bool IsVisible()
        {
            return meshObject.activeSelf;
        }
    }
}