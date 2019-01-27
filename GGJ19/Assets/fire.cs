using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fire : MonoBehaviour {
    public float travelSpeed = 10.0f;

    [SerializeField] private GameObject projectile;
    [SerializeField] private Transform leftBarrel;
    [SerializeField] private Transform rightBarrel;

    private Vector2 leftStick;
    private float fireTimer = 1.0f;
    bool cross = false;
    bool left = false;
    Vector3 neutralPosition;
    private GameObject muzzleLight;

	// Use this for initialization
	void Start () {
        neutralPosition = transform.position;
        muzzleLight = GameObject.Find("Muzzle Light");
        muzzleLight.GetComponent<Light>().intensity = 0;
	}
	
    public void UpdateInput()
    {
        leftStick.x = Input.GetAxis("LeftStickXAxis");
        leftStick.y = Input.GetAxis("LeftStickYAxis");


        cross = Input.GetButtonDown("Cross");
    }

	// Update is called once per frame
	void Update () {
        fireTimer -= Time.deltaTime;

        //transform.rotation = Quaternion.identity;
        //float stickAngle = Mathf.Atan2(leftStick.y, leftStick.x) * Mathf.Rad2Deg;
        transform.Rotate(Vector3.up, -40.0f * leftStick.x * Time.deltaTime);

        if (cross && leftStick.magnitude > 0.2f && fireTimer <= 0.0f)
        {
            Vector3 spawnPos;
            if (left)
            {
                spawnPos = leftBarrel.position;
                leftBarrel.GetComponent<Animator>().Play("GunBarrel - Left");
            }
            else
            {
                spawnPos = rightBarrel.position;
                rightBarrel.GetComponent<Animator>().Play("GunBarrel - Right");
            }
            muzzleLight.GetComponent<Animator>().Play("Muzzle Flash");

            left = !left;

            spawnPos.y = 0;
            GameObject proj = Instantiate(projectile, spawnPos, Quaternion.identity);
            proj.GetComponent<Rigidbody>().AddForce(-transform.forward * travelSpeed, ForceMode.Impulse);
            fireTimer = 1.0f;
        }

        transform.position = neutralPosition - transform.forward * 3;

	}
}
