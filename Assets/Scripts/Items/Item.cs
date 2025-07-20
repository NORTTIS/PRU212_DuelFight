using UnityEngine;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class Item : MonoBehaviour
{
    public ItemEffect effect; // Gắn trong prefab
    private bool pickedUp = false;

    private void Start()
    {
        GetComponent<Collider2D>().isTrigger = true;
        Destroy(gameObject, 10f); // Tự hủy sau 10s
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (pickedUp) return;
        if (!other.CompareTag("Player")) return;

        var player = other.GetComponent<PlayerStats>();
        if (player == null) return;

        pickedUp = true;

        if (effect == null)
        {
            Debug.LogWarning($"Item {name} missing ItemEffect reference!");
            Destroy(gameObject);
            return;
        }

        if (effect.isRandomBox)
        {
            Debug.Log($"{name} is a random box: {effect.randomEffectType}");
            ApplyRandomEffect(player, effect.randomEffectType);
        }
        else
        {
            Debug.Log($"{name} is a fixed item: {effect.itemType}");
            ApplyFixedEffect(player);
        }

        Destroy(gameObject);
    }

    void ApplyFixedEffect(PlayerStats player)
    {
        switch (effect.itemType)
        {
            case Enums.ItemType.Heal:
                player.Heal(effect.amount);
                break;

            case Enums.ItemType.ManaGain:
                player.AddMana(effect.amount);
                break;

            case Enums.ItemType.DamageBuff:
                if (effect.buffEffect != null)
                    player.ApplyBuff(effect.buffEffect.type, effect.buffEffect.value, effect.buffEffect.duration);
                break;
        }

        Debug.Log($"{player.playerName} used fixed item: {effect.effectName}");
    }

    void ApplyRandomEffect(PlayerStats player, Enums.RandomEffect effectType)
    {
        switch (effectType)
        {
            case Enums.RandomEffect.Heal:
                player.Heal(effect.amount);
                break;

            case Enums.RandomEffect.DamageBuff:
            case Enums.RandomEffect.SpeedBuff:
            case Enums.RandomEffect.SpeedPenalty:
                if (effect.buffEffect != null)
                    player.ApplyBuff(effect.buffEffect.type, effect.buffEffect.value, effect.buffEffect.duration);
                break;

            case Enums.RandomEffect.DamagePenalty:
                player.TakeDamage(effect.amount, "Penalty");
                break;

            case Enums.RandomEffect.Confusion:
                player.SetConfuse(effect.buffEffect.duration);
                break;
        }

        Debug.Log($"{player.playerName} received random effect: {effectType} from {name}");
    }
}
