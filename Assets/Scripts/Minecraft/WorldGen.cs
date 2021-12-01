using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldGen : MonoBehaviour
{
    public int sizeX;
    public int sizeZ;

    public int groundHeight;
    public float terDetail;
    public float terHeight;
    int seed;

    public GameObject[] blocks;
    // Start is called before the first frame update
    void Start()
    {
        seed = Random.Range(100000, 9999999);
        GenerateTerrain();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void GenerateTerrain()
    {
        for(int x=0; x<sizeX; x++)
        {
            for (int z = 0; z < sizeZ; z++)
            {
                int maxy = (int)(Mathf.PerlinNoise((x / 2 + seed) / terDetail, (z / 2 + seed) / terDetail) * terHeight);
                maxy += groundHeight;

                GameObject grass = Instantiate(blocks[0], new Vector3(x, maxy, z), Quaternion.identity)as GameObject;
                grass.transform.SetParent(GameObject.FindGameObjectWithTag("Envi").transform);
                
                for (int y=0; y<maxy; y++)
                {
                    int dirtLayers = Random.Range(1, 5);
                    if (y >= maxy - dirtLayers)
                    {
                        GameObject dirt = Instantiate(blocks[1], new Vector3(x, y, z), Quaternion.identity) as GameObject;
                        dirt.transform.SetParent(GameObject.FindGameObjectWithTag("Envi").transform);

                    }
                    else
                    {
                        GameObject stone = Instantiate(blocks[2], new Vector3(x, y, z), Quaternion.identity) as GameObject;
                        stone.transform.SetParent(GameObject.FindGameObjectWithTag("Envi").transform);
                    }
                }
            }
        }
    }
}
