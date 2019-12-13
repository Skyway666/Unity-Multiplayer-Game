using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ObjectiveSpawner : NetworkBehaviour
{


    // Enviroment variables
    float SpawnHeight = -10;
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



    public CustomNetworkManager networkManager;


    [Command]
    void CmdSpawn(int prefabIndex,Vector3 newPos, Quaternion rotation)
    {
        networkManager.Spawn(prefabIndex, newPos, rotation);
    }

    private void Start()
    {
        NetworkManager mng = NetworkManager.singleton;
        networkManager = mng.GetComponent<CustomNetworkManager>();
    }
    void Update()
    {
        if (!isServer) return;
        
        if(Time.time - lastSpawnedShootableTime > nextShootableSpawnInterval)
        {
            // Reset timer
            lastSpawnedShootableTime = Time.time;
            nextShootableSpawnInterval = Random.Range(0.5f, 3.0f);

            // Spawn Object
            Vector3 spawnPos = new Vector3(Random.Range(-XSpawnThreshHold, XSpawnThreshHold),
            SpawnHeight, Random.Range(-ZSpawnThreshHold, ZSpawnThreshHold));

            CmdSpawn((int)AgentType.Shootable, spawnPos, Quaternion.identity);
        }

        if (Time.time - lastSpawnedCollectableTime > nextCollectableSpawnInterval)
        {
            // Reset timer
            lastSpawnedCollectableTime = Time.time;
            nextCollectableSpawnInterval = Random.Range(3.0f, 5.0f);

            // Spawn Object
            Vector3 spawnPos = new Vector3(Random.Range(-XSpawnThreshHold, XSpawnThreshHold),
            SpawnHeight, Random.Range(-ZSpawnThreshHold, ZSpawnThreshHold));

            CmdSpawn((int)AgentType.Collectable, spawnPos, Quaternion.identity);
        }

        if (Time.time - lastSpawnedObstacleTime > nextObstacleSpawnInterval)
        {
            // Reset timer
            lastSpawnedObstacleTime = Time.time;
            nextObstacleSpawnInterval = Random.Range(1f, 3.0f);

            // Spawn Object
            Vector3 spawnPos = new Vector3(Random.Range(-XSpawnThreshHold, XSpawnThreshHold),
            SpawnHeight, Random.Range(-ZSpawnThreshHold, ZSpawnThreshHold));

            CmdSpawn((int)AgentType.Obstacle, spawnPos, Quaternion.identity);
        }
    }
}
