﻿using UnityEngine;

public class Enemies : MonoBehaviour
{

    public GameObject Enemy;
    public float repeatTime = 10f;
    public float SpawnTimer = 0f;

    private void Update()
    {
        SpawnTimer += Time.deltaTime;
        if (SpawnTimer > repeatTime)
        {
            Instantiate(Enemy, transform.position, Quaternion.identity);
            SpawnTimer = 0f;
            repeatTime = Random.Range(10f, 30f);
        }

    }




    private void Start()
    {
       // InvokeRepeating("Enemy", 10f, repeatTime);
    }

    void Spawn()
    {
        Debug.Log("Enemy has spawned");
        Instantiate(Enemy, transform.position, Quaternion.identity);
        
    }





}

    


