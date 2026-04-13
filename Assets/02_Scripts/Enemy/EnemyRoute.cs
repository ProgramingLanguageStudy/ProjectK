using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 적이 따라갈 경로. 첫 셀 = 스폰, 마지막 셀 = 기지(도착 시 피해).
/// 인스펙터에서 셀 좌표를 순서대로 넣습니다 (문서 Docs/GridAndRoutes.md 와 동일 규칙).
/// </summary>
public class EnemyRoute : MonoBehaviour
{
    [SerializeField] private BattleGrid _battleGrid;
    [SerializeField] private List<Vector2Int> _pathCells = new List<Vector2Int>();

    [Header("Gizmos")]
    [SerializeField] private bool _drawPathGizmos = true;
    [SerializeField] private Color _pathColor = new Color(1f, 0.4f, 0.2f, 0.9f);

    public BattleGrid Grid => _battleGrid;
    public IReadOnlyList<Vector2Int> PathCells => _pathCells;

    void OnDrawGizmos()
    {
        if (!_drawPathGizmos || _battleGrid == null || _pathCells == null || _pathCells.Count < 2)
            return;

        Gizmos.color = _pathColor;
        for (int i = 0; i < _pathCells.Count - 1; i++)
        {
            Vector3 a = _battleGrid.CellToWorldCenter(_pathCells[i]);
            Vector3 b = _battleGrid.CellToWorldCenter(_pathCells[i + 1]);
            Gizmos.DrawLine(a + Vector3.up * 0.05f, b + Vector3.up * 0.05f);
        }

        for (int i = 0; i < _pathCells.Count; i++)
        {
            Vector3 c = _battleGrid.CellToWorldCenter(_pathCells[i]) + Vector3.up * 0.06f;
            float r = _battleGrid.CellSize * 0.2f;
            Gizmos.DrawWireSphere(c, r);
        }
    }
}
