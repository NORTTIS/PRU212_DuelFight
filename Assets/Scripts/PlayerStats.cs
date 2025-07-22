using System.Collections;
using UnityEngine;
using static Enums;

public class PlayerStats : MonoBehaviour
{
    [SerializeField] Transform spawnPoint;
    public string playerName = "Player";

    [Header("Stats")]
    public int maxHP = 100;
    public int currentHP;
    public int mana = 0;
    public int maxMana = 40;
    public int score = 0;
    public int baseAttack = 10;
    public int currentAttack;
    public float baseSpeed = 1.0f;
    public float currentSpeed = 1.0f;


    [Header("Status Flags")]
    public bool isConfused = false;
    public bool isDead = false;
    public bool isBlocking = false;

    void Start()
    {
        currentHP = maxHP;
        currentAttack = baseAttack;
    }

    public void Heal(int amount)
    {
        currentHP = Mathf.Min(maxHP, currentHP + amount);
        Debug.Log($"{playerName} healed +{amount} → {currentHP}/{maxHP}");

        // Update UI
        if (UIManager.Instance != null)
        {
            bool isPlayer1 = playerName == "Player 1";
            UIManager.Instance.UpdatePlayerHealth(isPlayer1, currentHP, maxHP);
        }
    }

    public void TakeDamage(int amount, string source = "Unknown")
    {
        if (isBlocking)
        {
            Debug.Log($"{playerName} blocked damage from {source}!");
            return;
        }
        if (currentHP > 0)
        {
            currentHP -= amount;
            Debug.Log($"{playerName} took {amount} damage from {source}. HP: {currentHP}");
        }

        Debug.Log($"{playerName} took {amount} damage from {source}. HP: {currentHP}, blocking {isBlocking}");

        // Update UI
        if (UIManager.Instance != null)
        {
            bool isPlayer1 = playerName == "Player 1";
            UIManager.Instance.UpdatePlayerHealth(isPlayer1, currentHP, maxHP);
        }

        if (currentHP <= 0)
        {
            FindFirstObjectByType<GameManager>()?.HandleDeath(this);
        }
    }

    public void AddMana(int amount)
    {
        mana = Mathf.Min(40, mana + amount);
        Debug.Log($"{playerName} gained {amount} mana → {mana}/40");

        // Update UI
        if (UIManager.Instance != null)
        {
            bool isPlayer1 = playerName == "Player 1";
            UIManager.Instance.UpdatePlayerMana(isPlayer1, mana, 40);
        }
    }

    public void ApplyBuff(PlayerBuffType type, float value, float duration)
    {
        StartCoroutine(BuffCoroutine(type, value, duration));
    }

    public void ApplyRandomEffect()
    {
        int roll = Random.Range(0, 6);
        RandomEffect effect = (RandomEffect)roll;
        Debug.Log($"{playerName} triggered {effect}");

        switch (effect)
        {
            case RandomEffect.Heal:
                Heal(20);
                break;
            case RandomEffect.DamageBuff:
                ApplyBuff(PlayerBuffType.Damage, 10, 5f);
                break;
            case RandomEffect.SpeedBuff:
                ApplyBuff(PlayerBuffType.Speed, 2f, 5f);
                break;
            case RandomEffect.DamagePenalty:
                TakeDamage(20, "Penalty");
                break;
            case RandomEffect.SpeedPenalty:
                Debug.Log($"{playerName}'s speed debuffed");
                break;
            case RandomEffect.Confusion:
                isConfused = true;
                Debug.Log($"{playerName} is confused for 5s");
                Invoke(nameof(ClearConfuse), 5f);
                break;
        }
    }

    void ClearConfuse()
    {
        isConfused = false;
        Debug.Log($"{playerName} is no longer confused");
    }

    IEnumerator BuffCoroutine(PlayerBuffType type, float value, float duration)
    {
        bool isPlayer1 = playerName == "Player 1";
        float timeLeft = duration;
        if (type == PlayerBuffType.Damage)
        {
            currentAttack += Mathf.RoundToInt(value);
            while (timeLeft > 0f)
            {
                UIManager.Instance?.SetBuffUI(isPlayer1, "ATK", true, timeLeft);
                yield return new WaitForSeconds(0.1f);
                timeLeft -= 0.1f;
            }
        }
        else if (type == PlayerBuffType.Speed)
        {
            currentSpeed *= value;
            while (timeLeft > 0f)
            {
                UIManager.Instance?.SetBuffUI(isPlayer1, "SPD", true, timeLeft);
                yield return new WaitForSeconds(0.1f);
                timeLeft -= 0.1f;
            }
        }

        if (type == PlayerBuffType.Damage)
        {
            currentAttack = baseAttack;
            UIManager.Instance?.SetBuffUI(isPlayer1, "ATK", false, 0f);
        }
        else if (type == PlayerBuffType.Speed)
        {
            currentSpeed = baseSpeed;
            UIManager.Instance?.SetBuffUI(isPlayer1, "SPD", false, 0f);
        }
    }

