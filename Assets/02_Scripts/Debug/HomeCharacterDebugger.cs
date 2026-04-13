using UnityEngine;

/// <summary>
/// (선택) 런타임에서 다른 스크립트가 HomeCharacter에 테스트 데미지를 줄 때 사용.
/// 에디터 전용 UI는 Tools / KSS / Home Character Debug 창을 사용하세요.
/// </summary>
public class HomeCharacterDebugger : MonoBehaviour
{
    [SerializeField] private HomeCharacter _homeCharacter;
    [SerializeField] private int _damageAmount = 10;

    /// <summary>설정된 양만큼 체력 감소.</summary>
    public void ApplyDamage()
    {
        if (_homeCharacter == null)
        {
            Debug.LogWarning("[HomeCharacterDebugger] HomeCharacter가 지정되지 않았습니다.", this);
            return;
        }

        _homeCharacter.TakeDamage(_damageAmount);
    }
}
