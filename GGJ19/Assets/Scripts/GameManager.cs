using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    GameObject asteroidPrefab;

    private void Awake()
    {
        asteroidPrefab = Resources.Load<GameObject>("Prefabs/Asteroid Medium");
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
        if (Random.Range(0, 100) < 2)
        {
            SpawnAsteroid();
        }
    }

    private void SpawnAsteroid()
    {
        Vector3 start = RandomPosition(16, 20);
        Vector3 target = RandomPosition(3, 15);
        Vector3 direction = (target - start).normalized;
        float speed = Random.Range(0.3f, 1.0f);
        Vector3 velocity = direction * speed;
        float avRange = 0.5f;
        var angularVelocity = new Vector3(
            Random.Range(-avRange, avRange),
            Random.Range(-avRange, avRange),
            Random.Range(-avRange, avRange)
        );

        var asteroid = Instantiate<GameObject>(asteroidPrefab, start, Quaternion.identity);

        var asteroidRigidbody = asteroid.GetComponent<Rigidbody>();
        asteroidRigidbody.velocity = velocity;
        asteroidRigidbody.angularVelocity = angularVelocity;
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
