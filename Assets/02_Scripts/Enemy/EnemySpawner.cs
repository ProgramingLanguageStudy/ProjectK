using UnityEngine;
using System.Collections.Generic;
using Sirenix.OdinInspector;

/// <summary>
/// 적을 스폰하는 클래스
/// </summary>
public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private Enemy _enemyPrefab;

    private Vector3 _spawnPosition;
    private BattleGrid _grid;
    private List<Vector2Int> _path = new List<Vector2Int>();
    private HomeCharacter _homeCharacter;

    private void Awake()
    {
        
    }

    public void Initialize(BattleGrid grid, List<Vector2Int> path, HomeCharacter homeCharacter)
    {
        _grid = grid;
        _path = path;
        _homeCharacter = homeCharacter;

        _spawnPosition = _grid.CellToWorldCenter(_path[0]);
    }
    
    [Button("Spawn Enemy")]
    private void SpawnEnemy()
    {
        Instantiate(_enemyPrefab, _spawnPosition, Quaternion.identity);
        Enemy enemy = _enemyPrefab.GetComponent<Enemy>();
        if (enemy == null)
        {
            Debug.LogError("Enemy prefab is missing Enemy component");
            return;
        }
        enemy.Initialize(_grid, _path, _homeCharacter);
    }
}
