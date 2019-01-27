using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    GameObject asteroidLargePrefab;
    GameObject asteroidMediumPrefab;
    GameObject asteroidSmallPrefab;
    GameObject healthPickupPrefab;
    GameObject fuelPickupPrefab;

    Spaceship spaceship;

    const float maxAsteroidCount = 4;

    //const int fuelChance = 30;
    //const int healthChance = fuelChance + 30;
    //const int bigAsteroidChance = healthChance + 20;
    //const int mediumAsteroidChance = bigAsteroidChance + 30;
    //const int smallAsteroidChance = mediumAsteroidChance + 50;
    [Range(0, 100)] public int asteroidSpawnChancePercent = 3;
    [Range(1, 100)] public float spawnRadius = 20;

    GameObject UICanvas;
    RawImage shipHealthBar;
    RawImage shipFuelBar;
    private void OnDrawGizmos()
    
    {
        Gizmos.color = Color.magenta / 3;
        Gizmos.DrawSphere(transform.position, spawnRadius);
    }

    private void Awake()
    {
        asteroidLargePrefab = Resources.Load<GameObject>("Prefabs/Asteroid Large");
        asteroidMediumPrefab = Resources.Load<GameObject>("Prefabs/Asteroid Medium");
        asteroidSmallPrefab = Resources.Load<GameObject>("Prefabs/Asteroid Small");
        healthPickupPrefab = Resources.Load<GameObject>("Prefabs/Health Pickup");
        fuelPickupPrefab = Resources.Load<GameObject>("Prefabs/Fuel Pickup");

        spaceship = FindObjectOfType<Spaceship>();
        
        UICanvas = GameObject.Find("Canvas");
        shipHealthBar = GameObject.Find("healthBar").GetComponent<RawImage>();
        shipFuelBar = GameObject.Find("fuelBar").GetComponent<RawImage>();
    }

    // Use this for initialization
    private void Start()
    {

    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetButtonDown("Share"))
        {
            spaceship.health = 0;
        }

        // Reset scene
        if (Input.GetButtonDown("Option"))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        // Spawn asteroid
        //int maxPercent = (smallAsteroidChance / asteroidSpawnChancePercent) * 100;
        //int randomNumber = Random.Range(0, maxPercent);

        float spawnPercent = 10;

        // Spawn asteroids
        if (GameObject.FindGameObjectsWithTag("Asteroid").Length <= maxAsteroidCount)
        {
            if (Random.Range(0, 100) < spawnPercent)
            {
                SpawnFloatingObject(asteroidSmallPrefab);
            }

            if (Random.Range(0, 100) < spawnPercent)
            {
                SpawnFloatingObject(asteroidMediumPrefab);
            }

            if (Random.Range(0, 100) < spawnPercent)
            {
                SpawnFloatingObject(asteroidLargePrefab);
            }
            SpawnFloatingObject(asteroidLargePrefab);
        }

        spawnPercent = 5;

        // Spawn collectibles
        if (Random.Range(0, 100) < spawnPercent)
        {
            SpawnFloatingObject(asteroidSmallPrefab);
        }

        if (Random.Range(0, 100) < spawnPercent)
        {
            SpawnFloatingObject(asteroidSmallPrefab);
        }

        // Remove asteroids out of view
        foreach (var asteroid in GameObject.FindGameObjectsWithTag("Asteroid"))
        {
            if (Vector3.Distance(asteroid.transform.position, Vector3.zero) > spawnRadius)
            {
                Destroy(asteroid);
            }
        }

        // Teleport players at the center of the ship when they accidentally leave it
        Vector3 spaceshipPosition = FindObjectOfType<Spaceship>().transform.position;
        foreach (var player in FindObjectsOfType<Player>())
        {
            if (Vector3.Distance(spaceship.transform.position, player.transform.position) > 3)
            {
                Vector3 newPlayerPosition = spaceshipPosition;
                newPlayerPosition.y = player.originalY;
                player.gameObject.transform.position = newPlayerPosition;
            }
        }
    }

    private void LateUpdate()
    {
        UpdateUI();
    }

    private void UpdateUI()
    {
        float healthProp = FindObjectOfType<Spaceship>().health / FindObjectOfType<Spaceship>().maxHealth;
        shipHealthBar.GetComponent<RectTransform>().sizeDelta = new Vector2(75.0f * healthProp, 15.0f);

        float fuelProp = FindObjectOfType<Spaceship>().fuel / FindObjectOfType<Spaceship>().maxFuel;
        shipFuelBar.GetComponent<RectTransform>().sizeDelta = new Vector2(75.0f * fuelProp, 15.0f);
    }

    private void SpawnFloatingObject(GameObject prefab)
    {
        Vector3 start = CalculatePointOnCircumference(spawnRadius, Random.Range(0, 360));
        Vector3 target = CalculatePointOnCircumference(spawnRadius * 0.8f, Random.Range(0, 360));
        Vector3 direction = (target - start).normalized;
        float speed = Random.Range(0.3f, 10.0f);
        Vector3 velocity = direction * speed;
        float range = 1.0f;
        var angularVelocity = new Vector3(
            range - Random.Range(-range, range),
            range = Random.Range(-range, range),
            range = Random.Range(-range, range)
        );

        var asteroid = Instantiate(prefab, start, Quaternion.identity);

        if (Vector3.Distance(asteroid.transform.position, FindObjectOfType<Spaceship>().transform.position) < 3)
        {
            Destroy(asteroid);
            return;
        }

        var asteroidRigidbody = asteroid.GetComponent<Rigidbody>();

        asteroidRigidbody.velocity = velocity;
        asteroidRigidbody.angularVelocity = angularVelocity * asteroidRigidbody.mass;
    }

    public Vector3 CalculatePointOnCircumference(float radius, float angle)
    {

        float x = transform.position.x + radius * Mathf.Cos(angle);
        float y = 0;
        float z = transform.position.z + radius * Mathf.Sin(angle);

        return new Vector3(x, y, z);
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

    public void GameOver()
    {
        // Stop receiving player input
        // Slowly fade out the scene
        // Show scores?
        // Reset game

        GameObject.Find("gameOverImage").GetComponent<RawImage>().enabled = true;
        GameObject.Find("gameOverImage 2").GetComponent<RawImage>().enabled = true;

        Debug.Log("YOU'RE DEAD");
    }

}
