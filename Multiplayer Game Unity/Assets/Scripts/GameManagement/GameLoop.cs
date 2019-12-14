using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GameLoop : NetworkBehaviour
{
    public int maxPlayers = 2;
    int playerAmount = 0;

    public GameObject waitingForPlayersLabel;
    
    // Update is called once per frame
    void Update()
    {
        if (!isServer) return;

        // A player joined, now they are more than one and can play
        if (GameObject.FindGameObjectsWithTag("Player").Length == 2 && playerAmount == 1)
        {
            // Update all players I guess
            StartCoroutine(SetToWait(false));
        } 

        // A player left. There is only one so he can't play
        if(GameObject.FindGameObjectsWithTag("Player").Length == 1 && playerAmount == 2)
        {
            // Update all players I guess
            StartCoroutine(SetToWait(true));
        }
        playerAmount = GameObject.FindGameObjectsWithTag("Player").Length;

    }
    [ClientRpc]
    void RpcPlayersWaitState(bool wait)
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        foreach (GameObject player in players)
            player.GetComponent<DroneController>().waitingForPlayers = wait;

        waitingForPlayersLabel.SetActive(wait);
    }

    IEnumerator SetToWait(bool wait)
    {
        yield return new WaitForSeconds(0.5f);

        RpcPlayersWaitState(wait);
    }
}
