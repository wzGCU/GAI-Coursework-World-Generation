using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PerlinNoise
{
    /*  Method returning a 2-dimensional array of values for perlin noise
     *  worldWidth - X-axis size value, always greater than 0
     *  worldDepth - Z-axis size value
     *  seed - to get the same map
     *  noiseDivision - scale of the noise
     *  intervals - amount of octaves for the noise, shouldnt be negative
     *  continuity - in range 0 to 1
     *  gapfill - lacunarity, how accurate the generation is, should be 1 or higher
     */


    public static float[,] GeneratePerlinMap(int worldWidth, int worldDepth, int seed, float noiseDivision, int intervals, float continuity, float gapfill, Vector2 offset)
    {
        float[,] perlinMap = new float[worldWidth, worldDepth];

        System.Random seederRandomise = new System.Random(seed);
        Vector2[] octaveOffsets = new Vector2[intervals];
        for (int i=0; i < intervals; i++)
        {
            float offsetX = seederRandomise.Next(-100000, 100000) + offset.x; //maximum values for perlin generation (more values are just repeating)
            float offsetZ = seederRandomise.Next(-100000, 100000) + offset.y;
            octaveOffsets[i] = new Vector2(offsetX, offsetZ);
        }

        if (noiseDivision <= 0) { noiseDivision = 0.0001f; } //made just so there is no division by 0.

        float maxNoiseHeight = float.MinValue;
        float minNoiseHeight = float.MaxValue;

        //to get the center of the perlin noise map
        float halfWidth = worldWidth / 2f; 
        float halfDepth = worldDepth / 2f;

        for(int z=0; z<worldDepth; z++) //looping for whole world size in X and Z axis
        {
            for (int x=0; x<worldWidth; x++)
            {
                
                float amplitude = 1;
                float frequency = 1;
                float noiseHeight = 0;

                //loop for octaves
                for (int i = 0; i < intervals; i++)
                {
                    float sampleX = (x - halfWidth) / noiseDivision * frequency + octaveOffsets[i].x;
                    float sampleZ = (z - halfDepth) / noiseDivision * frequency + octaveOffsets[i].y;

                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleZ) *2 -1;
                    noiseHeight += perlinValue + amplitude;
                    amplitude *= continuity; //it decreases each octave
                    frequency *= gapfill; //frequency increases each octave
                    
                }

                if (noiseHeight > maxNoiseHeight)
                {
                    maxNoiseHeight = noiseHeight;
                }
                else if (noiseHeight< minNoiseHeight)
                {
                    minNoiseHeight = noiseHeight;
                }
                perlinMap[x, z] = noiseHeight;
            }
        }
        for (int z = 0; z < worldDepth; z++) //looping for whole world size in X and Z axis
        {
            for (int x = 0; x < worldWidth; x++)
            {
                perlinMap[x, z] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, perlinMap[x, z]); //to return value between 0 and 1.
            }
        }
        return perlinMap;
    }
}
