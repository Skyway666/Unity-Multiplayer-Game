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
public class Objective : NetworkBehaviour
{
    // Start is called before the first frame update

    public AgentType type = AgentType.Shootable;

    public float maxSpeed = 0;
    public float minSpeed = 0;
    float speed = 3;

    float maxHeight = 60.0f;

    public CustomNetworkManager networkManager;
    public PointsManagement points;

    [Command]
    void CmdDestroy(GameObject objective)
    {
        networkManager.Destroy(objective);
    }

    private void Start()
    {

        points = GameObject.FindGameObjectWithTag("GameManager").GetComponent<PointsManagement>();

        NetworkManager mng = NetworkManager.singleton;
        networkManager = mng.GetComponent<CustomNetworkManager>();
        speed = Random.Range(minSpeed, maxSpeed);
    }
    // Update is called once per frame
    void Update()
    {
        transform.Rotate(new Vector3(1, 0, 0), 90 * Time.deltaTime);
        transform.position += new Vector3(0, speed * Time.deltaTime, 0);


        if (transform.position.y > maxHeight)
            Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        // A shootable object has been shot
        if(type == AgentType.Shootable && other.gameObject.tag == "Bullet" )
        {
            CmdDestroy(gameObject);
            CmdDestroy(other.gameObject);

            points.AddScore(other.gameObject.GetComponent<DroneBullet>().player, 50);

            Debug.Log("Points added to player");
        }
        if (type == AgentType.Collectable && other.gameObject.tag == "Player")
        {
            CmdDestroy(gameObject);
            // Give points to other.GameObject
            points.AddScore(other.gameObject.GetComponent<DroneController>().playerID, 100);
            Debug.Log("Lots of points added to player");
        }

    }
}
