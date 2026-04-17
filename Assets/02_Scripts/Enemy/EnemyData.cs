using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "New Enemy Data", menuName = "Data/Enemy Data", order = 1)]
public class EnemyData : CombatCharacterData
{
    [Title("이동 속도")]
    public float MoveSpeed;

    [Title("기지 피해량")]
    public int DamageToHome;
}