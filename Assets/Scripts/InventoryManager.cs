using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour {

    
    
    public Dictionary<BlockType, int> Inventory;

    [Range (2,10)]
    public float MaxPlacementRange = 5.0F;
    [Range(1,2)]
    public float MinPlacementRange = 1.0F;

    public GameObject StartingBlock;
    public Texture2D DefaultTexture;
    
    private BlockType m_HeldBlockType;
    private int m_iBlock = 0;
    private List<BlockType> m_BlockTypeList;
    private string m_DefaultText;

    private MinecraftLevelGenerator levelGenerator;
    private RawImage blockImage;
    private Text blockCount;
    private AudioManager audioManager;

    // Use this for initialization
    void Start () {
        levelGenerator = GameObject.FindGameObjectWithTag("LevelGenerator").GetComponent<MinecraftLevelGenerator>();
        m_BlockTypeList = Enum.GetValues(typeof(BlockType)).Cast<BlockType>().ToList<BlockType>();

        InitializeInventory();

        m_HeldBlockType = StartingBlock.GetComponent<BlockProperties>().TypeOfBlock;
        
        var canvas = GameObject.FindGameObjectWithTag("Canvas") as GameObject;
        blockImage = canvas.transform.FindChild("SelectedBlock").gameObject.GetComponent<RawImage>() ;
        blockCount = blockImage.gameObject.transform.FindChild("BlockCount").gameObject.GetComponent<Text>();

        m_DefaultText = blockCount.text;

        UpdateHeldBlock();

        audioManager = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<AudioManager>();

    }

    private void InitializeInventory()
    {
        Inventory = new Dictionary<BlockType, int>();
        foreach (var blockType in m_BlockTypeList)
        {
            Inventory[blockType] = 0;
        }
    }

    // Update is called once per frame
    void Update ()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll > 0)
        {
            m_iBlock++;
            m_HeldBlockType = m_BlockTypeList[m_iBlock % m_BlockTypeList.Count];
            UpdateHeldBlock();

        }
        else if (scroll < 0)
        {
            m_iBlock--;
            while (m_iBlock <= -1) m_iBlock += m_BlockTypeList.Count;
            m_HeldBlockType = m_BlockTypeList[m_iBlock % m_BlockTypeList.Count];
            UpdateHeldBlock();
        }

        if (Input.GetButton("Fire2"))
        {
            TryPlaceObject();
        }
    }
    
    public void AddToInventory(PickupItem item)
    {
        Inventory[item.TypeOfBlock]++;
        Destroy(item.gameObject);
        UpdateHeldBlock();
    }

    private void TryPlaceObject()
    {
        if (Inventory[m_HeldBlockType] <= 0 || m_HeldBlockType == BlockType.Null) return;

        RaycastHit hitOutput;

        if (Physics.Raycast(new Ray(transform.position, transform.forward), out hitOutput, MaxPlacementRange)){

            Vector3 placeLocation = hitOutput.normal.normalized + hitOutput.collider.gameObject.transform.position;

            if (Vector3.Distance(placeLocation, transform.position) < MinPlacementRange) return;

            audioManager.PlaySoundAtLocation(m_HeldBlockType.ToString(), transform.position);
            levelGenerator.PlaceBlockByType(m_HeldBlockType, placeLocation);
            Inventory[m_HeldBlockType]--;

            UpdateHeldBlock();
        }
    }
    private void UpdateHeldBlock()
    {
        if (m_HeldBlockType == BlockType.Null )
        {
            blockImage.texture = DefaultTexture;
            blockCount.text = m_DefaultText;
        }
        else
        {
            var blockMaterial = levelGenerator.GetObjectOfType(m_HeldBlockType).GetComponent<MeshRenderer>().sharedMaterial;
            blockImage.texture = blockMaterial.mainTexture;

            blockCount.text = Inventory[m_HeldBlockType].ToString();
        }
    }

}
