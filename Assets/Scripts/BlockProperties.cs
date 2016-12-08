using UnityEngine;
using System.Collections;
using System;

public enum BlockType
{
    Null,
    Water,
    Snow,
    Dirt,
    Stone,
    Grass,
}

public class BlockProperties : MonoBehaviour {


    public float Health = 1.0F;
    public bool IsIndestructable = false;
    public BlockType TypeOfBlock;
    public GameObject PickupToken;

    private bool m_IsQuitting = false;
    
    private MinecraftLevelGenerator levelGenerator;
    private MeshRenderer meshRenderer;
    private AudioManager audioManager;
    
    void Start ()
    {
        levelGenerator = GameObject.FindGameObjectWithTag("LevelGenerator").GetComponent<MinecraftLevelGenerator>();
        if (meshRenderer == null) meshRenderer = GetComponentInChildren<MeshRenderer>();
        audioManager = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<AudioManager>();

    }
    void Update()
    {
    }

    public void DigHit(float dmg)
    {
        if (!IsIndestructable) {
            audioManager.PlaySoundAtLocation(TypeOfBlock.ToString(), transform.position);

            Health -= dmg;
            if (Health <= 0)
            {

                KillCube();
            }
        }
    }

    public void SetOcclusion(bool isOccluded)
    {
        if (meshRenderer == null) meshRenderer = GetComponentInChildren<MeshRenderer>();
        meshRenderer.enabled = !isOccluded;
    }
    private void KillCube()
    {
        levelGenerator.UnoccludeNeighbors(gameObject);
        levelGenerator.RemoveBlock(gameObject);
        Destroy(gameObject);
    }

    void OnDestroy()
    {
        if (!m_IsQuitting)
        {
            GameObject.Instantiate(PickupToken, transform.position, Quaternion.identity);
        }
    }
    void OnApplicationQuit()
    {
        m_IsQuitting = true;
    }
}
