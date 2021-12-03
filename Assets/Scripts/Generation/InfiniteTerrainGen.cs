using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfiniteTerrainGen : MonoBehaviour
{
    public GameObject player;
    public GameObject defaultTerrain;

    //Variable to change how big should the terrain be generated around the player
    [SerializeField]
    private int radiusOfGeneration = 5;
    //Variable to manage the offset between each terrain object based on the size of the terrain template
    [SerializeField]
    private int terrainSizeOffset = 10;

    
    private Vector3 startPosition = Vector3.zero;

    //referencing the player movement on X and Z axis via lambda expression
    private int playerMoveX => (int)(player.transform.position.x - startPosition.x);
    private int playerMoveZ => (int)(player.transform.position.z - startPosition.z);
    //Getting X and Z axis player location by the offset of the size of terrain multiplied by the floor (rounded down) value of the player position responding to offset
    private int playerLocationX => (int)Mathf.Floor(player.transform.position.x / terrainSizeOffset) * terrainSizeOffset;
    private int playerLocationZ => (int)Mathf.Floor(player.transform.position.z / terrainSizeOffset) * terrainSizeOffset;

    //Hash table to be used whether certain tile is already generated or not. It is fair to use hashtable here to have the position of tile as key and the given terrain as value.
    private Hashtable tileTerrain = new Hashtable();

    // Start is called before the first frame update
    void Start()
    {
        GenerateTerrains();
    }

    // Update is called once per frame
    void Update()
    {

        if (HasPlayerMoved())
        {
            GenerateTerrains();
        }
    }

    void GenerateTerrains()
    {
        for (int x = -radiusOfGeneration; x < radiusOfGeneration; x++)
        {
            for (int z = -radiusOfGeneration; z < radiusOfGeneration; z++)
            {
                Vector3 position = new Vector3((x * terrainSizeOffset + playerLocationX), 0, (z * terrainSizeOffset + playerLocationZ));
                //Check if at the position there is a tile and if not generate one and add it to the hashtable
                if (!tileTerrain.Contains(position))
                {
                    GameObject _terrain = Instantiate(defaultTerrain, position, Quaternion.identity);
                    _terrain.transform.SetParent(this.transform);
                    tileTerrain.Add(position, _terrain);
                }
            }
        }
        CheckIfTooFar();
    }

    //function that checks if player has moved which returns a boolean
    bool HasPlayerMoved()
    {
        //Check if player has moved either in X or Z axis based on the offset
        if(Mathf.Abs(playerMoveX) >= terrainSizeOffset || Mathf.Abs(playerMoveZ) >= terrainSizeOffset)
        {
            return true;
        }
        else return false;
    }
    //function to check if player is far enough to delete the not visible terrains
    void CheckIfTooFar()
    {
        Hashtable newTerrains = new Hashtable();
        foreach (Vector3 position in tileTerrain.Keys)
        {
            GameObject terrainBlock = (GameObject)tileTerrain[position];
            int terrainPositionX = (int)(terrainBlock.transform.position.x);
            int terrainPositionZ = (int)(terrainBlock.transform.position.z);
            if (((position.x > playerLocationX + terrainSizeOffset*radiusOfGeneration) || (position.x < playerLocationX - terrainSizeOffset*radiusOfGeneration)) || 
                ((position.z > playerLocationZ + terrainSizeOffset * radiusOfGeneration) || (position.z < playerLocationZ - terrainSizeOffset * radiusOfGeneration)))
            {
                Destroy(terrainBlock);
            }
            else
            {
                newTerrains.Add(position, terrainBlock);
            }
        }
        tileTerrain = newTerrains;
    }
}