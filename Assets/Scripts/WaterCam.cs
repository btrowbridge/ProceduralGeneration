using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class WaterCam : MonoBehaviour {

    public bool IsUnderwater = false;

    private int waterBlockCount = 0;
    private Image WaterFilter;
    private AudioManager audioManager;

    void Awake()
    {
        WaterFilter = GameObject.FindGameObjectWithTag("Canvas").GetComponent<Image>();
        audioManager = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<AudioManager>();
    }
    
    void OnTriggerEnter(Collider other)
    {
        
        if (other.tag == "Water")
        {
            
            
            
            waterBlockCount++;
            IsUnderwater = waterBlockCount > 0;
            WaterFilter.enabled = IsUnderwater;
            if (waterBlockCount == 1) audioManager.PlaySoundAtLocation("Water", transform.position);
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Water")
        {
            waterBlockCount--;
            IsUnderwater = waterBlockCount > 0;
            WaterFilter.enabled = IsUnderwater;
        }
    }
}
