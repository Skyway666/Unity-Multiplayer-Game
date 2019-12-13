using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class MsgTypes{
    public const short PlayerPrefabSelect = MsgType.Highest + 1;
    public class PlayerPrefabMsg: MessageBase
    {
        public short controllerID;
        public short prefabIndex;
    }

}

public enum DroneScenesPrefabs
{
    PlayerDrone,
    Cylinder,
    Bullet
}


public class CustomNetworkManager : NetworkManager
{
    public short playerPrefabIndex = 0;

    public string[] playerNames = new string[] { "Boy", "Girl", "Robot" };
    private void OnGUI()
    {
        if (!isNetworkActive)
        {
            playerPrefabIndex = (short)GUI.SelectionGrid(new Rect(Screen.width - 200, 10, 200, 50), playerPrefabIndex, playerNames, 3);
        }
    }

    public override void OnStartServer()
    {
        NetworkServer.RegisterHandler(MsgTypes.PlayerPrefabSelect, OnPrefabResponse);
        base.OnStartServer();
    }

    public override void OnClientConnect(NetworkConnection conn)
    {
        client.RegisterHandler(MsgTypes.PlayerPrefabSelect, OnPrefabRequest);
        base.OnClientConnect(conn);
    }

    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
    {
        MsgTypes.PlayerPrefabMsg msg = new MsgTypes.PlayerPrefabMsg();
        msg.controllerID = playerControllerId;
        NetworkServer.SendToClient(conn.connectionId,MsgTypes.PlayerPrefabSelect, msg);
    }

    public void OnPrefabResponse(NetworkMessage netMsg)
    {
        MsgTypes.PlayerPrefabMsg msg = netMsg.ReadMessage<MsgTypes.PlayerPrefabMsg>();
        playerPrefab = spawnPrefabs[msg.prefabIndex];
        playerPrefab.GetComponent<DroneController>().playerID = NetworkServer.connections.Count;
        base.OnServerAddPlayer(netMsg.conn, msg.controllerID);
    }

    public void OnPrefabRequest(NetworkMessage netMsg)
    {
        MsgTypes.PlayerPrefabMsg msg = netMsg.ReadMessage<MsgTypes.PlayerPrefabMsg>();
        msg.prefabIndex = playerPrefabIndex;
        client.Send(MsgTypes.PlayerPrefabSelect, msg);
    }

    public void ChangePlayerPrefab(PlayerController currentPlayer, short prefabIndex)
    {
        GameObject newPlayer = Instantiate(spawnPrefabs[prefabIndex], currentPlayer.gameObject.transform.position, currentPlayer.gameObject.transform.rotation);

        NetworkServer.Destroy(currentPlayer.gameObject);

        NetworkServer.ReplacePlayerForConnection(currentPlayer.connectionToClient, newPlayer, 0);
    }

    public void Destroy(GameObject go)
    {
        NetworkServer.Destroy(go);
    }

    public void Spawn(int prefabIndex, Vector3 newPos, Quaternion rotation)
    {
        NetworkServer.Spawn(Instantiate(spawnPrefabs[prefabIndex], newPos, rotation));
    }

    public void SpawnBullet(int prefabIndex, GameObject father)
    {
        GameObject bullet = Instantiate(spawnPrefabs[prefabIndex], father.transform.position, father.transform.rotation);

        bullet.GetComponent<DroneBullet>().playerID = father.GetComponent<DroneController>().playerID;

        NetworkServer.SpawnWithClientAuthority(bullet, father);
    }


}
