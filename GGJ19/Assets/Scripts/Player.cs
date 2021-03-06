﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    GameManager gameManager;

    public enum AccessedObject
    {
        SteerAndGun,
        LightAndHarpoon,
        Nothing
    }

    public enum Stick
    {
        Left,
        Right
    }

    Spaceship spaceship;
    AccessedObject accessing = AccessedObject.Nothing;
    Controllable controlledObject = null;
    [HideInInspector] public float originalY;

    public float speed = 0.005f;

    private void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
        originalY = transform.position.y;
        spaceship = FindObjectOfType<Spaceship>();

        int layerMask = 1 << 8;
        layerMask = ~layerMask;
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out hit, Mathf.Infinity, layerMask))
        {
            transform.position += (Vector3.down * hit.distance);
            Vector3 tempPos = transform.position;

            GameObject player = GameObject.Find("Player");
            tempPos.y += player.GetComponent<SphereCollider>().radius * player.transform.localScale.y;

            transform.position = tempPos;
        }
    }

    // Use this for initialization
    private void Start()
    {

    }

    // Update is called once per frame
    private void Update()
    {
        if (FindObjectOfType<Spaceship>().health <= 0) { return; }
        // Exit controlled object
        if (Input.GetButtonDown("Triangle"))
        {
            accessing = AccessedObject.Nothing;

            if (controlledObject)
            {
                // Unparent from controllable
                transform.SetParent(controlledObject.transform.parent);

                controlledObject.isControlled = false;
                controlledObject = null;
            }
        }

        switch (accessing)
        {
            case AccessedObject.SteerAndGun:
                var input = Input.GetAxis("LeftStickXAxis");
                const float fuelBurnRate = 5;

                if (input != 0)
                {
                    spaceship.Rotate(Time.deltaTime * input);
                    spaceship.fuel -= Time.deltaTime * fuelBurnRate;
                }

                FindObjectOfType<Fire>().UpdateInput();
                break;

            case AccessedObject.LightAndHarpoon:
                FindObjectOfType<RopeScript>().UpdateInput();
                if (!FindObjectOfType<RopeScript>().ropeOut)
                {
                    ControlLight(GetRelativeStickDirection(Stick.Left)); 
                }
                break;

            case AccessedObject.Nothing:
                // Run around like a fool
                GetComponent<CharacterController>().Move(GetRelativeStickDirection(Stick.Left) * speed);
                break;

            default:
                // DON'T PUT ANYTHING HERE, WILL NEVER RUN
                break;
        }

    }

    private void ControlLight(Vector3 direction)
    {
        const float radius = 5;
        var spotLight = GameObject.Find("Spot Light Base");

        float angle = Vector3.Angle(Vector3.right, GetRelativeStickDirection(Stick.Right).normalized);

        Vector3 newPosition = gameManager.CalculatePointOnCircumference(radius, angle);

        const float rotationSpeed = 200.0f;
        spotLight.transform.Rotate(Vector3.up, rotationSpeed * GetRelativeStickDirection(Stick.Right).x * Time.deltaTime);
    }

    // Get the direction of the analog stick relative to the camera
    public Vector3 GetRelativeStickDirection(Stick stick)
    {
        Vector2 inputStick;

        switch (stick)
        {
            case Stick.Left:
                inputStick = new Vector2(Input.GetAxis("LeftStickXAxis"), Input.GetAxis("LeftStickYAxis"));
                break;

            case Stick.Right:
                inputStick = new Vector2(Input.GetAxis("RightStickXAxis"), Input.GetAxis("RightStickYAxis"));
                break;

            default:
                return Vector3.zero;
        }


        // Get camera forward and right unit vectors
        Vector3 forward = Camera.main.transform.forward;
        forward.y = 0;
        forward.Normalize();
        Vector3 right = Camera.main.transform.right;
        right.y = 0;
        right.Normalize();

        // Sync the pulling direction to the view angle
        return -((forward * inputStick.y) + (right * -inputStick.x)).normalized;
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
                    accessing = AccessedObject.SteerAndGun;
                }
                else if (other.name == "Harpoon")
                {
                    accessing = AccessedObject.LightAndHarpoon;
                }
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
