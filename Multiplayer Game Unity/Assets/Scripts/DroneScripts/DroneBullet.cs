using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class DroneBullet : NetworkBehaviour
{

    public float speed = 100.0f;
    public float life = 4.0f;

    public int playerID = 1;
    public PointsManagement points;
    float spawnedTime = 0;


    public CustomNetworkManager networkManager;



    [Command]
    void CmdDestroy(GameObject GO)
    {
        NetworkServer.Destroy(GO);
    }
    [Command]
    void CmdAddPoints(int pointAmount)
    {
        points.RpcAddScore(playerID, pointAmount);
    }
    [Command]
    void CmdSpawn(int prefabIndex, Vector3 newPos, Quaternion rotation)
    {
        networkManager.Spawn(prefabIndex, newPos, rotation);
    }

    void Start()
    {
        NetworkManager mng = NetworkManager.singleton;
        networkManager = mng.GetComponent<CustomNetworkManager>();
        points = GameObject.FindGameObjectWithTag("GameManager").GetComponent<PointsManagement>();
        spawnedTime = Time.time;
    }
    // Update is called once per frame
    void Update()
    {
        transform.position += transform.forward * speed * Time.deltaTime;

        if (!hasAuthority) return;

        if (Time.time - spawnedTime > life)
            CmdDestroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {

        // A shootable object has been shot. ONLY IF BULLET IS LOCAL
        if (other.tag != "Agent" || !hasAuthority) return;


        AgentType type = other.gameObject.GetComponent<AgentBehaviour>().type;

        switch (type)
        {
            case AgentType.Shootable:
                {
                    // Points
                    CmdAddPoints(50);
                    // Particles
                    CmdSpawn((int)DroneScenesPrefabs.ShootableParticle, other.gameObject.transform.position, other.gameObject.transform.rotation);
                    // Destroy
                    CmdDestroy(other.gameObject);
                    CmdDestroy(gameObject);
                    break;
                }
            case AgentType.Obstacle:
                {
                    // Particles
                    CmdSpawn((int)DroneScenesPrefabs.ShootableParticle, gameObject.transform.position, gameObject.transform.rotation);
                    // Destroy
                    CmdDestroy(gameObject);
                    break;
                }
        }

    }
}
