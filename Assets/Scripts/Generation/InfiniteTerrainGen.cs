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
                    tileTerrain.Add(position, _terrain);
                }
            }
        }
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
    void CheckIfTooFar()
    {

    }
}
