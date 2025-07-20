using System.Collections;
using UnityEngine;
using static Enums;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    //[SerializeField] private GameObject player1Prefab;
    //[SerializeField] private GameObject player2Prefab;

    public PlayerStats player1;
    public PlayerStats player2;

    public GameMode gameMode = GameMode.Bo3;

    public float matchTime = 30f;
    private float defaultMatchTime;
    public GameObject trackingProjectilePrefab;
    // [SerializeField] private Transform player1SpawnPoint;
    // [SerializeField] private Transform player2SpawnPoint;

    private bool isGameEnded = false;
    private bool overtimeDeathCheck = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        defaultMatchTime = matchTime;
        //player1 = player1Prefab.GetComponent<PlayerStats>();
        //player2 = player2Prefab.GetComponent<PlayerStats>();
    }
    void Start()
    {
        player1.playerName = "Player 1";
        player2.playerName = "Player 2";
    }

    void Update()
    {
        if (isGameEnded) return;

        if (matchTime > 0f)
        {
            matchTime -= Time.deltaTime;

            if (matchTime <= 0f)
            {
                matchTime = 0f;

                if (gameMode == GameMode.Bo3)
                {
                    if (player1.currentHP > player2.currentHP)
                    {
                        // Player 1 thắng round (không kết thúc luôn)
                        player1.score++;
                        Debug.Log($"[Time Up] {player1.playerName} has more HP → +1 point");
                        CheckBo3Victory();
                    }
                    else if (player2.currentHP > player1.currentHP)
                    {
                        player2.score++;
                        Debug.Log($"[Time Up] {player2.playerName} has more HP → +1 point");
                        CheckBo3Victory();
                    }
                    else
                    {
                        // Máu bằng nhau → overtime
                        overtimeDeathCheck = true;
                        Debug.Log("HP equal on time up → Overtime until 1 player dies (no game end).");
                    }
                }
                else if (gameMode == GameMode.VictoryByScore)
                {
                    if (player1.score > player2.score)
                        ProcessVictory(player1);
                    else if (player2.score > player1.score)
                        ProcessVictory(player2);
                    else
                    {
                        overtimeDeathCheck = true;
                        Debug.Log("Draw: Sudden Death until someone dies.");
                    }
                }
            }
        }
    }

    public void HandleDeath(PlayerStats deadPlayer, string cause = "unknown")
    {
        if (isGameEnded) return;
        if (!deadPlayer.isDead)
        {

            PlayerStats killer = deadPlayer == player1 ? player2 : player1;
            killer.score++;

            Debug.Log($"{deadPlayer.playerName} died due to {cause} — {killer.playerName} gains 1 score!");

            if (gameMode == GameMode.Bo3)
            {
                if (overtimeDeathCheck)
                {
                    overtimeDeathCheck = false;
                    Debug.Log("[Overtime Result] 1 point awarded, game continues.");
                    CheckBo3Victory();
                    StartCoroutine(ResetGame());
                    return;
                }

                CheckBo3Victory();
                StartCoroutine(ResetGame());
            }
            else if (gameMode == GameMode.VictoryByScore)
            {
                if (overtimeDeathCheck)
                {
                    overtimeDeathCheck = false;
                    ProcessVictory(killer);
                    return;
                }

                StartCoroutine(Respawn(deadPlayer));
            }
            deadPlayer.isDead = true;
        }
    }

    void CheckBo3Victory()
    {
        if (player1.score >= 2)
        {
            ProcessVictory(player1);
        }
        else if (player2.score >= 2)
        {
            ProcessVictory(player2);
        }
        ResetGame();
    }

    void ProcessVictory(PlayerStats winner)
    {
        if (isGameEnded) return;

        isGameEnded = true;
        Debug.Log($"[VICTORY] {winner.playerName} wins the match!");

        Time.timeScale = 0f; // Dừng game
    }

    IEnumerator Respawn(PlayerStats player)
    {
        yield return new WaitForSeconds(2f);
        player.Respawn();
    }

    IEnumerator ResetGame()
    {
        yield return new WaitForSeconds(1f);

        matchTime = defaultMatchTime;
        //suddenDeathActivated = false;

        player1.Respawn();
        player2.Respawn();

        Debug.Log("New round started.");
    }

    public void PlayerTakeDamageFromOther(bool isPlayer1)
    {
        PlayerStats targetPlayer = isPlayer1 ? player1 : player2;
        int damage = isPlayer1 ? player2.baseAttack : player1.baseAttack;

        if (!targetPlayer.isDead)
        {
            targetPlayer.currentHP -= damage;
            Debug.Log($"Player {(isPlayer1 ? "1" : "2")} took {damage} damage. Remaining health: {targetPlayer.currentHP}");
        }
    }
}
