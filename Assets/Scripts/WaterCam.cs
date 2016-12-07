using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class WaterCam : MonoBehaviour {


    private int waterBlockCount = 0;
    private Image WaterFilter;
    void Awake()
    {
        WaterFilter = GameObject.FindGameObjectWithTag("Canvas").GetComponent<Image>();
    }
    
    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Water")
        {
            waterBlockCount++;
            WaterFilter.enabled = waterBlockCount > 0;
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Water")
        {
            waterBlockCount--;
            WaterFilter.enabled = waterBlockCount > 0;
        }
    }
}
