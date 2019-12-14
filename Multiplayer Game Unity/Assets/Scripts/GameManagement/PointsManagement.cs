using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class PointsManagement : NetworkBehaviour
{
    public int winScore = 1000;
    int[] playerScores = new int[2];
    public Text[] playerScoresUI = new Text[2];

    public GameObject winnerLabel;
    public Text winnerText;

    [ClientRpc]
    public void RpcAddScore(int player, int amount)
    {
        playerScores[player - 1] += amount;
        playerScoresUI[player - 1].text = "Player " + player + ": " + playerScores[player - 1];
    }
    [ClientRpc]
    public void RpcDeclareWinner(int winner)
    {
        winnerLabel.SetActive(true);
        winnerText.text = "Player " + winner + " WINS!";
        for (int i = 0; i < 2; i++)
            playerScores[i] = 0;

        StartCoroutine(DelayedResetGame());
    }
    private void Start()
    {
        for (int i = 0; i < 2; i++)
            playerScores[i] = 0;
    }

    private void Update()
    {
        if (!isServer) return;

        // Player 1 wins
        if(playerScores[0] >= winScore)
        {
            RpcDeclareWinner(1);
        } else if(playerScores[1] >= winScore)
        {
            RpcDeclareWinner(2);
        }
    }

    IEnumerator DelayedResetGame()
    {
        yield return new WaitForSeconds(1.0f);
        winnerLabel.SetActive(false);
        for (int i = 0; i < 2; i++) 
            playerScoresUI[i].text = "Player " + (i+1) + ": " + playerScores[i];
        
    }
}
