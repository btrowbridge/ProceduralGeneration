using UnityEngine;
using System.Collections;
using UnityStandardAssets.Characters.FirstPerson;
using System;

public class Swim : MonoBehaviour {
    [Range(30,100)]
    public float SwimForce = 40.0F;
    [Range(0,1)]
    public float SwimRate = 0.2F;
    [Range(0, 2)]
    public float WaterGravityResist = 1.5F;

    private float m_NextSwim;
    private float m_OriginalJumpForce;

    private WaterCam waterCam;
    private Rigidbody m_Rigidbody;
    private RigidbodyFirstPersonController controller;
    private AudioManager audioManager;
	// Use this for initialization
	void Start ()
    {
        waterCam = GetComponentInChildren<WaterCam>();
        m_Rigidbody = GetComponent<Rigidbody>();
        controller = GetComponent<RigidbodyFirstPersonController>();
        audioManager = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<AudioManager>();
        m_OriginalJumpForce = controller.movementSettings.JumpForce;
    }
	
    
	// Update is called once per frame
	void Update () {
        if (waterCam.IsUnderwater) {
            controller.movementSettings.JumpForce = SwimForce/2;
            if (Input.GetKey(KeyCode.Space) && Time.time >= m_NextSwim)
            {
                m_NextSwim = Time.time + SwimRate;
                Stroke();
                
            }
            //other water things
            
        }
        else
        {
            controller.movementSettings.JumpForce = m_OriginalJumpForce;
        }

    }

    private void Stroke()
    {
        audioManager.PlaySoundAtLocation("Swim", transform.position);
        m_Rigidbody.AddForce(new Vector3(0, 1, 0) * SwimForce * m_Rigidbody.mass);
    }

    void FixedUpdate()
    {
        if (waterCam.IsUnderwater)
        {
            //slow fall
            m_Rigidbody.AddForce(Physics.gravity * -WaterGravityResist * m_Rigidbody.mass);
        }
    }
}
