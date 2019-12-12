using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PointsManagement : MonoBehaviour
{
    int[] playerScores = new int[2];
    public Text[] playerScoresUI = new Text[2];



    private void Start()
    {
        for (int i = 0; i < 2; i++)
            playerScores[i] = 0;
    }

    // Start is called before the first frame update
    public void AddScore(int player, int amount)
    {
        playerScores[player-1] += amount;
        playerScoresUI[player-1].text = "Player " + player + ": " + playerScores[player-1];
    }
}
