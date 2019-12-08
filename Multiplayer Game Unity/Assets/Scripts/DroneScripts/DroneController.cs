using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class DroneController : NetworkBehaviour
{
    private Camera mainCamera;
    private TextMesh nameLabel;

    const float MOVEMENT_SPEED = 10.0f;
    const float MAX_TILT_ANGLE = 20.0f;

    const float Y_ROTATION_SPEED = 180.0f;
    const float X_ROTATION_SPEED = 60.0f;


    Vector3 customEulerAngles = new Vector3(0,0,0);

    // Name sync /////////////////////////////////////
    [SyncVar(hook = "SyncNameChanged")]
    public string playerName = "Player";

    [Command]
    void CmdChangeName(string name) { playerName = name; }
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

    public CustomNetworkManager networkManager;
    // Use this for initialization
    void Start()
    {
        mainCamera = Camera.main;
        nameLabel = transform.Find("Label").gameObject.GetComponent<TextMesh>();

        NetworkManager mng = NetworkManager.singleton;
        networkManager = mng.GetComponent<CustomNetworkManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (nameLabel)
        {
            nameLabel.transform.LookAt(Camera.main.transform);

            nameLabel.transform.forward = -nameLabel.transform.forward;
        }

        if (!isLocalPlayer) return;


        Vector3 Forward2d = (new Vector3(transform.forward.x, 0, transform.forward.z)).normalized;
        Vector3 Right2d = (new Vector3(transform.right.x, 0, transform.right.z)).normalized;
        float angle = 0.0f;


        Vector2 LeftAxis = new Vector2(Input.GetAxis("HorizontalLeft"), Input.GetAxis("VerticalLeft"));
        Vector2 RightAxis = new Vector2(Input.GetAxis("HorizontalRight"), Input.GetAxis("VerticalRight"));


        if (LeftAxis.magnitude > 1)
            LeftAxis.Normalize();
        if (RightAxis.magnitude > 1)
            RightAxis.Normalize();


        // Left axis

        // Forward
        if (LeftAxis.y > 0.0 )
        {
            transform.position += Forward2d * LeftAxis.y * MOVEMENT_SPEED * Time.deltaTime;

            if (transform.rotation.eulerAngles.x < MAX_TILT_ANGLE || transform.rotation.eulerAngles.x > 180)
                transform.Rotate(new Vector3(1, 0, 0), X_ROTATION_SPEED * Time.deltaTime);

        }
        // Backwards
        else if (LeftAxis.y < 0.0)
        {
            transform.position += Forward2d * LeftAxis.y * MOVEMENT_SPEED * Time.deltaTime;

            if (transform.rotation.eulerAngles.x > 360 - MAX_TILT_ANGLE || transform.rotation.eulerAngles.x < 180)
                transform.Rotate(new Vector3(1, 0, 0), -X_ROTATION_SPEED * Time.deltaTime);
        }

        // Right
        if (LeftAxis.x > 0.0f)
        {
            transform.position += Right2d * LeftAxis.x * MOVEMENT_SPEED * Time.deltaTime;

            if (transform.rotation.eulerAngles.z > 360 - MAX_TILT_ANGLE || transform.rotation.eulerAngles.z < 180)
                transform.Rotate(new Vector3(0, 0, 1), -X_ROTATION_SPEED * Time.deltaTime);
        }
        // Left
        else if (LeftAxis.x < 0.0f)
        {
            transform.position += Right2d * LeftAxis.x * MOVEMENT_SPEED * Time.deltaTime;

            if (transform.rotation.eulerAngles.z <  MAX_TILT_ANGLE || transform.rotation.eulerAngles.z > 180)
                transform.Rotate(new Vector3(0, 0, 1), X_ROTATION_SPEED * Time.deltaTime);
        }

        // Righ axis
        // Up
        if(RightAxis.y > 0.0f)
        {
            transform.Translate(new Vector3(0.0f, RightAxis.y * MOVEMENT_SPEED * Time.deltaTime, 0.0f));
        }
        // Down
        else if(RightAxis.y < 0.0f)
        {
            transform.Translate(new Vector3(0.0f, RightAxis.y * MOVEMENT_SPEED * Time.deltaTime, 0.0f));
        }
        // Rotate right
        if (RightAxis.x > 0.0f)
        {
            angle = RightAxis.x * Time.deltaTime * Y_ROTATION_SPEED;
            transform.Rotate(new Vector3(0.0f, 1.0f, 0.0f), angle);
        }
        // Rotate left
        else if (RightAxis.x < 0.0f)
        {
            angle = RightAxis.x * Time.deltaTime * Y_ROTATION_SPEED;
            transform.Rotate(new Vector3(0.0f, 1.0f, 0.0f), angle);
        }



        // Stabilization





        // Force euler angles

        //transform.rotation = Quaternion.Euler(customEulerAngles.x, customEulerAngles.y, customEulerAngles.z);

        if (mainCamera)
        {
            Vector3 offset = -Forward2d * 8;
            offset.y += 5;
            mainCamera.transform.SetPositionAndRotation(transform.position + offset, Quaternion.identity);
            mainCamera.transform.LookAt(transform.position + new Vector3(0.0f, 0.0f, 0.0f), Vector3.up);
        }

    }

    private void OnDestroy()
    {
    }

}
