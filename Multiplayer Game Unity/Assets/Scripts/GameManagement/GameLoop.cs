using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GameLoop : NetworkBehaviour
{
    public int maxPlayers = 2;
    int lastNetworkCount = 0;
    
    // Update is called once per frame
    void Update()
    {
        if (!isServer) return;

        return;

        if (NetworkServer.connections.Count == 2 && lastNetworkCount == 1)
        {
            // Update all players I guess
            StartCoroutine(SetToWait(false));
        } 

        if(NetworkServer.connections.Count == 1 && lastNetworkCount == 2)
        {
            // Update all players I guess
            StartCoroutine(SetToWait(true));
        }
        lastNetworkCount = NetworkServer.connections.Count;

    }
    [ClientRpc]
    void RpcPlayersWaitState(bool wait)
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        foreach (GameObject player in players)
            player.GetComponent<DroneController>().waitingForPlayers = wait;

        GameObject.FindGameObjectWithTag("WaitingForPlayersLabel").SetActive(wait);
    }

    IEnumerator SetToWait(bool wait)
    {
        yield return new WaitForSeconds(0.5f);

        RpcPlayersWaitState(wait);
    }
}
