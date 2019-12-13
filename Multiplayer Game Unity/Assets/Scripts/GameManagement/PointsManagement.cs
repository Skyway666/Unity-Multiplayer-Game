using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class PointsManagement : NetworkBehaviour
{
    int[] playerScores = new int[2];
    public Text[] playerScoresUI = new Text[2];

    [ClientRpc]
    public void RpcAddScore(int player, int amount)
    {
        playerScores[player - 1] += amount;
        playerScoresUI[player - 1].text = "Player " + player + ": " + playerScores[player - 1];
    }
    private void Start()
    {
        for (int i = 0; i < 2; i++)
            playerScores[i] = 0;
    }
}
