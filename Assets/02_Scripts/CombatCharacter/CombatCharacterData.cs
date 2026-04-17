using UnityEngine;
using Sirenix.OdinInspector;

/// <summary>
/// 전투 캐릭터 데이터 추상 클래스
/// </summary>
public abstract class CombatCharacterData : ScriptableObject
{
    [Title("캐릭터 고유 ID")]
    public string Id;
    [Title("캐릭터 이름")]
    public string Name;
    [Title("캐릭터 설명")]
    [TextArea(3, 10)] public string Description;

    [Title("캐릭터 공격력")]
    public int Damage;
    [Title("캐릭터 체력")]
    public int Health;
    [Title("캐릭터 방어력")]
    public int Armor;
    [Title("캐릭터 마법 저항력")]
    public int MagicResistance;
    [Title("캐릭터 공격 속도")]
    public float AttackSpeed;
}