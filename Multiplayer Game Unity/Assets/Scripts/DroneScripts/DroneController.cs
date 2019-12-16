using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class DroneController : NetworkBehaviour
{
    private Camera mainCamera;
    private TextMesh nameLabel;
    public CustomNetworkManager networkManager;

    public  float MOVEMENT_ACCELERATION = 0.5f;
    public  float MOVEMENT_ACCELERATION_STABILIZATION_SPEED = 0.5f;
    public  float MAX_SPEED = 15.0f;


    public  float MAX_TILT_ANGLE = 20.0f;
    public  float Y_ROTATION_SPEED = 180.0f;
    public  float XZ_ROTATION_SPEED = 90.0f;
    public  float XZ_ROTATION_STABILIZATION_SPEED = 40.0f;

    public int playerID = 1;


    // Custom values
    Vector3 customEulerAngles = new Vector3(0, 0, 0);
    Vector3 speed = new Vector3(0, 0, 0);


    // Axis input...
    int rightAxisLastValue = 0;

    // Drone Limits
    float limitSquareSize = 50.0f;
    float minHeight = 0.0f;

    // External scripts
    PointsManagement points;

    // Others
    public bool waitingForPlayers = true;
    public string startingName = "Player";

    // Name sync /////////////////////////////////////
    [SyncVar(hook = "SyncNameChanged")]
    public string playerName = "Player";

    [Command]
    void CmdChangeName(string name) { playerName = name; }
    [Command]
    void CmdSpawnBullet()
    { 
        networkManager.SpawnBullet((int)DroneScenesPrefabs.Bullet, gameObject);
    }
    [Command]
    void CmdSpawn(int prefabIndex, Vector3 newPos, Quaternion rotation)
    {
        networkManager.Spawn(prefabIndex, newPos, rotation);
    }
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

    void SyncNameChanged(string name) { nameLabel.text = name; }

    // OnGUI /////////////////////////////////////////
    private void OnGUI()
    {
        if (!isLocalPlayer) return;

        GUILayout.BeginArea(new Rect(Screen.width - 260, 10, 250, Screen.height - 20));

        string prevPlayerName = playerName;
        playerName = GUILayout.TextField(playerName);
        if (playerName != prevPlayerName)
        {
            CmdChangeName(playerName);
        }

        GUILayout.EndArea();
    }

    // Lifecycle methods ////////////////////////////


    // Use this for initialization
    void Awake()
    {
        mainCamera = Camera.main;
        nameLabel = transform.Find("Label").gameObject.GetComponent<TextMesh>();

        NetworkManager mng = NetworkManager.singleton;
        networkManager = mng.GetComponent<CustomNetworkManager>();
        points = GameObject.FindGameObjectWithTag("GameManager").GetComponent<PointsManagement>();

        if (isLocalPlayer) { 
            // Set main camera in the first frame
            Vector3 Forward2d = (new Vector3(transform.forward.x, 0, transform.forward.z)).normalized;
            if (mainCamera)
            {
                Vector3 offset = -Forward2d * 8;
                offset.y += 5;
                mainCamera.transform.SetPositionAndRotation(transform.position + offset, Quaternion.identity);
                mainCamera.transform.LookAt(transform.position + new Vector3(0.0f, 0.0f, 0.0f), Vector3.up);
            }
        }

        playerName = startingName;
    }

    // Update is called once per frame
    void Update()
    {
        if (nameLabel)
        {
            nameLabel.transform.LookAt(Camera.main.transform);

            nameLabel.transform.forward = -nameLabel.transform.forward;
        }

        if (!isLocalPlayer || waitingForPlayers) return;

        // 2d Axis
        Vector3 Forward2d = (new Vector3(transform.forward.x, 0, transform.forward.z)).normalized;
        Vector3 Right2d = (new Vector3(transform.right.x, 0, transform.right.z)).normalized;


        // Input
        Vector2 LeftAxis = new Vector2(Input.GetAxis("HorizontalLeft"), Input.GetAxis("VerticalLeft"));
        Vector2 RightAxis = new Vector2(Input.GetAxis("HorizontalRight"), Input.GetAxis("VerticalRight"));


        // FUCK UNITY
        bool fire = Input.GetButtonDown("Fire") || Input.GetAxis("Fire") > 0.0f && rightAxisLastValue == 0;
        rightAxisLastValue = (int)Input.GetAxis("Fire");

        // Normalize Axis, shouldn't go faster in diagonal
        if (LeftAxis.magnitude > 1)
            LeftAxis.Normalize();
        if (RightAxis.magnitude > 1)
            RightAxis.Normalize();



        // Fire

        if (fire)
            CmdSpawnBullet();
    
        // Left axis

        // Forward
        if (LeftAxis.y > 0.0 )
        {
            speed += Forward2d * LeftAxis.y * MOVEMENT_ACCELERATION * Time.deltaTime;

            if (customEulerAngles.x < MAX_TILT_ANGLE)
                customEulerAngles.x += XZ_ROTATION_SPEED * Time.deltaTime;

        }
        // Backwards
        else if (LeftAxis.y < 0.0)
        {
            speed += Forward2d * LeftAxis.y * MOVEMENT_ACCELERATION * Time.deltaTime;

            if (customEulerAngles.x > -MAX_TILT_ANGLE)
               customEulerAngles.x -= XZ_ROTATION_SPEED * Time.deltaTime;
        }

        // Right
        if (LeftAxis.x > 0.0f)
        {
            speed += Right2d * LeftAxis.x * MOVEMENT_ACCELERATION * Time.deltaTime;

            if (customEulerAngles.z > -MAX_TILT_ANGLE)
                customEulerAngles.z -= XZ_ROTATION_SPEED * Time.deltaTime;
        }
        // Left
        else if (LeftAxis.x < 0.0f)
        {
            speed += Right2d * LeftAxis.x * MOVEMENT_ACCELERATION * Time.deltaTime;

            if (customEulerAngles.z < MAX_TILT_ANGLE)
                customEulerAngles.z += XZ_ROTATION_SPEED * Time.deltaTime;
        }

        // Righ axis
        // Up
        if(RightAxis.y > 0.0f)
        {
            speed += transform.up * RightAxis.y * MOVEMENT_ACCELERATION * Time.deltaTime;
        }
        // Down
        else if(RightAxis.y < 0.0f)
        {
            speed += transform.up * RightAxis.y * MOVEMENT_ACCELERATION * Time.deltaTime;
        }


        float angle = 0.0f;

        // Rotate right
        if (RightAxis.x > 0.0f)
        {
            angle = RightAxis.x * Time.deltaTime * Y_ROTATION_SPEED;
            customEulerAngles.y += angle;
        }
        // Rotate left
        else if (RightAxis.x < 0.0f)
        {
            angle = RightAxis.x * Time.deltaTime * Y_ROTATION_SPEED;
            customEulerAngles.y += angle;
        }







        // Stabilization of left axis angles
        if (LeftAxis.y == 0)
        {

            // Too much x
            if (customEulerAngles.x > 0)
                customEulerAngles.x -= XZ_ROTATION_STABILIZATION_SPEED * Time.deltaTime;

            // Too few x
            if (customEulerAngles.x < 0)
                customEulerAngles.x += XZ_ROTATION_STABILIZATION_SPEED * Time.deltaTime;
        }
        if (LeftAxis.x == 0)
        {
            // ANGLE
            // Too much z
            if (customEulerAngles.z > 0)
                customEulerAngles.z -= XZ_ROTATION_STABILIZATION_SPEED * Time.deltaTime;

            // Too few z
            if (customEulerAngles.z < 0)
                customEulerAngles.z += XZ_ROTATION_STABILIZATION_SPEED * Time.deltaTime;

        }



        // Stabilization of right axis movement
        if (RightAxis.y == 0)
        {
            // Too much y
            if (speed.y > 0)
                speed.y -= MOVEMENT_ACCELERATION_STABILIZATION_SPEED * Time.deltaTime;

            // Too few y
            if (speed.y < 0)
                speed.y += MOVEMENT_ACCELERATION_STABILIZATION_SPEED * Time.deltaTime;
        }
        // Stabilization of left axis movement TODO: Handle properly, where if the player stops pressing one of the two axis
        // said axis is stabilized
        if (LeftAxis.magnitude == 0)
        {
            // Too much x
            if (speed.x > 0)
                speed.x -= MOVEMENT_ACCELERATION_STABILIZATION_SPEED * Time.deltaTime;

            // Too few x
            if (speed.x < 0)
                speed.x += MOVEMENT_ACCELERATION_STABILIZATION_SPEED * Time.deltaTime;

            // Speed
            // Too much z
            if (speed.z > 0)
                speed.z -= MOVEMENT_ACCELERATION_STABILIZATION_SPEED * Time.deltaTime;

            // Too few z
            if (speed.z < 0)
                speed.z += MOVEMENT_ACCELERATION_STABILIZATION_SPEED * Time.deltaTime;

        }



        // Force euler angles 
        transform.rotation = Quaternion.Euler(customEulerAngles.x, customEulerAngles.y, customEulerAngles.z);

        // Apply speed

        float normalizedSpeed = MAX_SPEED * Time.deltaTime;
        if (speed.magnitude > normalizedSpeed)
            speed = speed.normalized * normalizedSpeed;
        transform.position += speed;

        // Top
        if (transform.position.y > limitSquareSize)
            transform.position = new Vector3(transform.position.x, limitSquareSize, transform.position.z);
        // Bottom
        if (transform.position.y < 0)
            transform.position = new Vector3(transform.position.x, 0, transform.position.z);
        // Right
        if (transform.position.x > limitSquareSize)
            transform.position = new Vector3(limitSquareSize, transform.position.y, transform.position.z);
        // Left
        if (transform.position.x < -limitSquareSize)
            transform.position = new Vector3(-limitSquareSize, transform.position.y, transform.position.z);
        // Front
        if (transform.position.z > limitSquareSize)
            transform.position = new Vector3(transform.position.x, transform.position.y, limitSquareSize);
        // Back
        if (transform.position.z < -limitSquareSize)
            transform.position = new Vector3(transform.position.x, transform.position.y, -limitSquareSize);

        if (mainCamera)
        {
            Vector3 offset = -Forward2d * 8;
            offset.y += 5;
            mainCamera.transform.SetPositionAndRotation(transform.position + offset, Quaternion.identity);
            mainCamera.transform.LookAt(transform.position + new Vector3(0.0f, 0.0f, 0.0f), Vector3.up);
        }

    }


    private void OnTriggerEnter(Collider other)
    {

        if (other.tag != "Agent" || !isLocalPlayer || waitingForPlayers) return;

        // A shootable object has been shot. ONLY IF BULLET IS LOCAL
        AgentType type = other.gameObject.GetComponent<AgentBehaviour>().type;

        switch (type){
            case AgentType.Collectable:
                {
                    CmdAddPoints(100);
                    CmdSpawn((int)DroneScenesPrefabs.CollectableParticle, other.gameObject.transform.position, other.gameObject.transform.rotation);
                    CmdDestroy(other.gameObject);
 

                    break;
                }
            case AgentType.Obstacle:
                {
                    CmdAddPoints(-200);
                    CmdSpawn((int)DroneScenesPrefabs.ObstacleParticle, other.gameObject.transform.position, other.gameObject.transform.rotation);
                    CmdDestroy(other.gameObject);
                    break;
                }
        }

    }

}
