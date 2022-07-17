﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wave : MonoBehaviour
{
    private enum WaveState
    {
        Uninitiated,
        Idle,
        WaveStarted,
        WaveComplete
    };

    [SerializeField] private WaveSO waveData;

    private SpawnType waveSpawnType;
    private SpawnLocation spawnLocationType;
    private EnemyWaveType enemyWaveType;

    private List<GameObject> enemies = new List<GameObject>();
    private Vector2 spawnBoxDimensions;
    private Vector2 spawnBoxPosition;

    private float timer = 0;
    [SerializeField]private WaveState waveState;
 
    // Start is called before the first frame update
    void Awake()
    {
        waveState = WaveState.Uninitiated;

        waveSpawnType = waveData.waveSpawnType;
        spawnLocationType = waveData.spawnLocationType;
        enemyWaveType = waveData.enemyWaveType;
        
        spawnBoxDimensions.x = waveData.spawnBoxWidth;
        spawnBoxDimensions.y = waveData.spawnBoxHeight;

        spawnBoxPosition.x = waveData.spawnBoxPosX;
        spawnBoxPosition.y = waveData.spawnBoxPosY;

        enemies = InstantiateEnemies(waveData.enemies);
    }

    // Update is called once per frame
    void Update()
    {
        if (waveState == WaveState.Idle) // || waveState == WaveState.WaveComplete) // It should only check WaveComplete state to restart if it's supposed to re-spawn
        {
            Debug.Log("Timer: " + timer);

            timer += Time.deltaTime;
            if (timer > 1)  // Need to change from being hard-coded to be a timer and for only a second
            {
                Debug.Log("Spawning Wave!");
                waveState = WaveState.WaveStarted;
                SpawnWave();
            }
        }

        if (waveState == WaveState.WaveStarted && IsWaveOver())
        {
            Debug.Log("WAVE IS OVER!!!");
            waveState = WaveState.WaveComplete;
            timer = 0;
        }

    }

    // Copied snippet from user 'chilemanga' (https://answers.unity.com/questions/461588/drawing-a-bounding-box-similar-to-box-collider.html)
    // It has been modified so the gizmos will be drawn from the ScriptableObject data so it can be seen outside of runtime.
    // I should be able to modify this to loop through multiple spawnboxes
    void OnDrawGizmos() 
    {
        if (spawnLocationType == SpawnLocation.SpawnBox)
        {
            Gizmos.color = Color.yellow;
            float wHalf = (waveData.spawnBoxWidth * .5f);
            float hHalf = (waveData.spawnBoxHeight * .5f);
            Vector3 topLeftCorner = new Vector3 (waveData.spawnBoxPosX - wHalf, waveData.spawnBoxPosY + hHalf, 1f);
            Vector3 topRightCorner = new Vector3 (waveData.spawnBoxPosX + wHalf, waveData.spawnBoxPosY + hHalf, 1f);
            Vector3 bottomLeftCorner = new Vector3 (waveData.spawnBoxPosX - wHalf, waveData.spawnBoxPosY - hHalf, 1f);
            Vector3 bottomRightCorner = new Vector3 (waveData.spawnBoxPosX + wHalf, waveData.spawnBoxPosY - hHalf, 1f);
            Gizmos.DrawLine (topLeftCorner, topRightCorner);
            Gizmos.DrawLine (topRightCorner, bottomRightCorner);
            Gizmos.DrawLine (bottomRightCorner, bottomLeftCorner);
            Gizmos.DrawLine (bottomLeftCorner, topLeftCorner);
        }
         
     }

    // Instantiates the list of enemy prefabs from the Wave Scriptable Object
    private List<GameObject> InstantiateEnemies(GameObject[] enemyPrefabList)
    {
        List<GameObject> enemyList = new List<GameObject>();
        GameObject enemy;

        for(int i = 0; i < enemyPrefabList.Length; i++)
        {
            enemy = Instantiate(enemyPrefabList[i]);
            enemy.SetActive(false);
            enemyList.Add(enemy);
        }

        return enemyList;
    }
    
    private void SpawnWave()
    {
        if (enemies.Count > 0)
        {
            foreach (GameObject e in enemies)
            {
                if (spawnLocationType == SpawnLocation.SpawnBox)
                {
                    float leftBound = spawnBoxPosition.x - spawnBoxDimensions.x/2f;
                    float rightBound = spawnBoxPosition.x + spawnBoxDimensions.x/2f;
                    float bottomBound = spawnBoxPosition.y - spawnBoxDimensions.y/2f;
                    float topBound = spawnBoxPosition.y + spawnBoxDimensions.y/2f;

                    Vector2 spawnPos = new Vector2(Random.Range(leftBound, rightBound), Random.Range(bottomBound, topBound));
                    e.GetComponent<Enemy>().SpawnEnemy(spawnPos);
                    e.SetActive(true);
                }
                else
                    e.GetComponent<Enemy>().SpawnEnemy();
            }
        }
    }

    private bool IsWaveOver()
    {
        foreach (GameObject e in enemies)
        {
            if (e.GetComponent<Enemy>().IsAlive)
                return false;
        }

        return true;
    }

    // Method used by the 'Level' class to initiate the wave
    // This may actually not be necessary if I just keep the GameObject inactive.
    public void InitiateWave()
    {
        waveState = WaveState.Idle;
        Debug.Log("Initiating Wave!");
    }

    // Used by the 'Level' class to easily check when the wave is complete. 
    public bool IsWaveComplete()
    {
        if (waveState == WaveState.WaveComplete)
            return true;
        
        return false;
    }

}
