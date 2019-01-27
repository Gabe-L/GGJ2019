using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spaceship : MonoBehaviour
{

    [HideInInspector] public float health;                  // damaged by asteroids
    [HideInInspector] public float fuel;                    // slowly consumed when moving/rotating the ship
    [HideInInspector] public float oxygen;                  // slowly consumed by the players (more if they move more)
    [Range(0, 360)] public float turningSpeed = 200.0f;

    // Use this for initialization
    private void Start()
    {
        health = 100;
        fuel = 100;
        oxygen = 100;
    }

    // Update is called once per frame
    private void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<CharacterJoint>())
        {
            //FindObjectOfType<RopeScript>().RemoveJoint();
        }
    }

    /// <summary>
    /// Rotate spaceship by angle
    /// </summary>
    /// <param name="deltaAngle">in degrees</param>
    public void Rotate(float deltaAngle)
    {
        var newRotation = transform.localRotation;

        newRotation *= Quaternion.Euler(0, Time.deltaTime * -turningSpeed * Input.GetAxis("LeftStickXAxis"), 0);

        if (Input.GetKey(KeyCode.A))
        {
            newRotation *= Quaternion.Euler(0, Time.deltaTime * -turningSpeed, 0);
        }

        if (Input.GetKey(KeyCode.D))
        {
            newRotation *= Quaternion.Euler(0, Time.deltaTime * turningSpeed, 0);
        }

        transform.rotation = newRotation;
    }

}
