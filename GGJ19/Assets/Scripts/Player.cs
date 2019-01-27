using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public enum AccessedObject
    {
        ControlPanel,
        Harpoon,
        GunTurret,
        Nothing
    }

    Spaceship spaceship;
    AccessedObject accessing = AccessedObject.Nothing;
    Controllable controlledObject = null;
    [HideInInspector] public float originalY;

    public float speed = 0.005f;

    private void Awake()
    {
        originalY = transform.position.y;
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
            accessing = AccessedObject.Nothing;

            if (controlledObject)
            {
                // Unparent from controllable
                transform.SetParent(controlledObject.transform.parent);

                controlledObject.isControlled = false;
                Debug.Log("Stopped controlling " + controlledObject.name);
                controlledObject = null;
            }
        }

        switch (accessing)
        {
            case AccessedObject.ControlPanel:
                spaceship.Rotate(Time.deltaTime * Input.GetAxis("LeftStickXAxis"));
                break;

            case AccessedObject.Harpoon:
                break;

            case AccessedObject.GunTurret:
                FindObjectOfType<fire>().UpdateInput();
                break;

            case AccessedObject.Nothing:
                // Run around like a fool
                GetComponent<CharacterController>().Move(GetRelativeLeftStickDirection() * speed);
                break;

            default:
                // DON'T PUT ANYTHING HERE, WILL NEVER RUN
                break;
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
            controlledObject = other.GetComponent<Controllable>();

            if (!controlledObject.isControlled)
            {
                // Set as parent to stick to it
                transform.SetParent(controlledObject.transform);
                var newPosition = controlledObject.transform.position;
                newPosition.y = transform.position.y;
                transform.position = newPosition;

                // Start controlling it
                controlledObject.isControlled = true;

                if (other.name == "Control Panel")
                {
                    accessing = AccessedObject.ControlPanel;
                }
                else if (other.name == "Harpoon")
                {
                    accessing = AccessedObject.Harpoon;
                }
                else if (other.name == "Gun Turret")
                {
                    accessing = AccessedObject.GunTurret;
                }

                Debug.Log(name + " gained access to the " + this.controlledObject.name);
            }
            else
            {
                controlledObject = null;
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        
    }

    private void OnTriggerExit(Collider other)
    {
        var controllable = other.GetComponent<Controllable>();

        // If by accident the player is knocked off the controllable, stop controlling it
        if (controllable && controlledObject == controllable && controllable.isControlled)
        {
            if (controlledObject)
            {
                // Unparent from controllable
                transform.SetParent(controlledObject.transform.parent);
                controlledObject = null; 
            }

            controllable.isControlled = false;
            accessing = AccessedObject.Nothing;
        }
    }
}
