using UnityEditor;
using UnityEngine;

/// <summary>
/// 에디터 전용 창. 플레이 모드에서 HomeCharacter에 테스트 데미지를 줍니다.
/// 메뉴: Tools / KSS / Home Character Debug
/// </summary>
public sealed class HomeCharacterDebugWindow : EditorWindow
{
    private const string MenuPath = "Tools/KSS/Home Character Debug";

    private HomeCharacter _homeCharacter;
    private int _damageAmount = 10;
    private Vector2 _scroll;

    [MenuItem(MenuPath)]
    static void Open()
    {
        var window = GetWindow<HomeCharacterDebugWindow>("Home Character Debug");
        window.minSize = new Vector2(320f, 140f);
    }

    void OnGUI()
    {
        _scroll = EditorGUILayout.BeginScrollView(_scroll);

        EditorGUILayout.LabelField("Home Character", EditorStyles.boldLabel);
        _homeCharacter = (HomeCharacter)EditorGUILayout.ObjectField(
            "대상",
            _homeCharacter,
            typeof(HomeCharacter),
            true);

        using (new EditorGUILayout.HorizontalScope())
        {
            if (GUILayout.Button("씬에서 찾기", GUILayout.Width(100f)))
                FindHomeCharacterInScene();
        }

        EditorGUILayout.Space(6f);
        _damageAmount = EditorGUILayout.IntField("데미지", _damageAmount);
        if (_damageAmount < 0)
            _damageAmount = 0;

        EditorGUILayout.Space(8f);
        EditorGUI.BeginDisabledGroup(!CanApplyDamage());
        if (GUILayout.Button("데미지 적용", GUILayout.Height(32f)))
            ApplyDamage();
        EditorGUI.EndDisabledGroup();

        if (!Application.isPlaying)
            EditorGUILayout.HelpBox("플레이 모드에서만 데미지를 적용할 수 있습니다.", MessageType.Info);
        else if (_homeCharacter == null)
            EditorGUILayout.HelpBox("Home Character를 지정하거나 「씬에서 찾기」를 누르세요.", MessageType.Warning);

        EditorGUILayout.EndScrollView();
    }

    bool CanApplyDamage()
    {
        return Application.isPlaying && _homeCharacter != null;
    }

    void FindHomeCharacterInScene()
    {
        _homeCharacter = Object.FindFirstObjectByType<HomeCharacter>();
        if (_homeCharacter == null)
            Debug.LogWarning("[HomeCharacterDebugWindow] 씬에 HomeCharacter가 없습니다.");
        else
            EditorGUIUtility.PingObject(_homeCharacter);
    }

    void ApplyDamage()
    {
        if (!CanApplyDamage())
            return;
        _homeCharacter.TakeDamage(_damageAmount);
    }
}
