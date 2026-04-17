using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 보드 크기·프리뷰 그리드·홈/장애물/스포너 칸 편집 후 BoardData + EnemySpawnerData 생성.
/// Tools / KSS / Board Authoring
/// </summary>
public sealed class BoardAuthoringWindow : EditorWindow
{
    const string MenuPath = "Tools/KSS/Board Authoring";
    /// <summary>기존 씬의 프리뷰 루트와 호환되도록 이름 유지.</summary>
    const string PreviewRootName = "KSS_MapPreviewRoot";
    const string BoardsDataFolder = "Assets/00_Data/Maps";
    const string SpawnersFolder = "Assets/00_Data/Spawners";

    enum PaintMode
    {
        None = 0,
        Home = 1,
        Obstacle = 2,
        Spawner = 3,
    }

    int _columns = 8;
    int _rows = 8;
    float _cellSize = 1f;
    Vector3 _gridOrigin = Vector3.zero;

    PaintMode _paintMode = PaintMode.None;

    bool _hasHome;
    Vector2Int _homeCell;

    readonly HashSet<Vector2Int> _obstacles = new HashSet<Vector2Int>();
    readonly HashSet<Vector2Int> _spawners = new HashSet<Vector2Int>();

    BattleGrid _previewGrid;
    Vector2Int? _hoverCell;
    Vector2 _scroll;

    string _boardBaseName = "Board01";

    BoardData _boardToLoad;

    [MenuItem(MenuPath)]
    static void Open()
    {
        var w = GetWindow<BoardAuthoringWindow>("Board Authoring");
        w.minSize = new Vector2(380f, 520f);
    }

    void OnEnable()
    {
        SceneView.duringSceneGui += OnSceneGUI;
        FindPreviewGrid();
    }

    void OnDisable()
    {
        SceneView.duringSceneGui -= OnSceneGUI;
    }

    void FindPreviewGrid()
    {
        var root = GameObject.Find(PreviewRootName);
        if (root != null)
            _previewGrid = root.GetComponent<BattleGrid>();
    }

    void LoadFromBoardData(BoardData data)
    {
        if (data == null)
            return;

        _columns = data.Columns;
        _rows = data.Rows;
        _cellSize = data.CellSize;
        _gridOrigin = data.GridOriginWorld;
        _hasHome = data.HasHome;
        _homeCell = data.HomeCell;

        _obstacles.Clear();
        foreach (var c in data.ObstacleCells)
            _obstacles.Add(c);

        _spawners.Clear();
        foreach (var sp in data.Spawners)
        {
            if (sp != null)
                _spawners.Add(sp.SpawnCell);
        }

        _boardBaseName = data.name;
        CreateOrUpdatePreviewGrid();
        FindPreviewGrid();
        SceneView.RepaintAll();
        EditorUtility.DisplayDialog("불러오기", $"「{data.name}」 데이터를 에디터에 적용했습니다.", "확인");
    }

