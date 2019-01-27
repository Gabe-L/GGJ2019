using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fire : MonoBehaviour {
    public float travelSpeed = 10.0f;

    [SerializeField] private GameObject projectile;
    [SerializeField] private Transform leftBarrel;
    [SerializeField] private Transform leftBarrelEnd;
    [SerializeField] private Transform rightBarrel;
    [SerializeField] private Transform rightBarrelEnd;

    private Vector2 inputStick;
    private float fireTimer = 1.0f;
    bool L1 = false;
    bool left = false;
    Vector3 neutralPosition;
    //private GameObject muzzleLight;
    const float projectileLifeTime = 5;

    private void Awake()
    {
        leftBarrelEnd = GameObject.Find("Barrel 1 Spawn Position").transform;
        rightBarrelEnd = GameObject.Find("Barrel 2 Spawn Position").transform;
    }

    // Use this for initialization
    void Start () {
        neutralPosition = transform.position;
        //muzzleLight = GameObject.Find("Muzzle Light");
        //muzzleLight.GetComponent<Light>().intensity = 0;
	}
	
    public void UpdateInput()
    {
        inputStick.x = Input.GetAxis("RightStickXAxis");
        inputStick.y = Input.GetAxis("RightStickYAxis");


        L1 = Input.GetButtonDown("L1") || Input.GetButtonDown("R1");
    }

	// Update is called once per frame
	void Update () {
        fireTimer -= Time.deltaTime;

        //transform.rotation = Quaternion.identity;
        //float stickAngle = Mathf.Atan2(leftStick.y, leftStick.x) * Mathf.Rad2Deg;
        transform.Rotate(Vector3.up, -40.0f * inputStick.x * Time.deltaTime);

        if (L1 && fireTimer <= 0.0f)
        {
            Vector3 spawnPos;
            if (left)
            {
                spawnPos = leftBarrelEnd.position;
                leftBarrel.GetComponent<Animator>().Play("GunBarrel - Left");
            }
            else
            {
                spawnPos = rightBarrelEnd.position;
                rightBarrel.GetComponent<Animator>().Play("GunBarrel - Right");
            }
            //muzzleLight.GetComponent<Animator>().Play("Muzzle Flash");

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
