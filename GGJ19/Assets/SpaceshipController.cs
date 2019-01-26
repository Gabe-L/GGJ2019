using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceshipController : MonoBehaviour
{

    [Range(0, 360)] public float turningSpeed = 200.0f;
    GameObject asteroidPrefab;

    // Use this for initialization
    private void Start()
    {
        asteroidPrefab = Resources.Load<GameObject>("Prefabs/Asteroid Medium");
    }

    // Update is called once per frame
    private void Update()
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

        // Spawn asteroid
        if (Random.Range(0, 100) < 2)
        {
            SpawnAsteroid();
        }

        transform.rotation = newRotation;
    }

    private void SpawnAsteroid()
    {
        Vector3 start = RandomPosition(16, 20);

        Vector3 target = RandomPosition(3, 15);

        var asteroid = Instantiate<GameObject>(asteroidPrefab, start, Quaternion.identity);

        float speed = Random.Range(0.1f, 0.3f);
        Vector3 velocity = (target - start) * speed;
        asteroid.GetComponent<Rigidbody>().velocity = velocity;
    }

    private Vector3 RandomPosition(float minRange, float maxRange)
    {
        Vector3 output = new Vector3();

        float angle = Random.Range(-360, 360);
        float radius = Random.Range(minRange, maxRange);

        output.x = radius * Mathf.Cos(Random.Range(-360, 360));

        output.y = transform.position.y;
        
        output.z = radius * Mathf.Sin(Random.Range(-360, 360));

        return output;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<CharacterJoint>())
        {
            FindObjectOfType<RopeScript>().RemoveJoint();
        }
    }

}
