using UnityEngine;

public class UIUpdater : MonoBehaviour
{
    private PlayerStats player1Stats;
    private PlayerStats player2Stats;
    private GameManager gameManager;

    void Start()
    {
        gameManager = GameManager.Instance;
        if (gameManager != null)
        {
            player1Stats = gameManager.player1;
            player2Stats = gameManager.player2;
        }
    }

    void Update()
    {
        if (UIManager.Instance == null) return;

        // Update Player 1 stats
        if (player1Stats != null)
        {
            UIManager.Instance.UpdatePlayerHealth(true, player1Stats.currentHP, player1Stats.maxHP);
            UIManager.Instance.UpdatePlayerMana(true, player1Stats.mana, 10);
            UIManager.Instance.UpdatePlayerScore(true, player1Stats.score);
            UIManager.Instance.UpdatePlayerStats(true, player1Stats.currentAttack, player1Stats.currentSpeed);
        }

        // Update Player 2 stats
        if (player2Stats != null)
        {
            UIManager.Instance.UpdatePlayerHealth(false, player2Stats.currentHP, player2Stats.maxHP);
            UIManager.Instance.UpdatePlayerMana(false, player2Stats.mana, 10);
            UIManager.Instance.UpdatePlayerScore(false, player2Stats.score);
            UIManager.Instance.UpdatePlayerStats(false, player2Stats.currentAttack, player2Stats.currentSpeed);
        }
    }
} 