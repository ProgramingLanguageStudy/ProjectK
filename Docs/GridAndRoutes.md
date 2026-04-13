# 배틀 그리드 · 적 경로 · 용어 (뼈대)

변경 자유. 공통으로만 맞추면 됨.

## 용어

| 말 | 뜻 |
|----|-----|
| **격자 / 셀** | `BattleGrid`가 나눈 한 칸. 좌표는 정수 `Vector2Int`. |
| **셀 좌표** | (0,0)이 왼쪽 아래인 **타일 인덱스**. 0~1 실수 정규화 아님. |
| **경로** | 셀 좌표를 **순서대로** 나열한 목록. |
| **스폰 셀** | 경로의 **첫** 칸. |
| **목표 셀** | 경로의 **마지막** 칸 (기지가 있는 칸으로 두는 것이 일반적). |
| **EnemyRouteData** | 경로만 담는 **ScriptableObject** (`PathCells`). 에셋은 `Assets/00_Data/EnemyRoutes/` 등에 두고, 파일명으로 맵/스포너 구분 가능. |
| **Route Generator** | 에디터 메뉴 **Tools → KSS → Enemy Route Generator** — 좌표 목록 편집·씬 미리보기·SO 저장. |
| **Map Authoring** | **Tools → KSS → Map Authoring** — 맵 크기·프리뷰 그리드·홈/장애물/스포너 칸 편집 후 `MapData` + `EnemySpawnerData` 저장 (`Assets/00_Data/Maps`, `Assets/00_Data/Spawners`). 기존 `MapData`를 넣고 **에디터에 적용**으로 불러와 수정 가능. |
| **EnemySpawnerData** | 스폰 셀 + (추가 예정) 루트 목록. 맵 저장 시 스포너 칸마다 생성. |
| **EnemyRoute** (컴포넌트) | (선택) 씬 편집용으로만 쓸 수 있음. 런타임 필수는 아님. |
| **Enemy** (스크립트) | 씬/프리팹에 붙는 **이동·도착 처리**용 컴포넌트. |
| **EnemySpawner** | 프리팹 스폰 + `Enemy.Initialize(...)` 호출. |

## 좌표

- **원점 (0,0)**: 판 **왼쪽 아래** 모서리 칸 (`BattleGrid` 원점과 일치).
- **한 칸**: `Vector2Int (x, y)` — `x`는 +X, `y`는 +Z 방향.
- 월드 변환: `BattleGrid` (`CellToWorldCenter`, `WorldToCell`).

## 적 경로

- 경로 = 셀 좌표 리스트. 데이터화할 때도 같은 순서·같은 규칙을 쓰면 됨.
- (선택) 나중에 `int` 인덱스 + `Columns`로 압축 가능.

## 적 행동 (현재 목표)

1. 스폰 후 **자기 경로만** 이동.
2. (나중) 유닛과 전투 등.
3. 목표 셀 도착 시 기지 피해 등 — 세부는 단계적으로.

## 나중에 (맵·장애물)

- 맵이 커지면 **장애물 셀 목록**(`Vector2Int` 등)을 별도 데이터로 두는 식으로 확장 가능. 한 번에 다 만들 필요 없음.