    public void ResetStats()
    {
        currentHP = maxHP;
        mana = 10;
        isConfused = false;
        isDead = false;
        currentAttack = baseAttack;
        transform.position = Vector3.zero;
    }

    public void Respawn()
    {
        currentHP = maxHP;
        // mana = 0;
        isConfused = false;
        isDead = false;
        //oneHitKO = false;
        currentAttack = baseAttack;
        transform.position = spawnPoint.position;
        Debug.Log($"{playerName} respawned");
    }

    public void SetConfuse(float duration)
    {
        StartCoroutine(ConfuseCoroutine(duration));
    }

    private IEnumerator ConfuseCoroutine(float duration)
    {
        bool isPlayer1 = playerName == "Player 1";
        float timeLeft = duration;
        isConfused = true;
        while (timeLeft > 0f)
        {
            UIManager.Instance?.SetBuffUI(isPlayer1, "CONF", true, timeLeft);
            yield return new WaitForSeconds(0.1f);
            timeLeft -= 0.1f;
        }
        isConfused = false;
        UIManager.Instance?.SetBuffUI(isPlayer1, "CONF", false, 0f);
    }

    public bool requestTrackingBullet = false;

    public void UseSkill(Enums.SkillType skill)
    {
        switch (skill)
        {
            case Enums.SkillType.Block:
                if (mana >= 2)
                {
                    mana -= 2;
                    StartCoroutine(BlockCoroutine());
                }
                else
                {
                    Debug.Log($"{playerName} not enough mana for Block");
                }
                break;
            case Enums.SkillType.Heal:
                if (mana >= 2)
                {
                    mana -= 2;
                    Heal(10);
                    if (healEffect != null)
                    {
                        healEffect.SetActive(true);
                        var ps = healEffect.GetComponent<ParticleSystem>();
                        if (ps != null) ps.Play();
                        // Tắt hiệu ứng sau 1.5s
                        StartCoroutine(DisableHealEffectAfterDelay(1.5f));
                    }
                }
                else
                {
                    Debug.Log($"{playerName} not enough mana for Heal");
                }
                break;
            case Enums.SkillType.TrackingBullet:
                if (mana >= 4)
                {
                    mana -= 4;
                    // Gọi trực tiếp FireTrackingBullet ở đây
                    GetComponent<PlayerController>().FireTrackingBullet();
                    // Không cần dùng requestTrackingBullet nữa
                }
                else
                {
                    Debug.Log($"{playerName} not enough mana for Tracking Bullet");
                }
                break;
        }
    }

    public GameObject shieldCircle; // Drag ShieldCircle here in Inspector
    public GameObject healEffect; // Drag HealEffect here in Inspector

    private IEnumerator BlockCoroutine()
    {
        bool isPlayer1 = playerName == "Player 1";
        float timeLeft = 3f;
        isBlocking = true;
        if (shieldCircle != null) shieldCircle.SetActive(true);
        while (timeLeft > 0f)
        {
            UIManager.Instance?.SetBuffUI(isPlayer1, "SHD", true, timeLeft);
            yield return new WaitForSeconds(0.1f);
            timeLeft -= 0.1f;
        }
        isBlocking = false;
        if (shieldCircle != null) shieldCircle.SetActive(false);
        UIManager.Instance?.SetBuffUI(isPlayer1, "SHD", false, 0f);
    }

    private IEnumerator DisableHealEffectAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (healEffect != null)
            healEffect.SetActive(false);
    }

    void Update()
    {
        if (playerName == "Player 1")
        {
            if (Input.GetKeyDown(KeyCode.Alpha1)) Heal(20);
            if (Input.GetKeyDown(KeyCode.Alpha2)) ApplyBuff(PlayerBuffType.Damage, 5, 5f);
            if (Input.GetKeyDown(KeyCode.Alpha3)) AddMana(2);
            if (Input.GetKeyDown(KeyCode.Alpha4)) ApplyRandomEffect();
        }
        else if (playerName == "Player 2")
        {
            if (Input.GetKeyDown(KeyCode.G)) Heal(20);
            if (Input.GetKeyDown(KeyCode.H)) ApplyBuff(PlayerBuffType.Damage, 5, 5f);
            if (Input.GetKeyDown(KeyCode.J)) AddMana(2);
            if (Input.GetKeyDown(KeyCode.K)) ApplyRandomEffect();
        }

    }

}