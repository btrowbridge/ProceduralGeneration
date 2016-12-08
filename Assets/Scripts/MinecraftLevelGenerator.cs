using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;


public class MinecraftLevelGenerator : MonoBehaviour {

    public int WaterLevel = 0;
    public int SnowLevel = 5;
    public int DirtThickness = 5;

    [Range(5,20)]
    public int HeightScale = 10;

    [Range(0,5)]
    public int MinDepth = 0;
    public int LevelHeight = 50;
    public int LevelWidth = 50;

    [Range(1, 8)]
    public int OctaveCount = 5;

    public float WaterFloodDelay = 1;

    public GameObject WaterCube;
    public GameObject DirtCube;
    public GameObject SnowCube;
    public GameObject GrassCube;
    public GameObject StoneCube;

    public GameObject PlayerCharacter;


    private Dictionary<BlockType, GameObject> m_BlockTypeObject;
    private Dictionary<Vector3, GameObject> m_BlockMap;
    void Start () {
        m_BlockMap = new Dictionary<Vector3, GameObject>();
        GenerateLevel();
        MassOcclusionTest();
        SpawnPlayer();
        m_BlockTypeObject = new Dictionary<BlockType, GameObject>
        {
            { BlockType.Null, null },
            { BlockType.Grass, GrassCube },
            { BlockType.Snow, SnowCube },
            { BlockType.Dirt, DirtCube },
            { BlockType.Stone, StoneCube },
            { BlockType.Water, WaterCube }
        };
    }

    public GameObject GetObjectOfType(BlockType blockType)
    {
        return m_BlockTypeObject[blockType];
    }

    #region Level Generation
    private void SpawnPlayer()
    {
        GameObject.Instantiate(PlayerCharacter, new Vector3(LevelHeight/2, HeightScale, LevelWidth/2), Quaternion.identity);
    }

    private void GenerateLevel()
    {
        float[][] perlinNoise = PerlinNoise.GeneratePerlinNoise(LevelHeight, LevelWidth , OctaveCount);
        for (int z = 0; z < LevelHeight; z++)
        {
            for (int x = 0; x < LevelWidth; x++)
            {
                float y = (perlinNoise[z][x] * HeightScale) + MinDepth;
                Vector3 columnPosition = new Vector3(x, Mathf.RoundToInt(y), z);
                SpawnColumn(columnPosition);

            }
        }
    }
    private void SpawnColumn(Vector3 surfacePosition)
    {
        GameObject surfaceBlock = null;

        if (surfacePosition.y >= SnowLevel && surfacePosition.y > WaterLevel)
        {
            surfaceBlock = SnowCube;
        } else if (surfacePosition.y < WaterLevel)
        {
            surfaceBlock = DirtCube;
            //Fill Water
            for(int y = (int)surfacePosition.y + 1; y <= WaterLevel; y++)
            {
                Vector3 waterPosition = new Vector3(surfacePosition.x, y, surfacePosition.z);
                SpawnBlock(WaterCube, waterPosition);
            }
        } else {
            surfaceBlock = GrassCube;
        }
        if (surfaceBlock == null)
        {
            return;
        }

        SpawnBlock(surfaceBlock, surfacePosition);

        //Fill ground
        for (int depthY = MinDepth; depthY < surfacePosition.y; depthY++)
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
            SpawnBlock(undergroundBlock, undergroundPosition);
        }
    }

    public GameObject SpawnBlock(GameObject block, Vector3 position)
    {
        var instance = GameObject.Instantiate(block, position, Quaternion.identity) as GameObject;
        m_BlockMap.Add(position, instance);
        instance.name += position.ToString();
        return instance;
    }

    public void PlaceBlockByType(BlockType blockType, Vector3 position)
    {
        GameObject block = m_BlockTypeObject[blockType];

        GameObject existingBlock;

        if(m_BlockMap.TryGetValue(position, out existingBlock))
        {
            if (existingBlock.layer == LayerMask.NameToLayer("Water"))
            {
                m_BlockMap.Remove(position);
                Destroy(existingBlock);
            }
        }

        var instance = SpawnBlock(block, position);
        TryOccludeNeighbors(instance);
    }

    #endregion

    #region Occlusion Stuff

    private static Vector3[] NeighborDirections = new Vector3[6]
    {
       new Vector3(0, 0, +1),
       new Vector3(0, 0, -1),
       new Vector3(0, +1, 0),
       new Vector3(0, -1, 0),
       new Vector3(+1, 0, 0),
       new Vector3(-1, 0, 0)
    };

    private bool IsSurrounded (GameObject blockInstance)
    {
        foreach (Vector3 dir in NeighborDirections)
        {
            Vector3 queryPosition = blockInstance.transform.position + dir;

            GameObject neighborBlock;
            if (!m_BlockMap.TryGetValue(queryPosition, out neighborBlock))
            {
                return false;
                
            }
            else if (neighborBlock.tag == "Water" && blockInstance.tag != "Water")
            {
                return false;
            }
        }
        return true;
    }

    private void MassOcclusionTest()
    {
        foreach(var blockPair in m_BlockMap)
        {
            TesOcclusion(blockPair.Value);

        }
    }

    private void TesOcclusion(GameObject block)
    {
        if (IsSurrounded(block))
        {
            block.GetComponent<BlockProperties>().SetOcclusion(true);
        }
    }

    public void UnoccludeNeighbors(GameObject blockCenter)
    {
        foreach(Vector3 dir in NeighborDirections)
        {
            Vector3 queryPosition = blockCenter.transform.position + dir;
            GameObject neighbor;
            if (m_BlockMap.TryGetValue(queryPosition,out neighbor))
            {
                
                neighbor.GetComponent<BlockProperties>().SetOcclusion(false);
            }

        }
    }
    private void TryOccludeNeighbors(GameObject blockCenter)
    {
        foreach (Vector3 dir in NeighborDirections)
        {
            Vector3 queryPosition = blockCenter.transform.position + dir;
            GameObject neighbor;
            if (m_BlockMap.TryGetValue(queryPosition, out neighbor))
            {
                TesOcclusion(neighbor);
            }

        }
    }
    #endregion

    public void RemoveBlock(GameObject objectToRemove)
    {
        m_BlockMap.Remove(objectToRemove.transform.position);

        StartCoroutine(TrySpawnWaterAfterDelay(objectToRemove.transform.position, WaterFloodDelay));
    }

    private IEnumerator TrySpawnWaterAfterDelay(Vector3 position, float time)
    {
        yield return new WaitForSeconds(time);

        GameObject block = null;

        if (!m_BlockMap.TryGetValue(position, out block) && IsNextToWater(position))
        {
            PlaceBlockByType(BlockType.Water, position);
        }
    }

    private bool IsNextToWater(Vector3 position)
    {
        foreach (Vector3 dir in NeighborDirections)
        {
            //DontCheckBelow
            if (dir == Vector3.down) continue;

            Vector3 queryPosition = position + dir;

            GameObject neighborBlock;
            if (m_BlockMap.TryGetValue(queryPosition, out neighborBlock))
            {
                if (neighborBlock.tag == "Water")
                {
                    return true;
                }
            }
        }
        return false;
    }
}
