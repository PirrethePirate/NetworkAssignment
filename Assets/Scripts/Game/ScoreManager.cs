using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;

public class ScoreManager : NetworkBehaviour
{
    [Header("Points")]
    [SerializeField] private int normalTarget = 1;
    [SerializeField] private TextMeshProUGUI scoreText;

    [Header("Amount of Players")]
    public int playerAmount = 0;

    [Header("Points Per Player")]
    private int player1 = 0;
    private int player2 = 0;
    private int player3 = 0;
    private int player4 = 0;

    private static readonly string[] PlayerColorNames = { "Blue", "Green", "Magenta", "Yellow" };

    [ServerRpc(RequireOwnership = false)]
    public void AwardPointServerRpc(int playernumber)
    {
        switch (playernumber)
        {
            case 1:
                player1 += normalTarget;
                break;
            case 2:
                player2 += normalTarget;
                break;
            case 3:
                player3 += normalTarget;
                break;
            case 4:
                player4 += normalTarget;
                break;
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void AddPlayersServerRpc()
    {
        if (IsServer)
        playerAmount++;
        Debug.Log(playerAmount);
    }

    [ServerRpc]
    public void ShowScoreboardServerRpc()
    {
        if (!IsServer) return;

        List<(int playerNumber, int score)> scores = new List<(int, int)>();

        if (playerAmount >= 1) scores.Add((1, player1));
        if (playerAmount >= 2) scores.Add((2, player2));
        if (playerAmount >= 3) scores.Add((3, player3));
        if (playerAmount >= 4) scores.Add((4, player4));

        scores.Sort((a, b) => b.score.CompareTo(a.score));

        string finalScore = "Final Scores:\n";
        foreach (var score in scores)
        {
            int playerIndex = score.playerNumber - 1;
            string colorName = PlayerColorNames[playerIndex];
            finalScore += $"{colorName}: {score.score} points\n";
        }

        UpdateScoreboardClientRpc(finalScore);
    }

    [ClientRpc]
    private void UpdateScoreboardClientRpc(string result)
    {
        if (scoreText != null)
        {
            scoreText.text = result;
        }
    }

    [ServerRpc]
    public void ResetPointsServerRpc()
    {
        if (IsServer)
        {
            ResetPointsClientRpc();
        }
    }
    [ClientRpc]
    public void ResetPointsClientRpc()
    {
        player1 = 0;
        player2 = 0;
        player3 = 0;
        player4 = 0;
        scoreText.text = "";
    }
}
