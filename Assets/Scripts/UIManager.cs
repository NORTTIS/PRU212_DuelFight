using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("Player 1 UI")]
    public Slider player1HealthBar;
    public Slider player1ManaBar;
    public TextMeshProUGUI player1ScoreText;
    public TextMeshProUGUI player1NameText;
    public TextMeshProUGUI player1AttackText;
    public TextMeshProUGUI player1SpeedText;

    [Header("Player 2 UI")]
    public Slider player2HealthBar;
    public Slider player2ManaBar;
    public TextMeshProUGUI player2ScoreText;
    public TextMeshProUGUI player2NameText;
    public TextMeshProUGUI player2AttackText;
    public TextMeshProUGUI player2SpeedText;

    [Header("Center UI")]
    public TextMeshProUGUI matchTimerText;
    public TextMeshProUGUI vsText;

    [Header("Game Over UI")]
    public GameObject gameOverPanel;
    public TextMeshProUGUI winnerText;
    public TextMeshProUGUI finalScoreText;
    public Button rematchButton;
    public Button mainMenuButton;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        InitializeUI();
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
    }

    void InitializeUI()
    {
        // Set up player names
        if (player1NameText != null) player1NameText.text = "Player 1";
        if (player2NameText != null) player2NameText.text = "Player 2";

        // Set up health bars
        if (player1HealthBar != null)
        {
            player1HealthBar.maxValue = 100;
            player1HealthBar.value = 100;
        }
        if (player2HealthBar != null)
        {
            player2HealthBar.maxValue = 100;
            player2HealthBar.value = 100;
        }

        // Set up mana bars
        if (player1ManaBar != null)
        {
            player1ManaBar.maxValue = 10;
            player1ManaBar.value = 0;
        }
        if (player2ManaBar != null)
        {
            player2ManaBar.maxValue = 10;
            player2ManaBar.value = 0;
        }

        // Set up scores
        if (player1ScoreText != null) player1ScoreText.text = "Score: 0";
        if (player2ScoreText != null) player2ScoreText.text = "Score: 0";

        // Set up VS text
        if (vsText != null) vsText.text = "VS";
    }

    public void UpdatePlayerHealth(bool isPlayer1, int currentHealth, int maxHealth)
    {
        if (isPlayer1 && player1HealthBar != null)
        {
            player1HealthBar.value = currentHealth;
        }
        else if (!isPlayer1 && player2HealthBar != null)
        {
            player2HealthBar.value = currentHealth;
        }
    }

    public void UpdatePlayerMana(bool isPlayer1, int currentMana, int maxMana)
    {
        if (isPlayer1 && player1ManaBar != null)
        {
            player1ManaBar.value = currentMana;
        }
        else if (!isPlayer1 && player2ManaBar != null)
        {
            player2ManaBar.value = currentMana;
        }
    }

    public void UpdatePlayerScore(bool isPlayer1, int score)
    {
        if (isPlayer1 && player1ScoreText != null)
        {
            player1ScoreText.text = $"{score}";
        }
        else if (!isPlayer1 && player2ScoreText != null)
        {
            player2ScoreText.text = $"{score}";
        }
    }

    public void UpdatePlayerStats(bool isPlayer1, int attack, float speed)
    {
        if (isPlayer1)
        {
            if (player1AttackText != null) player1AttackText.text = $"{attack}";
            if (player1SpeedText != null) player1SpeedText.text = $"{speed:F1}";
        }
        else
        {
            if (player2AttackText != null) player2AttackText.text = $"{attack}";
            if (player2SpeedText != null) player2SpeedText.text = $"{speed:F1}";
        }
    }

    public void UpdateMatchTimer(float timeLeft)
    {
        if (matchTimerText != null)
        {
            int minutes = Mathf.FloorToInt(timeLeft / 60);
            int seconds = Mathf.FloorToInt(timeLeft % 60);
            matchTimerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
    }

    public void ShowGameOver(string winnerName, int player1Score, int player2Score)
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
            if (winnerText != null) winnerText.text = $"{winnerName} Wins!";
            if (finalScoreText != null) finalScoreText.text = $"Final Score: {player1Score} - {player2Score}";
        }
    }
        
    public void HideGameOver()
    {
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
    }
} 