using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 전투 화면 뷰. Preparation, Combat, End 패널을 관리.
/// </summary>
public class BattleSceneView : MonoBehaviour
{
    [SerializeField] GameObject _preparationPanel;
    [SerializeField] GameObject _combatPanel;
    [SerializeField] GameObject _endPanel;

    [SerializeField] Button _battleStartButton;

    [SerializeField] TextMeshProUGUI _homeCharacterHpText;
    [SerializeField] TextMeshProUGUI _homeCharacterMpText;

    public event Action OnBattleStartButtonClicked;

    public void Initialize()
    {
        _battleStartButton.onClick.AddListener(HandleBattleStartButtonClicked);
    }

    /// <summary>Preparation 패널 표시
    /// Preparation 패널을 표시하고, Combat 와 End 패널을 숨김.
    /// </summary>
    public void ShowPreparationPanel()
    {
        _preparationPanel.SetActive(true);
        _combatPanel.SetActive(false);
        _endPanel.SetActive(false);
    }

    /// <summary>Combat 패널 표시
    /// Combat 패널을 표시하고, Preparation 와 End 패널을 숨김.
    /// </summary>
    public void ShowCombatPanel()
    {
        _combatPanel.SetActive(true);
        _preparationPanel.SetActive(false);
        _endPanel.SetActive(false);
    }

    /// <summary>End 패널 표시
    /// End 패널을 표시하고, Preparation 와 Combat 패널을 숨김.
    /// </summary>
    public void ShowEndPanel()
    {
        _endPanel.SetActive(true);
        _preparationPanel.SetActive(false);
        _combatPanel.SetActive(false);
    }

    /// <summary>집 캐릭터 체력 업데이트 함수</summary>
    /// <param name="hp">업데이트할 체력</param>
    public void UpdateHomeCharacterHp(int hp)
    {
        _homeCharacterHpText.text = hp.ToString();
    }

    /// <summary>집 캐릭터 마나 업데이트 함수</summary>
    /// <param name="mp">업데이트할 마나</param>
    public void UpdateHomeCharacterMp(int mp)
    {
        _homeCharacterMpText.text = mp.ToString();
    }

    void HandleBattleStartButtonClicked()
    {
        OnBattleStartButtonClicked?.Invoke();
    }
}