    void OnGUI()
    {
        _scroll = EditorGUILayout.BeginScrollView(_scroll);

        EditorGUILayout.LabelField("기존 보드 불러오기", EditorStyles.boldLabel);
        _boardToLoad = (BoardData)EditorGUILayout.ObjectField("BoardData", _boardToLoad, typeof(BoardData), false);
        using (new EditorGUILayout.HorizontalScope())
        {
            EditorGUI.BeginDisabledGroup(_boardToLoad == null);
            if (GUILayout.Button("에디터에 적용 (크기·홈·장애물·스포너 칸)", GUILayout.Height(24f)))
                LoadFromBoardData(_boardToLoad);
            EditorGUI.EndDisabledGroup();
        }
        EditorGUILayout.HelpBox(
            "프로젝트의 BoardData 에셋을 넣고 적용하면, 아래 값과 페인트 상태가 그 보드 기준으로 채워집니다. 저장 시 같은 이름으로 덮어쓰게 됩니다.",
            MessageType.None);

        EditorGUILayout.Space(10f);
        EditorGUILayout.LabelField("그리드 설정", EditorStyles.boldLabel);
        _columns = Mathf.Max(1, EditorGUILayout.IntField("열 (Columns)", _columns));
        _rows = Mathf.Max(1, EditorGUILayout.IntField("행 (Rows)", _rows));
        _cellSize = Mathf.Max(0.01f, EditorGUILayout.FloatField("셀 크기", _cellSize));
        _gridOrigin = EditorGUILayout.Vector3Field("원점 (셀 0,0 모서리)", _gridOrigin);

        EditorGUILayout.Space(6f);
        if (GUILayout.Button("씬에 프리뷰 그리드 생성/갱신", GUILayout.Height(26f)))
            CreateOrUpdatePreviewGrid();

        EditorGUILayout.HelpBox(
            "프리뷰 오브젝트 이름: \"" + PreviewRootName + "\". 씬에 BattleGrid가 있어야 씬 뷰에서 칸을 찍을 수 있습니다.",
            MessageType.Info);

        _previewGrid = (BattleGrid)EditorGUILayout.ObjectField("프리뷰 BattleGrid (자동)", _previewGrid, typeof(BattleGrid), true);

        EditorGUILayout.Space(10f);
        EditorGUILayout.LabelField("페인트 모드", EditorStyles.boldLabel);
        _paintMode = (PaintMode)EditorGUILayout.EnumPopup("모드", _paintMode);
        EditorGUILayout.HelpBox(
            "홈: 클릭한 칸을 기지로 설정 (하나).\n장애물: 클릭으로 토글.\n스포너: 클릭으로 토글.",
            MessageType.None);

        EditorGUILayout.Space(8f);
        EditorGUILayout.LabelField("저장용 파일 이름 (확장자 없음)", EditorStyles.boldLabel);
        _boardBaseName = EditorGUILayout.TextField("보드 이름", _boardBaseName);

        EditorGUILayout.Space(6f);
        if (GUILayout.Button("BoardData + 스포너 SO 저장", GUILayout.Height(30f)))
            TrySaveBoardAndSpawners();

        EditorGUILayout.Space(8f);
        DrawSummary();

        EditorGUILayout.EndScrollView();
    }

    void DrawSummary()
    {
        EditorGUILayout.LabelField("현재 상태", EditorStyles.boldLabel);
        EditorGUILayout.LabelField("홈", _hasHome ? $"({_homeCell.x},{_homeCell.y})" : "(미설정)");
        EditorGUILayout.LabelField("장애물", _obstacles.Count.ToString());
        EditorGUILayout.LabelField("스포너 칸", _spawners.Count.ToString());
    }

    void CreateOrUpdatePreviewGrid()
    {
        var root = GameObject.Find(PreviewRootName);
        if (root == null)
        {
            root = new GameObject(PreviewRootName);
            Undo.RegisterCreatedObjectUndo(root, "Create Board Preview");
        }

        var grid = root.GetComponent<BattleGrid>();
        if (grid == null)
            grid = Undo.AddComponent<BattleGrid>(root);

        grid.ApplyGridSettings(_columns, _rows, _cellSize, _gridOrigin);

        var plane = root.transform.Find("PreviewPlane");
        if (plane == null)
        {
            var p = GameObject.CreatePrimitive(PrimitiveType.Plane);
            p.name = "PreviewPlane";
            p.transform.SetParent(root.transform, false);
            p.transform.localRotation = Quaternion.identity;
            Undo.RegisterCreatedObjectUndo(p, "Board Preview Plane");
        }

        plane = root.transform.Find("PreviewPlane");
        if (plane != null)
        {
            float w = _columns * _cellSize;
            float h = _rows * _cellSize;
            plane.localScale = new Vector3(w / 10f, 1f, h / 10f);
            plane.localPosition = new Vector3(w * 0.5f, 0f, h * 0.5f);
            var collider = plane.GetComponent<Collider>();
            if (collider != null)
                collider.hideFlags = HideFlags.None;
        }

        _previewGrid = grid;
        EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
        SceneView.RepaintAll();
    }

