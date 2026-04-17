using UnityEngine;

/// <summary>
/// Rigidbody를 사용하여 이동을 담당하는 클래스
/// </summary>
[DisallowMultipleComponent]
[RequireComponent(typeof(Rigidbody))]
public class RigidbodyMover : Mover
{
    private Rigidbody _rigidbody;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    public override void Move(Vector3 direction)
    {
        Vector3 dir = direction.normalized;  // 비정규화된 방향을 정규화
        _rigidbody.linearVelocity = dir * _moveSpeed;
    }
}