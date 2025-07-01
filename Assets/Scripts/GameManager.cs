using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public PlayerStats player1;
    public PlayerStats player2;
    public float matchTime = 180f;

    void Start()
    {
        player1.playerName = "Player 1";
        player2.playerName = "Player 2";
    }

    void Update()
    {
        matchTime -= Time.deltaTime;
        if (matchTime <= 0f)
        {
            EvaluateWinner();
        }
    }

    void EvaluateWinner()
    {
        if (player1.score > player2.score) EndGame(player1);
        else if (player2.score > player1.score) EndGame(player2);
        else ActivateSuddenDeath();
    }

    void EndGame(PlayerStats winner)
    {
        Debug.Log("Winner: " + winner.playerName);
    }

    public void HandleDeath(PlayerStats deadPlayer)
    {
        PlayerStats killer = deadPlayer == player1 ? player2 : player1;
        killer.score++;
        Debug.Log($"{deadPlayer.playerName} died — {killer.playerName} gains 1 score!");
        StartCoroutine(Respawn(deadPlayer));
    }

    IEnumerator Respawn(PlayerStats player)
    {
        yield return new WaitForSeconds(3f);
        player.Respawn();
    }

    void ActivateSuddenDeath()
    {
        Debug.Log("Sudden Death! One Hit KO activated.");
        player1.ActivateOneHitKO();
        player2.ActivateOneHitKO();
    }
}
