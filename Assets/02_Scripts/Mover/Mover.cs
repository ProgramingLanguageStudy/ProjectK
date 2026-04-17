using UnityEngine;
using Sirenix.OdinInspector;

/// <summary>
/// 이동을 담당하는 클래스의 추상 클래스
/// </summary>
[DisallowMultipleComponent]
public abstract class Mover : MonoBehaviour
{
    [Title("읽기전용(이동 속도)")]
    [SerializeField, ReadOnly] protected float _moveSpeed;  // 이동 속도

    public float MoveSpeed => _moveSpeed;  // 이동 속도 프로퍼티

    /// <summary>
    /// 이동 속도를 설정하는 메서드
    /// </summary>
    /// <param name="moveSpeed">이동 속도</param>
    public void SetMoveSpeed(float moveSpeed)
    {
        _moveSpeed = moveSpeed;
    }

    /// <summary>
    /// 이동을 담당하는 메서드
    /// </summary>
    /// <param name="direction">이동 방향 (비정규화여도 됨)</param>
    public abstract void Move(Vector3 direction);
}