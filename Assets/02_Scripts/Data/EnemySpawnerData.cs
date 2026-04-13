using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 스폰 셀과 (나중에 채울) 루트 목록을 담는 SO. 맵 에디터에서 스포너 칸마다 하나 생성됩니다.
/// </summary>
[CreateAssetMenu(fileName = "EnemySpawnerData", menuName = "KSS/Enemy Spawner Data", order = 1)]
public class EnemySpawnerData : ScriptableObject
{
    [SerializeField] private MapData _mapData;
    [SerializeField] private Vector2Int _spawnCell;
    [SerializeField] private List<EnemyRouteData> _routes = new List<EnemyRouteData>();

    public MapData MapData => _mapData;
    public Vector2Int SpawnCell => _spawnCell;
    public IReadOnlyList<EnemyRouteData> Routes => _routes;
}
