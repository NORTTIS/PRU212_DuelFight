using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    [SerializeField] private GameObject player1Prefab;
    [SerializeField] private GameObject player2Prefab;
    // [SerializeField] private Transform player1SpawnPoint;
    // [SerializeField] private Transform player2SpawnPoint;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
    void Start()
    {
        player1.playerName = "Player 1";
        player2.playerName = "Player 2";
    }

    // Update is called once per frame
    void Update()
    {
        matchTime -= Time.deltaTime;
        if (matchTime <= 0f)
        {
            EvaluateWinner();
        }
    }

    public void PlayerTakeDamage(bool isPlayer1, int dmgType)
    {
        PlayerController targetPlayer = isPlayer1
            ? player1Prefab.GetComponent<PlayerController>()
            : player2Prefab.GetComponent<PlayerController>();

        float damage = 0f;

        switch (dmgType)
        {
            case damageType.BASIC:
                damage = isPlayer1
                    ? player2Prefab.GetComponent<PlayerController>().atk
                    : 10f;
                break;
            case damageType.ENV:
                damage = 5f;
                break;
            case damageType.ITEM:
                damage = 20f;
                break;
        }

        targetPlayer.maxHealth -= damage;
        Debug.Log($"Player {(isPlayer1 ? "1" : "2")} took {damage} damage. Remaining health: {targetPlayer.maxHealth}");
    }

    public class damageType
    {
        public const int BASIC = 0;
        public const int ENV = 1;
        public const int ITEM = 2;
    }
    public PlayerStats player1;
    public PlayerStats player2;
    public float matchTime = 180f;


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
