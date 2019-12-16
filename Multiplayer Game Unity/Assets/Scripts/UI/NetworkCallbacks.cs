using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NetworkCallbacks : MonoBehaviour
{
    CustomNetworkManager networkManager;

    // Lan party
    InputField adressInput;
    InputField portInput;
    InputField playerNameInput;

    // Matchmaking
    InputField roomInput;
    InputField playerNameInputRoom;

    public void Start()
    {

        networkManager = GameObject.FindGameObjectWithTag("NetworkManager").GetComponent<CustomNetworkManager>();

        // Lan party toggled
        if (GameObject.FindGameObjectWithTag("AdressInput"))
        {
            adressInput = GameObject.FindGameObjectWithTag("AdressInput").GetComponent<InputField>();
            portInput = GameObject.FindGameObjectWithTag("PortInput").GetComponent<InputField>();
            playerNameInput = GameObject.FindGameObjectWithTag("PlayerNameInput").GetComponent<InputField>();
        }

        // Matchmaking toggled
        if (GameObject.FindGameObjectWithTag("RoomNameInput"))
        {
            roomInput = GameObject.FindGameObjectWithTag("RoomNameInput").GetComponent<InputField>();
            playerNameInputRoom = GameObject.FindGameObjectWithTag("PlayerNameRoomInput").GetComponent<InputField>();
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




    public void updatePlayerNameInputRoom()
    {
        networkManager.playerName = playerNameInputRoom.text;
    }

    public void updateRoomName()
    {
        networkManager.newRoomName = roomInput.text;
    }


    public void joinMMRoom()
    {
        networkManager.JoinMatch();
    }
    public void createMMRoom()
    {
        networkManager.CreateRoom();
    }


    // Others

    public void shutDown()
    {
        Application.Quit();
    }


}
