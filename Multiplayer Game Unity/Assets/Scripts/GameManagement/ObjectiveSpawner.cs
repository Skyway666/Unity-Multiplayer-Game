using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectiveSpawner : MonoBehaviour
{
    // Prefabs
    public GameObject shootable;
    public GameObject collectable;
    public GameObject obstacle;


    // Enviroment variables
    float SpawnHeight = -4;
    float XSpawnThreshHold = 50;
    float ZSpawnThreshHold = 50;


    // Shootable Spawn Time
    float lastSpawnedShootableTime = 0;
    float nextShootableSpawnInterval = 0; // Random value between 0.5 and 3 seconds

    // Collectable Spawn Time
    float lastSpawnedCollectableTime = 0;
    float nextCollectableSpawnInterval = 0; // Random value between 3 and 5 seconds

    // Obstacle spawn Time
    float lastSpawnedObstacleTime = 0;
    float nextObstacleSpawnInterval = 0; // Random value between 3 and 5 seconds

    // Update is called once per frame
    void Update()
    {
        if(Time.time - lastSpawnedShootableTime > nextShootableSpawnInterval)
        {
            // Reset timer
            lastSpawnedShootableTime = Time.time;
            nextShootableSpawnInterval = Random.Range(0.5f, 3.0f);

            // Spawn Object
            Vector3 spawnPos = new Vector3(Random.Range(-XSpawnThreshHold, XSpawnThreshHold),
            SpawnHeight, Random.Range(-ZSpawnThreshHold, ZSpawnThreshHold));

            Instantiate(shootable, spawnPos, Quaternion.identity);
        }

        if (Time.time - lastSpawnedCollectableTime > nextCollectableSpawnInterval)
        {
            // Reset timer
            lastSpawnedCollectableTime = Time.time;
            nextCollectableSpawnInterval = Random.Range(3.0f, 5.0f);

            // Spawn Object
            Vector3 spawnPos = new Vector3(Random.Range(-XSpawnThreshHold, XSpawnThreshHold),
            SpawnHeight, Random.Range(-ZSpawnThreshHold, ZSpawnThreshHold));

            Instantiate(collectable, spawnPos, Quaternion.identity);
        }

        if (Time.time - lastSpawnedObstacleTime > nextObstacleSpawnInterval)
        {
            // Reset timer
            lastSpawnedObstacleTime = Time.time;
            nextObstacleSpawnInterval = Random.Range(1f, 3.0f);

            // Spawn Object
            Vector3 spawnPos = new Vector3(Random.Range(-XSpawnThreshHold, XSpawnThreshHold),
            SpawnHeight, Random.Range(-ZSpawnThreshHold, ZSpawnThreshHold));

            Instantiate(obstacle, spawnPos, Quaternion.identity);
        }
    }
}
