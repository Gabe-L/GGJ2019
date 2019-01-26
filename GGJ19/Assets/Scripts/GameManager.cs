using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    GameObject asteroidPrefab;
    Spaceship spaceship;


    private void Awake()
    {
        asteroidPrefab = Resources.Load<GameObject>("Prefabs/Asteroid Large");
        spaceship = FindObjectOfType<Spaceship>();
    }

    // Use this for initialization
    private void Start()
    {
        
    }

    // Update is called once per frame
    private void Update()
    {
        // Reset scene
        if (Input.GetButtonDown("Option"))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        // Spawn asteroid
        const float maxAsteroidCount = 4;
        if (GameObject.FindGameObjectsWithTag("Asteroid").Length <= maxAsteroidCount && 
            Random.Range(0, 200) < 3)
        {
            SpawnAsteroid();
        }

        // Remove asteroids out of view
        foreach (var asteroid in GameObject.FindGameObjectsWithTag("Asteroid"))
        {
            if (Vector3.Distance(asteroid.transform.position, Vector3.zero) > 50)
            {
                Destroy(asteroid);
            }
        }
    }

    private void SpawnAsteroid()
    {
        Vector3 start = RandomPosition(16, 20);
        Vector3 target = RandomPosition(3, 15);
        Vector3 direction = (target - start).normalized;
        float speed = Random.Range(0.3f, 1.0f);
        Vector3 velocity = direction * speed;
        float avRange = 1.0f;
        var angularVelocity = new Vector3(
            avRange - Random.Range(-avRange, avRange),
            avRange = Random.Range(-avRange, avRange),
            avRange = Random.Range(-avRange, avRange)
        );

        var asteroid = Instantiate<GameObject>(asteroidPrefab, start, Quaternion.identity);
        var asteroidRigidbody = asteroid.GetComponent<Rigidbody>();

        asteroidRigidbody.velocity = velocity;
        asteroidRigidbody.angularVelocity = angularVelocity * asteroidRigidbody.mass;
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

}
