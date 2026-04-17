using UnityEngine;

/// <summary>
/// Transform을 사용하여 이동을 담당하는 클래스
/// </summary>
[DisallowMultipleComponent]
public class TransformMover : Mover
{
    public override void Move(Vector3 direction)
    {
        Vector3 dir = direction.normalized;  // 비정규화된 방향을 정규화
        transform.position += dir * _moveSpeed * Time.deltaTime;
    }
}