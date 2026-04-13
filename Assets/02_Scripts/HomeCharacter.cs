using System;
using UnityEngine;

/// <summary>집 캐릭터 클래스</summary>
/// <remarks>
/// 캐릭터의 체력과 마나를 관리하고, 사망 시 이벤트를 발생시킨다.
/// </remarks>
public class HomeCharacter : MonoBehaviour
{
    [SerializeField] int _hp;        // 체력
    [SerializeField] int _mp;        // 마나

    bool _isDead;

    public int HP => _hp;            // 체력 프로퍼티
    public int MP => _mp;            // 마나 프로퍼티

    public event Action OnHpChanged;    // 체력 변경 이벤트
    public event Action OnMpChanged;    // 마나 변경 이벤트
    public event Action OnDead;         // 사망 이벤트

    /// <summary>체력 감소 함수</summary>
    /// <param name="damage">감소할 체력</param>
    public void TakeDamage(int damage)
    {
        if (_isDead || damage <= 0)
            return;

        _hp -= damage;

        if (_hp <= 0)
        {
            _hp = 0;
            _isDead = true;
            Die();
        }
        OnHpChanged?.Invoke();
    }

    /// <summary>마나 사용 함수</summary>
    /// <param name="mp">사용할 마나</param>
    public void UseMp(int mp)
    {
        _mp -= mp;              // 마나 감소
        if (_mp < 0)
        {
            _mp = 0;            // 마나가 0 이하이면 0으로 설정
        }

        OnMpChanged?.Invoke(); // 마나 변경 이벤트 발생
    }

    /// <summary>사망 함수</summary>
    void Die()
    {
        OnDead?.Invoke(); // 사망 이벤트 발생
    }
}