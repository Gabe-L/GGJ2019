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
    const float projectileLifeTime = 5;

	// Use this for initialization
	void Start () {
        neutralPosition = transform.position;
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

        transform.rotation = Quaternion.identity;
        float stickAngle = Mathf.Atan2(leftStick.y, leftStick.x) * Mathf.Rad2Deg;
        transform.Rotate(Vector3.up, stickAngle);

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

            left = !left;

            spawnPos.y = 0;
            GameObject proj = Instantiate(projectile, spawnPos, Quaternion.identity);
            proj.GetComponent<Rigidbody>().AddForce(-transform.forward * travelSpeed, ForceMode.Impulse);
            fireTimer = 1.0f;


            Destroy(proj, projectileLifeTime);

            //var lsf = new Vector3(leftStick.x, 0, leftStick.y);
            //transform.position = neutralPosition + new Vector3(leftStick.x, 0, leftStick.y) * 5;
            //transform.forward = lsf.magnitude > 0.2f ? lsf.normalized : transform.forward;
        }

        transform.position = neutralPosition - transform.forward * 3;

	}
}
