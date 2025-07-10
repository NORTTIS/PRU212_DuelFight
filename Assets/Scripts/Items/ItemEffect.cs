using UnityEngine;
using static Enums;

[CreateAssetMenu(fileName = "NewItemEffect", menuName = "DuelFight/Effects/ItemEffect")]
public class ItemEffect : ScriptableObject
{
    public string effectName;
    public ItemType itemType;
    public int amount; // dùng cho Heal, ManaGain
    public BuffEffect buffEffect;
    public bool isRandomBox = false;
    public RandomEffect randomEffectType;
}
