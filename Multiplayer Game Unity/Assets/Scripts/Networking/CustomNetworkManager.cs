using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class MsgTypes{
    public const short PlayerPrefabSelect = MsgType.Highest + 1;
    public class PlayerPrefabMsg: MessageBase
    {
        public short controllerID;
        public string playerName;
    }

}

public enum DroneScenesPrefabs
{
    PlayerDrone,
    Cylinder,
    Bullet,
    CollectableParticle = 6,
    ObstacleParticle,
    ShootableParticle
}


public class CustomNetworkManager : NetworkManager
{
    public string playerName = "Player";


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
        playerPrefab = spawnPrefabs[0];
        DroneController player = playerPrefab.GetComponent<DroneController>();
        player.playerID = NetworkServer.connections.Count;
        player.startingName = msg.playerName;
        base.OnServerAddPlayer(netMsg.conn, msg.controllerID);
    }

    public void OnPrefabRequest(NetworkMessage netMsg)
    {
        MsgTypes.PlayerPrefabMsg msg = netMsg.ReadMessage<MsgTypes.PlayerPrefabMsg>();
        msg.playerName = playerName;
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
