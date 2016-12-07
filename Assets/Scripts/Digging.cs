using UnityEngine;
using System.Collections;
using System;

public class Digging : MonoBehaviour {
    
    [Range(0,1)]
    public float DigRate;
    [Range(1, 10)]
    public float DigDamage;
    [Range(2, 10)]
    public float DigRange;

	// Update is called once per frame
	void Update () {
        if (Input.GetButton("Fire1") && Time.time >= nextDig)
        {
            //Debug.Log("Time: " + Time.time + ", nextDig: " + nextDig);
            nextDig = Time.time + DigRate;
            Dig();
        }
    }

    private void Dig()
    {
        RaycastHit hitInfo;
        if (Physics.Raycast(new Ray(transform.position, transform.forward), out hitInfo, DigRange)) 
        {
            var blockProperties = hitInfo.collider.gameObject.GetComponent<BlockProperties>();
            if (blockProperties != null)
            {
                blockProperties.DigHit(DigDamage);
            }
        }
    }

    private float nextDig = 0.0F;
}
