using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NetworkCallbacks : MonoBehaviour
{
    CustomNetworkManager networkManager;


    InputField adressInput;
    InputField portInput;
    InputField playerNameInput;

    public void Start()
    {

        networkManager = GameObject.FindGameObjectWithTag("NetworkManager").GetComponent<CustomNetworkManager>();
        if (GameObject.FindGameObjectWithTag("AdressInput"))
        {
            adressInput = GameObject.FindGameObjectWithTag("AdressInput").GetComponent<InputField>();
            portInput = GameObject.FindGameObjectWithTag("PortInput").GetComponent<InputField>();
            playerNameInput = GameObject.FindGameObjectWithTag("PlayerNameInput").GetComponent<InputField>();
        }
    }

    public void Stop()
    {
        networkManager.StopClient();
        networkManager.StopServer();
    }

    public void StartClient()
    {
        networkManager.StartClient();
    }

    public void StartHost()
    {
        networkManager.StartHost();
    }

    public void updateNetworkAdress()
    {
        networkManager.networkAddress = adressInput.text;
    }

    public void updateNetworkPort()
    {
        networkManager.networkPort = int.Parse(portInput.text);
    }

    public void updatePlayerName()
    {
        networkManager.playerName = playerNameInput.text;
    }

}
