using TMPro;
using UnityEngine;

public class StatsUIUpdater : MonoBehaviour
{
    public PlayerStats player1;
    public PlayerStats player2;

    public TextMeshProUGUI player1Text;
    public TextMeshProUGUI player2Text;

    void Update()
    {
        player1Text.text = GetStatsText(player1);
        player2Text.text = GetStatsText(player2);
    }

    string GetStatsText(PlayerStats p)
    {
        return $"{p.playerName}\n" +
               $"HP: {p.currentHP}/{p.maxHP}\n" +
               $"Mana: {p.mana}/10\n" +
               $"ATK: {p.currentAttack}\n" +
               $"Speed: {p.currentSpeed}\n" +
               $"Score: {p.score}\n" +
               $"Confused: {p.isConfused}\n" +
               $"OneHitKO: {p.oneHitKO}";
    }

}
