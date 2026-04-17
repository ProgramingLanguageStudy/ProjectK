using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// 스폰 셀과 (나중에 채울) 루트 목록을 담는 SO. 보드 에디터에서 스포너 칸마다 하나 생성됩니다.
/// </summary>
[CreateAssetMenu(fileName = "EnemySpawnerData", menuName = "KSS/Enemy Spawner Data", order = 1)]
public class EnemySpawnerData : ScriptableObject
{
    [FormerlySerializedAs("_mapData")]
    [SerializeField] private BoardData _boardData;
    [SerializeField] private Vector2Int _spawnCell;
    [SerializeField] private List<EnemyRouteData> _routes = new List<EnemyRouteData>();

    public BoardData BoardData => _boardData;
    public Vector2Int SpawnCell => _spawnCell;
    public IReadOnlyList<EnemyRouteData> Routes => _routes;
}