    void OnSceneGUI(SceneView sceneView)
    {
        if (_previewGrid == null)
            return;

        Event e = Event.current;
        Vector2Int? cellUnderMouse = GetHoveredCell(e.mousePosition, sceneView);

        _hoverCell = cellUnderMouse;

        if (cellUnderMouse.HasValue && _previewGrid.ContainsCell(cellUnderMouse.Value))
        {
            Vector3 c = _previewGrid.CellToWorldCenter(cellUnderMouse.Value);
            Handles.color = new Color(0.3f, 1f, 0.5f, 0.35f);
            float s = _previewGrid.CellSize * 0.95f;
            Handles.DrawWireCube(c + Vector3.up * 0.02f, new Vector3(s, 0.02f, s));
        }

        if (e.type == EventType.MouseDown && e.button == 0 && cellUnderMouse.HasValue)
        {
            var cell = cellUnderMouse.Value;
            if (!_previewGrid.ContainsCell(cell))
                return;

            if (_paintMode == PaintMode.Home)
            {
                _homeCell = cell;
                _hasHome = true;
                e.Use();
            }
            else if (_paintMode == PaintMode.Obstacle)
            {
                if (!_obstacles.Add(cell))
                    _obstacles.Remove(cell);
                e.Use();
            }
            else if (_paintMode == PaintMode.Spawner)
            {
                if (!_spawners.Add(cell))
                    _spawners.Remove(cell);
                e.Use();
            }

            Repaint();
        }

        foreach (var o in _obstacles)
        {
            if (!_previewGrid.ContainsCell(o))
                continue;
            Vector3 p = _previewGrid.CellToWorldCenter(o) + Vector3.up * 0.04f;
            Handles.color = new Color(0.8f, 0.2f, 0.2f, 0.9f);
            float r = _previewGrid.CellSize * 0.25f;
            Handles.DrawSolidDisc(p, Vector3.up, r);
        }

        foreach (var s in _spawners)
        {
            if (!_previewGrid.ContainsCell(s))
                continue;
            Vector3 p = _previewGrid.CellToWorldCenter(s) + Vector3.up * 0.05f;
            Handles.color = new Color(0.2f, 0.5f, 1f, 0.95f);
            float r = _previewGrid.CellSize * 0.22f;
            Handles.DrawWireDisc(p, Vector3.up, r);
            Handles.Label(p + Vector3.up * 0.15f, "S");
        }

        if (_hasHome && _previewGrid.ContainsCell(_homeCell))
        {
            Vector3 p = _previewGrid.CellToWorldCenter(_homeCell) + Vector3.up * 0.06f;
            Handles.color = Color.yellow;
            float r = _previewGrid.CellSize * 0.28f;
            Handles.DrawSolidDisc(p, Vector3.up, r);
            Handles.Label(p + Vector3.up * 0.2f, "H");
        }
    }

    Vector2Int? GetHoveredCell(Vector2 mousePosition, SceneView _)
    {
        if (_previewGrid == null)
            return null;

        Ray ray = HandleUtility.GUIPointToWorldRay(mousePosition);
        float originY = _previewGrid.Origin.y;
        Plane plane = new Plane(Vector3.up, new Vector3(0f, originY, 0f));
        if (!plane.Raycast(ray, out float dist))
            return null;

        Vector3 hit = ray.GetPoint(dist);
        return _previewGrid.WorldToCell(hit);
    }

