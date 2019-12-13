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


    [Command]
    void CmdDestroy(GameObject GO)
    {
        NetworkServer.Destroy(GO);
    }

    void Start()
    {
        points = GameObject.FindGameObjectWithTag("GameManager").GetComponent<PointsManagement>();
        spawnedTime = Time.time;
    }
    // Update is called once per frame
    void Update()
    {
        transform.position += transform.forward * speed * Time.deltaTime;

        if (!localPlayerAuthority) return;

        if (Time.time - spawnedTime > life)
            CmdDestroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {

        // A shootable object has been shot. ONLY IF BULLET IS LOCAL

        if (other.tag != "Agent") return;
        AgentType type = other.gameObject.GetComponent<AgentBehaviour>().type;

        switch (type)
        {
            case AgentType.Shootable:
                {
                    points.AddScore(playerID, 50);
                    CmdDestroy(other.gameObject);
                    CmdDestroy(gameObject);


                    break;
                }
            case AgentType.Obstacle:
                {
                    CmdDestroy(gameObject);
                    break;
                }
        }

    }
}
