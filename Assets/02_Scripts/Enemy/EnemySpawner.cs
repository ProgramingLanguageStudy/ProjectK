using UnityEngine;

/// <summary>
/// 전투 시작 시(선택) 한 마리 스폰하거나, 코드/인스펙터에서 테스트 스폰합니다.
/// </summary>
public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject _enemyPrefab;
    [SerializeField] private EnemyRoute _route;
    [SerializeField] private HomeCharacter _homeCharacter;
    [SerializeField] private PhaseController _phaseController;

    [SerializeField] private int _damageOnReach = 10;
    [SerializeField] private float _moveSpeed = 4f;
    [SerializeField] private bool _spawnOnceOnCombatStart = true;

    bool _spawnedForThisSession;

    void OnEnable()
    {
        if (_phaseController != null)
            _phaseController.OnPhaseChanged += OnPhaseChanged;
    }

    void OnDisable()
    {
        if (_phaseController != null)
            _phaseController.OnPhaseChanged -= OnPhaseChanged;
    }

    void OnPhaseChanged(BattlePhase phase)
    {
        if (phase != BattlePhase.Combat || !_spawnOnceOnCombatStart)
            return;
        if (_spawnedForThisSession)
            return;
        SpawnOne();
        _spawnedForThisSession = true;
    }

    /// <summary>테스트용: 적 한 마리 스폰.</summary>
    [ContextMenu("Debug/Spawn One Enemy")]
    public void SpawnOne()
    {
        if (_enemyPrefab == null || _route == null || _homeCharacter == null)
        {
            Debug.LogWarning("[EnemySpawner] Prefab, Route, HomeCharacter를 모두 지정하세요.", this);
            return;
        }

        var path = _route.PathCells;
        if (path == null || path.Count == 0)
        {
            Debug.LogWarning("[EnemyRoute] 경로(_pathCells)에 셀이 없습니다.", _route);
            return;
        }

        var instance = Instantiate(_enemyPrefab, transform.position, Quaternion.identity);
        var enemy = instance.GetComponent<Enemy>();
        if (enemy == null)
        {
            Debug.LogWarning("[EnemySpawner] 프리팹에 Enemy 컴포넌트가 필요합니다.", this);
            Destroy(instance);
            return;
        }

        enemy.Initialize(_route.Grid, path, _homeCharacter, _damageOnReach, _moveSpeed);
    }
}
