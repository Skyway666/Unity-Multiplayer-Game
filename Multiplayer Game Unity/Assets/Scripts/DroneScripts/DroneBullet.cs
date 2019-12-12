using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class DroneBullet : NetworkBehaviour
{

    public float speed = 100.0f;
    public float life = 4.0f;

    public int player = 1;


    public CustomNetworkManager networkManager;

    float spawnedTime = 0;

    [Command]
    void CmdDestroy(GameObject bullet)
    {
        networkManager.Destroy(bullet);
    }

    void Start()
    {
        spawnedTime = Time.time;

        NetworkManager mng = NetworkManager.singleton;
        networkManager = mng.GetComponent<CustomNetworkManager>();
    }
    // Update is called once per frame
    void Update()
    {
        transform.position += transform.forward * speed * Time.deltaTime;

        if (Time.time - spawnedTime > life)
            CmdDestroy(gameObject);
    }
}
