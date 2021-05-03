﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameManager : MonoBehaviour
{
    public GameObject Minion;
    public GameObject Box;
    public GameObject Log;

    public Vector3 EnemySpawn = new Vector3(0,0,0);

    public int InitialEnemySpeed = 2;
    public int EnemyAcceleration = 2;

    private Queue<GameObject> activeEnemies = new Queue<GameObject>();
    private Queue<GameObject> inactiveEnemies = new Queue<GameObject>();

    private int enemySpeed;
    private float spawnTimer = 0.0f;
    private float timeToSpawn = 2.0f;

    // Start is called before the first frame update
    void Start()
    {
        GameObject enemy;
        enemySpeed = InitialEnemySpeed;

        //Create a pool of enemies
        enemy = Instantiate(Minion, EnemySpawn, Quaternion.identity);
        inactiveEnemies.Enqueue(enemy);
        enemy = Instantiate(Box, EnemySpawn, Quaternion.identity);
        inactiveEnemies.Enqueue(enemy);
        enemy = Instantiate(Log, EnemySpawn, Quaternion.identity);
        inactiveEnemies.Enqueue(enemy);
    }

    // Update is called once per frame
    void Update()
    {
        PlayerInput();

        spawnTimer += Time.deltaTime;
        if (spawnTimer >= timeToSpawn)
        {
            SpawnEnemy();
            spawnTimer = 0.0f;
            Debug.Log("Spawned Enemy!");
        }
    }

    private void FixedUpdate()
    {
        //Move all enemies to the left a certain speed
        foreach (GameObject enemy in activeEnemies)
        {
            enemy.transform.position -= transform.right * Time.fixedDeltaTime * enemySpeed;

            if (enemy.transform.position.x < -10)
                ResetEnemy(enemy);
        }
    }

    private void PlayerInput()
    {
        
    }

    private void SpawnEnemy()
    {
        GameObject enemy;

        enemy = inactiveEnemies.Dequeue();
        activeEnemies.Enqueue(enemy);
    }

    private void ResetEnemy(GameObject enemy)
    {
        GameObject resetEnemy;

        enemy.transform.position = EnemySpawn;
        resetEnemy = activeEnemies.Dequeue();
        inactiveEnemies.Enqueue(resetEnemy);
    }
}
