using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public enum AgentType
{
    Shootable = 3,
    Collectable,
    Obstacle

}
public class AgentBehaviour : NetworkBehaviour
{
    // Start is called before the first frame update

    public AgentType type = AgentType.Shootable;

    public float maxSpeed = 0;
    public float minSpeed = 0;
    float speed = 3;

    float maxHeight = 60.0f;

    private void Start()
    {
        speed = Random.Range(minSpeed, maxSpeed);
    }
    // Update is called once per frame
    void Update()
    {
        if(type != AgentType.Obstacle)
            transform.Rotate(new Vector3(1, 0, 0), 90 * Time.deltaTime);

        transform.position += new Vector3(0, speed * Time.deltaTime, 0);


        if (!isServer) return;

        if (transform.position.y > maxHeight)
            NetworkServer.Destroy(gameObject);
    }

}
