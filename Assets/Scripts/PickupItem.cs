using UnityEngine;
using System.Collections;

public class PickupItem : MonoBehaviour {

    public float RotationRate = 90.0F;
    [Range(0,0.5F)]
    public float BobLength = 0.25F;
    public float BobRate = 1.0F;
    public float LifeSpan = 30.0F;
    public BlockType TypeOfBlock; 

    private Vector3 Origin;
    private AudioManager audioManager;
	// Use this for initialization
	void Awake () {
        Origin = transform.position;
        audioManager = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<AudioManager>();
        Destroy(gameObject, LifeSpan);
    }
	
	// Update is called once per frame
	void Update () {
        transform.Rotate(Vector3.up, RotationRate);
        transform.position = new Vector3(Origin.x, Origin.y + BobLength * Mathf.Sin(Time.time * BobRate) , Origin.z);
	}

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            audioManager.PlaySoundAtLocation("Pickup", transform.position);
            other.GetComponentInChildren<InventoryManager>().AddToInventory(this);
        }
    }

    void OnApplicationQuit()
    {
        DestroyImmediate(gameObject);
    }
}
