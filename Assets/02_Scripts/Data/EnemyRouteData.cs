using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 적 이동 경로만 담는 SO. 셀 좌표 규칙은 Docs/GridAndRoutes.md 참고.
/// 잘못된 데이터 방어는 런타임(이동/스폰)에서 처리.
/// </summary>
[CreateAssetMenu(fileName = "EnemyRouteData", menuName = "KSS/Enemy Route Data", order = 0)]
public class EnemyRouteData : ScriptableObject
{
    [SerializeField] private List<Vector2Int> _pathCells = new List<Vector2Int>();

    public IReadOnlyList<Vector2Int> PathCells => _pathCells;
}
