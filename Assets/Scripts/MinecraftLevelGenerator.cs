using UnityEngine;
using System.Collections;

public class MinecraftLevelGenerator : MonoBehaviour {

    public int WaterLevel = 0;
    public int SnowLevel = 5;
    public int DirtThickness = 5;

    [Range(1,20)]
    public int HeightScale = 10;

    public int MinDepth = -10;
    public int LevelHeight = 50;
    public int LevelWidth = 50;

    [Range(1, 8)]
    public int OctaveCount = 5;

    public GameObject WaterCube;
    public GameObject DirtCube;
    public GameObject SnowCube;
    public GameObject GrassCube;
    public GameObject StoneCube;

    void Start () {
        GenerateLevel();
	}
	
    private void GenerateLevel()
    {
        float[][] perlinNoise = PerlinNoise.GeneratePerlinNoise(LevelHeight, LevelWidth, OctaveCount);
        for (int z = 0; z < LevelHeight; z++)
        {
            for (int x = 0; x < LevelWidth; x++)
            {
                float y = (perlinNoise[z][x] * HeightScale);
                Vector3 blockPosition = new Vector3(x, Mathf.RoundToInt(y), z);
                CreateBlock(blockPosition);

            }
        }
    }
    private void CreateBlock(Vector3 surfacePosition)
    {
        GameObject surfaceBlock = null;

        if (surfacePosition.y >= SnowLevel)
        {
            GameObject.Instantiate(SnowCube, surfacePosition, Quaternion.identity);
        } else if (surfacePosition.y < WaterLevel)
        {
            surfaceBlock = DirtCube;
            //Fill Water
            for(int y = (int)surfacePosition.y + 1; y <= WaterLevel; y++)
            {
                Vector3 waterPosition = new Vector3(surfacePosition.x, y, surfacePosition.z);
                GameObject.Instantiate(WaterCube, waterPosition, Quaternion.identity);
            }
        } else {
            surfaceBlock = GrassCube;
        }
        GameObject.Instantiate(surfaceBlock, surfacePosition, Quaternion.identity);
       
        //Fill ground
        for(int depthY = MinDepth; depthY < surfacePosition.y; depthY++)
        {
            GameObject undergroundBlock = null;
            if (surfacePosition.y - depthY  <= DirtThickness)
            {
                undergroundBlock = DirtCube;
            }
            else
            {
                undergroundBlock = StoneCube;
            }
            Vector3 undergroundPosition = new Vector3(surfacePosition.x, depthY, surfacePosition.z);
            GameObject.Instantiate(undergroundBlock, undergroundPosition, Quaternion.identity);
        }
    }



}
