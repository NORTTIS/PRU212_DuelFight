using UnityEngine;

public class Enums : MonoBehaviour
{
    public enum PlayerBuffType
    {
        Damage,
        Speed
    }

    public enum ItemType
    {
        Heal,
        DamageBuff,
        RandomEffectBox,
        ManaGain
    }

    public enum RandomEffect
    {
        Heal,
        DamageBuff,
        SpeedBuff,
        DamagePenalty,
        SpeedPenalty,
        Confusion
    }
}
