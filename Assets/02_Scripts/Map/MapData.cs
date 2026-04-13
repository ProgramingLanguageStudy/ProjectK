using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 맵 한 판의 정적 데이터. 그리드 크기·원점·홈·장애물·스포너 SO 참조.
/// </summary>
[CreateAssetMenu(fileName = "MapData", menuName = "KSS/Map Data", order = 0)]
public class MapData : ScriptableObject
{
    [SerializeField] private int _columns = 8;
    [SerializeField] private int _rows = 8;
    [SerializeField] private float _cellSize = 1f;
    [SerializeField] private Vector3 _gridOriginWorld;

    [SerializeField] private bool _hasHome;
    [SerializeField] private Vector2Int _homeCell;

    [SerializeField] private List<Vector2Int> _obstacleCells = new List<Vector2Int>();
    [SerializeField] private List<EnemySpawnerData> _spawners = new List<EnemySpawnerData>();

    public int Columns => _columns;
    public int Rows => _rows;
    public float CellSize => _cellSize;
    public Vector3 GridOriginWorld => _gridOriginWorld;
    public bool HasHome => _hasHome;
    public Vector2Int HomeCell => _homeCell;
    public IReadOnlyList<Vector2Int> ObstacleCells => _obstacleCells;
    public IReadOnlyList<EnemySpawnerData> Spawners => _spawners;
}
