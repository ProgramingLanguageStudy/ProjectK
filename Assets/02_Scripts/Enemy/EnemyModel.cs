using UnityEngine;

/// <summary>
/// Enemy의 비즈니스 로직을 담당하는 클래스
/// </summary>
[DisallowMultipleComponent]
public class EnemyModel : MonoBehaviour
{
    [SerializeField] private EnemyData _enemyData;

    public int DamageToHome => _enemyData.DamageToHome;
}