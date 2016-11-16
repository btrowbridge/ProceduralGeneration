using UnityEngine;
using System.Collections;
using System;

public class LevelGenerator : MonoBehaviour {

    public int Height = 128;
    public int Width = 128;

    [Range(1,8)]
    public int OctaveCount = 5;

	void Start () {
        GenerateLevel();
	}

    private void GenerateLevel()
    {
        mTexture = new Texture2D(Height, Width, TextureFormat.RGB24, false);

        Renderer renderer = GetComponent<Renderer>();
        renderer.material.mainTexture = mTexture;

        float[][] perlinNoise = PerlinNoise.GeneratePerlinNoise(Height, Width, OctaveCount);
        for (int i = 0; i<Height; i++)
        {
            for (int j = 0; j<Width; j++)
            {
                float noise = perlinNoise[i][j];
                mTexture.SetPixel(i, j, new Color(noise, noise, noise));
            }
        }
        mTexture.Apply();
    }

    private Texture2D mTexture;
}
