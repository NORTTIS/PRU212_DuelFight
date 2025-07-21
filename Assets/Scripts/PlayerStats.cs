using System.Collections;
using UnityEngine;
using static Enums;

public class PlayerStats : MonoBehaviour
{
    public string playerName = "Player";

    [Header("Stats")]
    public int maxHP = 100;
    public int currentHP;
    public int mana = 0;
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

        if (currentHP <= 0)
        {
            FindFirstObjectByType<GameManager>()?.HandleDeath(this);
        }
    }

    public void AddMana(int amount)
    {
        mana = Mathf.Min(40, mana + amount);
        Debug.Log($"{playerName} gained {amount} mana → {mana}/40");
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
        if (type == PlayerBuffType.Damage)
        {
            currentAttack += Mathf.RoundToInt(value);
            Debug.Log($"{playerName} gained +{value} ATK → {currentAttack} (duration: {duration}s)");
        }
        else if (type == PlayerBuffType.Speed)
        {
            currentSpeed *= value;
            Debug.Log($"{playerName} speed x{value} → {currentSpeed} (duration: {duration}s)");
        }

        yield return new WaitForSeconds(duration);

        if (type == PlayerBuffType.Damage)
        {
            currentAttack = baseAttack;
            Debug.Log($"{playerName}'s ATK reset → {currentAttack}");
        }
        else if (type == PlayerBuffType.Speed)
        {
            currentSpeed = baseSpeed;
            Debug.Log($"{playerName}'s speed reset → {currentSpeed}");
        }
    }

    public void ResetStats()
    {
        currentHP = maxHP;
        mana = 0;
        isConfused = false;
        isDead = false;
        currentAttack = baseAttack;
        transform.position = Vector3.zero;
    }

    public void Respawn()
    {
        currentHP = maxHP;
        mana = 0;
        isConfused = false;
        isDead = false;
        //oneHitKO = false;
        currentAttack = baseAttack;
        transform.position = Vector3.zero;
        Debug.Log($"{playerName} respawned");
    }

    public void SetConfuse(float duration)
    {
        StartCoroutine(ConfuseCoroutine(duration));
    }

    private IEnumerator ConfuseCoroutine(float duration)
    {
        isConfused = true;
        Debug.Log($"{playerName} is confused for {duration} seconds.");
        yield return new WaitForSeconds(duration);
        isConfused = false;
        Debug.Log($"{playerName} is no longer confused.");
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
                    requestTrackingBullet = true; // Đánh dấu cần bắn đạn
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
        isBlocking = true;
        if (shieldCircle != null) shieldCircle.SetActive(true);
        Debug.Log($"{playerName} is blocking for 3 seconds");
        yield return new WaitForSeconds(3f);
        isBlocking = false;
        if (shieldCircle != null) shieldCircle.SetActive(false);
        Debug.Log($"{playerName} block ended");
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