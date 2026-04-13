using UnityEngine;

public class BattleScene : MonoBehaviour
{
    [SerializeField] PhaseController _phaseController;
    [SerializeField] BattleSceneView _battleSceneView;
    [SerializeField] HomeCharacter _homeCharacter;

    void Start()
    {
        _phaseController.OnPhaseChanged += OnPhaseChanged;
        _battleSceneView.OnBattleStartButtonClicked += HandleBattleStartButtonClicked;
        _homeCharacter.OnHpChanged += HandleHomeCharacterHpChanged;
        _homeCharacter.OnMpChanged += HandleHomeCharacterMpChanged;
        _homeCharacter.OnDead += HandleHomeCharacterDead;

        _battleSceneView.Initialize();
        OnPhaseChanged(_phaseController.CurrentPhase);
        _battleSceneView.UpdateHomeCharacterHp(_homeCharacter.HP);
        _battleSceneView.UpdateHomeCharacterMp(_homeCharacter.MP);
    }

    void OnDestroy()
    {
        if (_phaseController != null)
            _phaseController.OnPhaseChanged -= OnPhaseChanged;
        if (_battleSceneView != null)
            _battleSceneView.OnBattleStartButtonClicked -= HandleBattleStartButtonClicked;
        if (_homeCharacter != null)
        {
            _homeCharacter.OnHpChanged -= HandleHomeCharacterHpChanged;
            _homeCharacter.OnMpChanged -= HandleHomeCharacterMpChanged;
            _homeCharacter.OnDead -= HandleHomeCharacterDead;
        }
    }

    /// <summary>집 캐릭터 체력 감소 함수</summary>
    /// <param name="damage">감소할 체력</param>
    public void DecreaseHomeCharacterHp(int damage)
    {
        _homeCharacter.TakeDamage(damage);
    }

    /// <summary>집 캐릭터 마나 감소 함수</summary>
    /// <param name="mp">감소할 마나</param>
    public void DecreaseHomeCharacterMp(int mp)
    {
        _homeCharacter.UseMp(mp);
    }

    /// <summary>페이즈 변경 함수</summary>
    /// <param name="phase">변경할 페이즈</param>
    void OnPhaseChanged(BattlePhase phase)
    {
        switch (phase)
        {
            case BattlePhase.Preparation:
                _battleSceneView.ShowPreparationPanel();
                break;
            case BattlePhase.Combat:
                _battleSceneView.ShowCombatPanel();
                break;
            case BattlePhase.End:
                _battleSceneView.ShowEndPanel();
                break;
        }
    }

    /// <summary>전투 시작 버튼 클릭 함수</summary>
    void HandleBattleStartButtonClicked()
    {
        _phaseController.ChangePhase(BattlePhase.Combat);
    }

    /// <summary>집 캐릭터 체력 변경 함수</summary>
    void HandleHomeCharacterHpChanged()
    {
        _battleSceneView.UpdateHomeCharacterHp(_homeCharacter.HP);
    }

    /// <summary>집 캐릭터 마나 변경 함수</summary>
    void HandleHomeCharacterMpChanged()
    {
        _battleSceneView.UpdateHomeCharacterMp(_homeCharacter.MP);
    }

    /// <summary>집 캐릭터 사망 함수</summary>
    void HandleHomeCharacterDead()
    {
        _phaseController.ChangePhase(BattlePhase.End);
    }
}