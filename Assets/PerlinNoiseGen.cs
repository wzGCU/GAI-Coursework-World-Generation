using System.Collections.Generic;
using UnityEngine;

public class PerlinNoiseGen : MonoBehaviour
{
    public float scale = 20;
    public int size = 256;
    int width => size;
    int height => size;

    private void Update()
    {
        Renderer renderer = GetComponent<Renderer>();
        renderer.material.mainTexture = GenerateTexture();
    }

    Texture2D GenerateTexture()
    {
        Texture2D texture = new Texture2D(width, height);

        for(int x = 0; x < size; x++)
        {
            for (int z = 0; z<size; z++)
            {
                Color colour = CalculateColour(x,z);
                texture.SetPixel(x, z, colour);
            }
        }

        texture.Apply();
        return texture;
    }

    Color CalculateColour(int x, int z)
    {
        float xCoord = (float)x / size * scale;
        float zCoord = (float)z / size * scale;

        float sample = Mathf.PerlinNoise(x, z);
        return new Color(sample, sample, sample);
    }
}
