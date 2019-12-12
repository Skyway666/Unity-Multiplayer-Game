using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Obstacle : NetworkBehaviour
{
    float maxHeight = 60.0f;

    public CustomNetworkManager networkManager;
    public PointsManagement points;

    [Command]
    void CmdDestroy(GameObject obstacle)
    {
        networkManager.Destroy(obstacle);
    }

    private void Start()
    {

        points = GameObject.FindGameObjectWithTag("GameManager").GetComponent<PointsManagement>();
        NetworkManager mng = NetworkManager.singleton;
        networkManager = mng.GetComponent<CustomNetworkManager>();
    }

    void Update()
    {
        transform.position += new Vector3(0, 3 * Time.deltaTime, 0);

        if (transform.position.y > maxHeight)
            CmdDestroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.tag == "Player")
        {
            CmdDestroy(gameObject);
            // Substract points to other.GameObject
            points.AddScore(other.gameObject.GetComponent<DroneController>().playerID, -200);
            Debug.Log("Points substracted from player");
        }

        if(other.gameObject.tag == "Bullet")
        {
            CmdDestroy(other.gameObject);

            Debug.Log("Destroyed bullet against obstacle");
        }

    }
}
