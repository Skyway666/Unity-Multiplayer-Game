using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;
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
    public string newRoomName = "SampleRoom";


    // LAN STUFF
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



    // Commands
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


    // Matchmaking

        // Room
    public void CreateRoom()
    {
        Debug.Log("Creating room: " + newRoomName);
        StartMatchMaker();
        matchMaker.CreateMatch(newRoomName, 2, true, "", "", "", 0, 0, OnMatchCreate);
        
    }

    public void OnMatchCreate(bool success, string extendedInfo, MatchInfo matchInfo)
    {
        StopMatchMaker();

        if (success)
            StartHost(matchInfo);
        else
            Debug.Log("Failed when creating match");
    }

        // Client
    public void JoinMatch()
    {
        StartMatchMaker();
        matchMaker.ListMatches(0, 10, "", true, 0, 0, OnMatchList);
    }

    public void OnMatchList(bool success, string extendedInfo, List<MatchInfoSnapshot> matches)
    {
        for(int i = 0; i < matches.Count; i++)
        {
            MatchInfoSnapshot match = matches[i];

            if(newRoomName == match.name)
            {
                StartCoroutine(ActuallyJoinMatch(match.networkId));
                return;
            }
        }

        Debug.Log("Match not found");
    }

    IEnumerator ActuallyJoinMatch(UnityEngine.Networking.Types.NetworkID selectedMatchId)
    {
        yield return new WaitForSeconds(1.0f);
        StartMatchMaker();
        matchMaker.JoinMatch(selectedMatchId, "", "", "", 0, 0, OnMatchJoin);
    }

    public void OnMatchJoin(bool success, string extendedInfo, MatchInfo matchInfo)
    {
        StopMatchMaker();

        if (success)
            StartClient(matchInfo);
        else
            Debug.Log("Failed when creating match");
    }


}
