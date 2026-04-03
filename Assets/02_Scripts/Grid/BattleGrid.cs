using UnityEngine;

/// <summary>
/// XZ 평면 그리드. origin은 셀 (0,0)의 한 모서리(기본: 왼쪽 아래) 월드 좌표.
/// Plane 위에 빈 오브젝트를 두고, 그 모서리에 맞춰 위치시키면 됩니다.
/// </summary>
public class BattleGrid : MonoBehaviour
{
    [SerializeField] private int _columns = 8;
    [SerializeField] private int _rows = 8;
    [SerializeField] private float _cellSize = 1f;

    [Tooltip("셀 (0,0)의 바닥 모서리 월드 위치. 비우면 이 오브젝트의 position 사용.")]
    [SerializeField] private Transform _originOverride;

    [Header("Gizmos")]
    [SerializeField] private bool _drawGridGizmos = true;
    [SerializeField] private Color _gridColor = new Color(0.2f, 0.8f, 1f, 0.35f);

    public int Columns => _columns;
    public int Rows => _rows;
    public float CellSize => _cellSize;

    public Vector3 Origin => _originOverride != null ? _originOverride.position : transform.position;

    public Vector2Int WorldToCell(Vector3 worldPos)
    {
        Vector3 o = Origin;
        float lx = worldPos.x - o.x;
        float lz = worldPos.z - o.z;
        return new Vector2Int(Mathf.FloorToInt(lx / _cellSize), Mathf.FloorToInt(lz / _cellSize));
    }

    /// <summary>셀 중심 (XZ), Y는 origin과 동일.</summary>
    public Vector3 CellToWorldCenter(Vector2Int cell)
    {
        Vector3 o = Origin;
        return new Vector3(
            o.x + (cell.x + 0.5f) * _cellSize,
            o.y,
            o.z + (cell.y + 0.5f) * _cellSize);
    }

    public bool ContainsCell(Vector2Int cell)
    {
        return cell.x >= 0 && cell.x < _columns && cell.y >= 0 && cell.y < _rows;
    }

    public Vector2Int ClampToGrid(Vector2Int cell)
    {
        return new Vector2Int(
            Mathf.Clamp(cell.x, 0, _columns - 1),
            Mathf.Clamp(cell.y, 0, _rows - 1));
    }

    /// <summary>월드 위치를 그리드 안으로 스냅(셀 중심).</summary>
    public Vector3 SnapWorldToCellCenter(Vector3 worldPos)
    {
        Vector2Int c = WorldToCell(worldPos);
        if (!ContainsCell(c))
            c = ClampToGrid(c);
        return CellToWorldCenter(c);
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        if (!_drawGridGizmos)
            return;

        Vector3 o = Origin;
        Gizmos.color = _gridColor;

        for (int x = 0; x < _columns; x++)
        {
            for (int z = 0; z < _rows; z++)
            {
                Vector3 center = new Vector3(
                    o.x + (x + 0.5f) * _cellSize,
                    o.y,
                    o.z + (z + 0.5f) * _cellSize);
                Vector3 size = new Vector3(_cellSize * 0.98f, 0.02f, _cellSize * 0.98f);
                Gizmos.DrawWireCube(center, size);
            }
        }
    }
#endif
}
