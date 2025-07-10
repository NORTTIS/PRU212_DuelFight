using UnityEngine;

public class Item : MonoBehaviour
{
    public ItemEffect effect;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        var player = other.GetComponent<PlayerStats>();
        if (player == null) return;

        if (effect.isRandomBox)
        {
            ApplyRandomEffect(player, effect.randomEffectType);
        }
        else
        {
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

        Debug.Log($"{player.playerName} used item: {effect.effectName}");
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

            case Enums.RandomEffect.Confusion:
                player.SetConfuse(effect.buffEffect.duration);
                break;
        }

        Debug.Log($"{player.playerName} got random effect: {effectType}");
    }
}
