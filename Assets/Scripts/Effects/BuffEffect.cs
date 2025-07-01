using UnityEngine;
using static Enums;

[CreateAssetMenu(fileName = "NewBuffEffect", menuName = "DuelFight/Effects/BuffEffect")]
public class BuffEffect : ScriptableObject
{
    public string effectName;
    public PlayerBuffType type;
    public float value = 1f;
    public float duration = 5f;
}