    void TrySaveBoardAndSpawners()
    {
        if (_previewGrid == null)
        {
            EditorUtility.DisplayDialog("저장 불가", "먼저 「씬에 프리뷰 그리드 생성/갱신」을 눌러 주세요.", "확인");
            return;
        }

        string baseName = SanitizeFileName(_boardBaseName);
        if (string.IsNullOrEmpty(baseName))
        {
            EditorUtility.DisplayDialog("저장 불가", "보드 이름이 비었습니다.", "확인");
            return;
        }

        if (!_hasHome)
        {
            if (!EditorUtility.DisplayDialog("홈 미설정", "홈 칸이 없습니다. 그래도 저장할까요?", "저장", "취소"))
                return;
        }

        EnsureFolder(BoardsDataFolder);
        EnsureFolder(SpawnersFolder);

        string boardPath = Path.Combine(BoardsDataFolder, baseName + ".asset").Replace('\\', '/');
        if (File.Exists(boardPath))
        {
            if (!EditorUtility.DisplayDialog("덮어쓰기", $"이미 있습니다:\n{boardPath}\n덮어쓸까요?", "예", "아니오"))
                return;
        }

        var spawnerAssets = new List<EnemySpawnerData>();

        var sortedSpawnerCells = new List<Vector2Int>(_spawners);
        sortedSpawnerCells.Sort((a, b) => a.x != b.x ? a.x.CompareTo(b.x) : a.y.CompareTo(b.y));

        AssetDatabase.StartAssetEditing();
        try
        {
            // BoardData를 먼저 에셋으로 등록해야 스포너에서 참조가 유지됨 (미등록 인스턴스 참조는 저장 시 유실될 수 있음).
            BoardData board;
            if (File.Exists(boardPath))
            {
                board = AssetDatabase.LoadAssetAtPath<BoardData>(boardPath);
                if (board == null)
                    board = ScriptableObject.CreateInstance<BoardData>();
            }
            else
            {
                board = ScriptableObject.CreateInstance<BoardData>();
            }

            var mso = new SerializedObject(board);
            mso.FindProperty("_columns").intValue = _columns;
            mso.FindProperty("_rows").intValue = _rows;
            mso.FindProperty("_cellSize").floatValue = _cellSize;
            mso.FindProperty("_gridOriginWorld").vector3Value = _gridOrigin;
            mso.FindProperty("_hasHome").boolValue = _hasHome;
            mso.FindProperty("_homeCell").vector2IntValue = _homeCell;

            var obs = mso.FindProperty("_obstacleCells");
            obs.ClearArray();
            int oi = 0;
            foreach (var c in _obstacles)
            {
                obs.InsertArrayElementAtIndex(oi);
                obs.GetArrayElementAtIndex(oi).vector2IntValue = c;
                oi++;
            }

            mso.FindProperty("_spawners").ClearArray();
            mso.ApplyModifiedPropertiesWithoutUndo();

            if (!File.Exists(boardPath))
                AssetDatabase.CreateAsset(board, boardPath);
            else
                EditorUtility.SetDirty(board);

            AssetDatabase.SaveAssets();

            board = AssetDatabase.LoadAssetAtPath<BoardData>(boardPath);

            int idx = 0;
            foreach (var cell in sortedSpawnerCells)
            {
                string spPath = Path.Combine(SpawnersFolder, $"{baseName}_Spawner_{idx}.asset").Replace('\\', '/');
                var sp = ScriptableObject.CreateInstance<EnemySpawnerData>();
                var spso = new SerializedObject(sp);
                spso.FindProperty("_boardData").objectReferenceValue = board;
                spso.FindProperty("_spawnCell").vector2IntValue = cell;
                spso.FindProperty("_routes").ClearArray();
                spso.ApplyModifiedPropertiesWithoutUndo();

                if (File.Exists(spPath))
                    AssetDatabase.DeleteAsset(spPath);

                AssetDatabase.CreateAsset(sp, spPath);
                spawnerAssets.Add(sp);
                idx++;
            }

            mso = new SerializedObject(board);
            var spr = mso.FindProperty("_spawners");
            spr.ClearArray();
            for (int i = 0; i < spawnerAssets.Count; i++)
            {
                spr.InsertArrayElementAtIndex(i);
                spr.GetArrayElementAtIndex(i).objectReferenceValue = spawnerAssets[i];
            }

            mso.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(board);

            AssetDatabase.SaveAssets();
            EditorGUIUtility.PingObject(board);
        }
        finally
        {
            AssetDatabase.StopAssetEditing();
        }

        EditorUtility.DisplayDialog("저장 완료", $"{boardPath}\n스포너 {spawnerAssets.Count}개 ({SpawnersFolder})", "확인");
    }

    static void EnsureFolder(string path)
    {
        if (AssetDatabase.IsValidFolder(path))
            return;
        if (path.StartsWith("Assets/"))
        {
            string rest = path.Substring("Assets/".Length);
            string[] parts = rest.Split('/');
            string current = "Assets";
            foreach (var p in parts)
            {
                if (string.IsNullOrEmpty(p))
                    continue;
                string next = current + "/" + p;
                if (!AssetDatabase.IsValidFolder(next))
                    AssetDatabase.CreateFolder(current, p);
                current = next;
            }
        }
    }

    static string SanitizeFileName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return null;
        string s = Regex.Replace(name.Trim(), @"[\\/:*?""<>|]", "_");
        return string.IsNullOrEmpty(s) ? null : s;
    }
}
