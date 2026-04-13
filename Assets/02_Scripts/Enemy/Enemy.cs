using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 격자 경로의 셀 중심을 순서대로 이동하고, 마지막 셀에 도착하면 기지에 피해를 줍니다.
/// </summary>
[DisallowMultipleComponent]
public class Enemy : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 4f;

    private BattleGrid _grid;
    private IReadOnlyList<Vector2Int> _path;
    private HomeCharacter _home;
    private int _damageOnReach;
    private int _targetIndex;
    private Vector3 _currentWaypoint;
    private bool _initialized;

    /// <summary>경로: [0]=스폰 셀, [^1]=기지 셀.</summary>
    public void Initialize(
        BattleGrid grid,
        IReadOnlyList<Vector2Int> path,
        HomeCharacter home,
        int damageOnReach,
        float moveSpeed)
    {
        _grid = grid;
        _path = path;
        _home = home;
        _damageOnReach = damageOnReach;
        _moveSpeed = moveSpeed;

        if (grid == null || path == null || path.Count == 0)
        {
            Debug.LogWarning("[Enemy] 경로 또는 그리드가 비었습니다.", this);
            Destroy(gameObject);
            return;
        }

        transform.position = _grid.CellToWorldCenter(path[0]);

        if (path.Count == 1)
        {
            ReachBase();
            return;
        }

        _targetIndex = 1;
        _currentWaypoint = _grid.CellToWorldCenter(path[_targetIndex]);
        _initialized = true;
    }

    void Update()
    {
        if (!_initialized || _grid == null || _path == null)
            return;

        transform.position = Vector3.MoveTowards(
            transform.position,
            _currentWaypoint,
            _moveSpeed * Time.deltaTime);

        if ((transform.position - _currentWaypoint).sqrMagnitude > 0.0001f)
            return;

        if (_targetIndex >= _path.Count - 1)
        {
            ReachBase();
            return;
        }

        _targetIndex++;
        _currentWaypoint = _grid.CellToWorldCenter(_path[_targetIndex]);
    }

    void ReachBase()
    {
        _initialized = false;
        if (_home != null && _damageOnReach > 0)
            _home.TakeDamage(_damageOnReach);
        Destroy(gameObject);
    }
}
