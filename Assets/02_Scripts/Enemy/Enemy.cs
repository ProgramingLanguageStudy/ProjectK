using System;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

/// <summary>
/// 격자 경로의 셀을 순서대로 이동하고, 마지막 셀에 도착하면 <see cref="ReachedLastWaypoint"/> 를 한 번 발행합니다.
/// </summary>
[DisallowMultipleComponent]
[RequireComponent(typeof(TransformMover)), RequireComponent(typeof(EnemyModel))]
public class Enemy : MonoBehaviour
{
    [SerializeField, ReadOnly] private TransformMover _mover;
    [SerializeField, ReadOnly] private EnemyModel _enemyModel;

    private BattleGrid _grid;
    private List<Vector2Int> _cellPath = new List<Vector2Int>();
    
    private int _currentIndex;
    private HomeCharacter _homeCharacter;
    private bool _pathEndHandled;

    public Mover Mover => _mover;

    void Awake()
    {
        _mover = GetComponent<TransformMover>();
        _enemyModel = GetComponent<EnemyModel>();
    }

    void Update()
    {
        if (_grid == null || _cellPath.Count < 2)
            return;

        AdvanceIfEnteredNextCell();

        if (_currentIndex >= _cellPath.Count - 1)
        {
            if (!_pathEndHandled)
            {
                _pathEndHandled = true;
                if (_homeCharacter != null)
                    _homeCharacter.TakeDamage(_enemyModel.DamageToHome);
            }
            return;
        }

        Vector3 direction = GetMoveDirection();
        if (direction.sqrMagnitude > 0.0001f)
            _mover.Move(direction);
    }

    /// <summary>
    /// 셀 경로를 등록하고 그리드 좌표계로 이동을 준비합니다.
    /// </summary>
    public void Initialize(BattleGrid grid, List<Vector2Int> cellPath, HomeCharacter homeCharacter)
    {
        _cellPath.Clear();
        _homeCharacter = homeCharacter;
        _currentIndex = 0;
        _pathEndHandled = false;
        _grid = null;

        if (grid == null || cellPath == null || cellPath.Count == 0)
            return;

        _grid = grid;
        _cellPath.AddRange(cellPath);
    }

    private void AdvanceIfEnteredNextCell()
    {
        Vector2Int currentCell = _grid.WorldToCell(transform.position);

        while (_currentIndex < _cellPath.Count - 1)
        {
            Vector2Int nextCell = _cellPath[_currentIndex + 1];
            if (currentCell != nextCell)
                break;
            _currentIndex++;
        }
    }

    private Vector3 GetMoveDirection()
    {
        Vector3 target = _grid.CellToWorldCenter(_cellPath[_currentIndex + 1]);
        Vector3 toTarget = target - transform.position;
        toTarget.y = 0f;

        if (toTarget.sqrMagnitude < 0.0001f)
            return Vector3.zero;

        return toTarget.normalized;
    }
}
