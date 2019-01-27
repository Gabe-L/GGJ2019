﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    GameObject asteroidLargePrefab;
    GameObject asteroidMediumPrefab;
    GameObject asteroidSmallPrefab;
    Spaceship spaceship;

    const float maxAsteroidCount = 4;
    const int bigAsteroidChance = 20;
    const int mediumAsteroidChance = bigAsteroidChance + 50;
    const int smallAsteroidChance = mediumAsteroidChance + 100;
    [Range(0, 100)] public int asteroidSpawnChancePercent = 3;
    GameObject UICanvas;
    RawImage shipHealthBar;

    private void Awake()
    {
        asteroidLargePrefab = Resources.Load<GameObject>("Prefabs/Asteroid Large");
        asteroidMediumPrefab = Resources.Load<GameObject>("Prefabs/Asteroid Medium");
        asteroidSmallPrefab = Resources.Load<GameObject>("Prefabs/Asteroid Small");
        spaceship = FindObjectOfType<Spaceship>();
        UICanvas = GameObject.Find("Canvas");
        shipHealthBar = GameObject.Find("healthBar").GetComponent<RawImage>();
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
        int maxPercent = (smallAsteroidChance / asteroidSpawnChancePercent) * 100;
        int randomNumber = Random.Range(0, maxPercent);
        if (GameObject.FindGameObjectsWithTag("Asteroid").Length <= maxAsteroidCount && 
            randomNumber < bigAsteroidChance)
        {
            SpawnAsteroid(asteroidLargePrefab);
        }
        else if (randomNumber < mediumAsteroidChance)
        {
            SpawnAsteroid(asteroidMediumPrefab);
        }
        else if (randomNumber < smallAsteroidChance)
        {
            SpawnAsteroid(asteroidSmallPrefab);
        }

        // Remove asteroids out of view
        foreach (var asteroid in GameObject.FindGameObjectsWithTag("Asteroid"))
        {
            if (Vector3.Distance(asteroid.transform.position, Vector3.zero) > 50)
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
        //Rect tempHealthRect = shipHealthBar.rectTransform.rect;
        //tempHealthRect.width = 75.0f * healthProp;
        //shipHealthBar.rectTransform.rect = tempHealthRect;

    }

    private void SpawnAsteroid(GameObject prefab)
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

        var asteroid = Instantiate<GameObject>(prefab, start, Quaternion.identity);
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
