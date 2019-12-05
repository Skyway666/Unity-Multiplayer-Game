using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class DroneController : NetworkBehaviour
{
    private Camera mainCamera;
    private TextMesh nameLabel;

    const float MOVEMENT_SPEED = 10.0f;
    const float ROTATION_SPEED = 180.0f;

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
            nameLabel.transform.rotation = Quaternion.identity;
        }

        if (!isLocalPlayer) return;

        Vector3 translation = new Vector3();
        float angle = 0.0f;

        float horizontalAxis = Input.GetAxis("Horizontal");
        float verticalAxis = Input.GetAxis("Vertical");

        if (verticalAxis > 0.0)
        {
            translation += new Vector3(0.0f, 0.0f, verticalAxis * MOVEMENT_SPEED * Time.deltaTime);
            transform.Translate(translation);
        }
        else if (verticalAxis < 0.0)
        {
            translation += new Vector3(0.0f, 0.0f, verticalAxis * MOVEMENT_SPEED * Time.deltaTime * 0.5f);
            transform.Translate(translation);
        }

        if (horizontalAxis > 0.0f)
        {
            angle = horizontalAxis * Time.deltaTime * ROTATION_SPEED;
            transform.Rotate(new Vector3(0.0f, 1.0f, 0.0f), angle);
        }
        else if (horizontalAxis < 0.0f)
        {
            angle = horizontalAxis * Time.deltaTime * ROTATION_SPEED;
            transform.Rotate(new Vector3(0.0f, 1.0f, 0.0f), angle);
        }

        if (mainCamera)
        {
            mainCamera.transform.SetPositionAndRotation(transform.position + new Vector3(0.0f, 2.0f, -4.0f), Quaternion.identity);
            mainCamera.transform.LookAt(transform.position + new Vector3(0.0f, 0.0f, 0.0f), Vector3.up);
        }

    }

    private void OnDestroy()
    {
    }

}
