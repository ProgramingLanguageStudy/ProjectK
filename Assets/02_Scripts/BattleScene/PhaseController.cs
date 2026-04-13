using UnityEngine;
using System;

// 페이즈를 3단계로 나누는 enum
public enum BattlePhase
{
    Preparation,
    Combat,
    End,
}

/// <summary>
/// 게임 페이즈 관리. Preparation, Combat, End
/// </summary>
public class PhaseController : MonoBehaviour
{
    // 현재 페이즈
    [SerializeField] BattlePhase _currentPhase;

    /// <summary>현재 페이즈 프로퍼티</summary>
    public BattlePhase CurrentPhase => _currentPhase;
    
    /// <summary>페이즈 변경 이벤트</summary>
    public event Action<BattlePhase> OnPhaseChanged;

    /// <summary>페이즈 변경 함수</summary>
    public void ChangePhase(BattlePhase newPhase)
    {
        // 현재 페이즈와 새로운 페이즈가 같으면 반환
        if (_currentPhase == newPhase) return;
        // 현재 페이즈를 새로운 페이즈로 변경
        _currentPhase = newPhase;
        // 페이즈 변경 이벤트 발생
        OnPhaseChanged?.Invoke(newPhase);
    }
}