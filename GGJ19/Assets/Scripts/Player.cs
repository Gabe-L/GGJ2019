using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    Spaceship spaceship;
    [HideInInspector] public bool hasControlPanelAccess = false;
    [HideInInspector] public bool hasHarpoonAccess = false;
    [HideInInspector] public bool hasGunTurretAccess = false;

    public float speed = 0.005f;

    private void Awake()
    {
        spaceship = FindObjectOfType<Spaceship>();
    }

    // Use this for initialization
    private void Start()
    {

    }

    // Update is called once per frame
    private void Update()
    {
        // Exit controlled object
        if (Input.GetButtonDown("Triangle"))
        {
            hasControlPanelAccess = false;
            hasHarpoonAccess = false;
            hasGunTurretAccess = false;
        }

        if (hasControlPanelAccess)
        {
            spaceship.Rotate(Time.deltaTime * Input.GetAxis("LeftStickXAxis"));
        }
        else if (hasHarpoonAccess)
        {
            FindObjectOfType<RopeScript>().UpdateInput();
        }
        else if (hasGunTurretAccess)
        {
            FindObjectOfType<fire>().UpdateInput();
        }
        else
        {
            // Run around like a fool
            GetComponent<CharacterController>().Move(GetRelativeLeftStickDirection() * speed);
        }

    }

    // Get the direction of the analog stick relative to the camera
    private Vector3 GetRelativeLeftStickDirection()
    {
        Vector2 leftStick = new Vector2(Input.GetAxis("LeftStickXAxis"), Input.GetAxis("LeftStickYAxis"));

        // Get camera forward and right unit vectors
        Vector3 forward = Camera.main.transform.forward;
        forward.y = 0;
        forward.Normalize();
        Vector3 right = Camera.main.transform.right;
        right.y = 0;
        right.Normalize();

        // Sync the pulling direction to the view angle
        return -((forward * leftStick.y) + (right * -leftStick.x)).normalized;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Controllable>())
        {
            var controllable = other.GetComponent<Controllable>();

            if (other.name == "Control Panel")
            {
                controllable.isControlled = true;
                hasControlPanelAccess = true;
                Debug.Log(name + " gained access to the Control Panel");
            }
            else if (other.name == "Harpoon")
            {
                controllable.isControlled = true;
                hasHarpoonAccess = true;
                Debug.Log(name + " gained access to the Harpoon");
            }
            else if (other.name == "Gun Turret")
            {
                controllable.isControlled = true;
                hasGunTurretAccess = true;
                Debug.Log(name + " gained access to the Gun Turret");
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.name == "Control Panel")
        {
            
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<Controllable>())
        {
            var controllable = other.GetComponent<Controllable>();

            if (other.name == "Control Panel")
            {
                controllable.isControlled = false;
                hasControlPanelAccess = false;
                Debug.Log(name + " lost access to the Control Panel");
            }
            else if (other.name == "Harpoon")
            {
                controllable.isControlled = false;
                hasHarpoonAccess = false;
                Debug.Log(name + " lost access to the Harpoon");
            }
            else if (other.name == "Gun Turret")
            {
                controllable.isControlled = false;
                hasGunTurretAccess = false;
                Debug.Log(name + " lost access to the Gun Turret");
            } 
        }
    }
}
