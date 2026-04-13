using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

/// <summary>
/// 경로 좌표를 입력해 EnemyRouteData를 저장하고, 씬 뷰에 미리보기합니다.
/// Tools / KSS / Enemy Route Generator
/// </summary>
public sealed class EnemyRouteGeneratorWindow : EditorWindow
{
    const string MenuPath = "Tools/KSS/Enemy Route Generator";
    const string SaveFolder = "Assets/00_Data/EnemyRoutes";

    BattleGrid _battleGrid;
    readonly List<Vector2Int> _pathCells = new List<Vector2Int>();

    string _fileName = "Route1";
    bool _previewInScene = true;
    bool _clearAfterSave = true;

    EnemyRouteData _loadFromAsset;
    Vector2 _scroll;

    [MenuItem(MenuPath)]
    static void Open()
    {
        var w = GetWindow<EnemyRouteGeneratorWindow>("Enemy Route Generator");
        w.minSize = new Vector2(360f, 420f);
    }

    void OnEnable()
    {
        SceneView.duringSceneGui += OnSceneGUI;
    }

    void OnDisable()
    {
        SceneView.duringSceneGui -= OnSceneGUI;
    }

    void OnGUI()
    {
        EditorGUILayout.LabelField("Battle Grid", EditorStyles.boldLabel);
        _battleGrid = (BattleGrid)EditorGUILayout.ObjectField("그리드", _battleGrid, typeof(BattleGrid), true);

        EditorGUILayout.Space(6f);
        _previewInScene = EditorGUILayout.ToggleLeft("씬 뷰에 미리보기", _previewInScene);

        EditorGUILayout.Space(8f);
        EditorGUILayout.LabelField("경로 (셀 좌표)", EditorStyles.boldLabel);
        DrawValidationHelp();

        if (_battleGrid == null && _pathCells.Count > 0)
            EditorGUILayout.HelpBox("BattleGrid를 넣으면 셀 범위 검사와 씬 미리보기가 됩니다.", MessageType.Info);

        _scroll = EditorGUILayout.BeginScrollView(_scroll, GUILayout.MinHeight(200f));
        for (int i = 0; i < _pathCells.Count; i++)
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField($"{i}", GUILayout.Width(24f));
                _pathCells[i] = EditorGUILayout.Vector2IntField(GUIContent.none, _pathCells[i]);
                if (GUILayout.Button("−", GUILayout.Width(24f)))
                {
                    _pathCells.RemoveAt(i);
                    GUIUtility.ExitGUI();
                }
            }
        }
        EditorGUILayout.EndScrollView();

        using (new EditorGUILayout.HorizontalScope())
        {
            if (GUILayout.Button("+ 칸 추가"))
                _pathCells.Add(Vector2Int.zero);
            if (GUILayout.Button("전체 지우기"))
            {
                if (EditorUtility.DisplayDialog("경로 초기화", "목록을 비울까요?", "예", "아니오"))
                    _pathCells.Clear();
            }
        }

        EditorGUILayout.Space(8f);
        EditorGUILayout.LabelField("마지막 칸에서 한 칸 확장 (4방)", EditorStyles.boldLabel);
        EditorGUILayout.LabelField(
            "첫 칸은 위에서 「+ 칸 추가」로 넣은 뒤, 아래 버튼으로 직선·꺾인 경로를 빠르게 이을 수 있습니다.",
            EditorStyles.wordWrappedMiniLabel);

        EditorGUI.BeginDisabledGroup(_pathCells.Count == 0);
        using (new EditorGUILayout.HorizontalScope())
        {
            GUILayout.FlexibleSpace();
            if (GUILayout.Button(new GUIContent("↑\ny+1", "셀 y 증가 (월드 +Z 방향)"), GUILayout.Width(52f), GUILayout.Height(40f)))
                TryAppendDelta(new Vector2Int(0, 1));
            GUILayout.FlexibleSpace();
        }

        using (new EditorGUILayout.HorizontalScope())
        {
            GUILayout.FlexibleSpace();
            if (GUILayout.Button(new GUIContent("←\nx-1", "셀 x 감소 (월드 -X)"), GUILayout.Width(52f), GUILayout.Height(40f)))
                TryAppendDelta(new Vector2Int(-1, 0));
            GUILayout.Space(10f);
            if (GUILayout.Button(new GUIContent("→\nx+1", "셀 x 증가 (월드 +X)"), GUILayout.Width(52f), GUILayout.Height(40f)))
                TryAppendDelta(new Vector2Int(1, 0));
            GUILayout.FlexibleSpace();
        }

        using (new EditorGUILayout.HorizontalScope())
        {
            GUILayout.FlexibleSpace();
            if (GUILayout.Button(new GUIContent("↓\ny-1", "셀 y 감소 (월드 -Z)"), GUILayout.Width(52f), GUILayout.Height(40f)))
                TryAppendDelta(new Vector2Int(0, -1));
            GUILayout.FlexibleSpace();
        }

        EditorGUI.EndDisabledGroup();

        EditorGUILayout.Space(10f);
        EditorGUILayout.LabelField("에셋 불러오기 (선택)", EditorStyles.miniLabel);
        using (new EditorGUILayout.HorizontalScope())
        {
            _loadFromAsset = (EnemyRouteData)EditorGUILayout.ObjectField(_loadFromAsset, typeof(EnemyRouteData), false);
            EditorGUI.BeginDisabledGroup(_loadFromAsset == null);
            if (GUILayout.Button("불러오기", GUILayout.Width(80f)))
                LoadFromAsset(_loadFromAsset);
            EditorGUI.EndDisabledGroup();
        }

        EditorGUILayout.Space(10f);
        EditorGUILayout.LabelField("저장", EditorStyles.boldLabel);
        _fileName = EditorGUILayout.TextField("파일 이름 (.asset 제외)", _fileName);
        _clearAfterSave = EditorGUILayout.ToggleLeft("저장 후 목록 비우기", _clearAfterSave);

        using (new EditorGUILayout.HorizontalScope())
        {
            if (GUILayout.Button("EnemyRouteData로 저장", GUILayout.Height(28f)))
                TrySave();
        }

        EditorGUILayout.Space(6f);
        EditorGUILayout.HelpBox(
            "저장 위치: " + SaveFolder + "\n연속 두 칸은 4방 인접이어야 합니다. 빈 경로는 저장할 수 없습니다.",
            MessageType.None);
    }

    void DrawValidationHelp()
    {
        var msg = GetValidationMessage();
        if (string.IsNullOrEmpty(msg))
            return;
        EditorGUILayout.HelpBox(msg, MessageType.Warning);
    }

    string GetValidationMessage()
    {
        for (int i = 0; i < _pathCells.Count - 1; i++)
        {
            var a = _pathCells[i];
            var b = _pathCells[i + 1];
            int manhattan = Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
            if (manhattan != 1)
                return $"인접 아님 ({i}→{i + 1}): ({a.x},{a.y}) → ({b.x},{b.y}). 4방만 허용.";
        }

        if (_battleGrid != null)
        {
            for (int i = 0; i < _pathCells.Count; i++)
            {
                var c = _pathCells[i];
                if (!_battleGrid.ContainsCell(c))
                    return $"인덱스 {i}: ({c.x},{c.y}) 는 그리드 범위 밖입니다.";
            }
        }

        return null;
    }

    void TryAppendDelta(Vector2Int delta)
    {
        if (_pathCells.Count == 0)
            return;

        var last = _pathCells[_pathCells.Count - 1];
        var next = last + delta;

        if (_battleGrid != null && !_battleGrid.ContainsCell(next))
        {
            EditorUtility.DisplayDialog("범위 밖", $"({next.x}, {next.y}) 는 그리드 밖입니다.", "확인");
            return;
        }

        _pathCells.Add(next);
        Repaint();
        SceneView.RepaintAll();
    }

    void LoadFromAsset(EnemyRouteData data)
    {
        if (data == null)
            return;
        _pathCells.Clear();
        var path = data.PathCells;
        if (path != null)
        {
            for (int i = 0; i < path.Count; i++)
                _pathCells.Add(path[i]);
        }
        _fileName = data.name;
        EditorUtility.DisplayDialog("불러오기", "경로를 창에 복사했습니다.", "확인");
    }

    void TrySave()
    {
        if (_pathCells.Count == 0)
        {
            EditorUtility.DisplayDialog("저장 불가", "경로가 비어 있습니다.", "확인");
            return;
        }

        string err = GetValidationMessage();
        if (!string.IsNullOrEmpty(err))
        {
            EditorUtility.DisplayDialog("저장 불가", err, "확인");
            return;
        }

        string safeName = SanitizeFileName(_fileName);
        if (string.IsNullOrEmpty(safeName))
        {
            EditorUtility.DisplayDialog("저장 불가", "파일 이름이 비었습니다.", "확인");
            return;
        }

        EnsureSaveFolderExists();
        string assetPath = Path.Combine(SaveFolder, safeName + ".asset").Replace('\\', '/');

        if (File.Exists(assetPath))
        {
            if (!EditorUtility.DisplayDialog("덮어쓰기", $"이미 있습니다:\n{assetPath}\n덮어쓸까요?", "예", "아니오"))
                return;
        }

        var data = ScriptableObject.CreateInstance<EnemyRouteData>();
        var so = new SerializedObject(data);
        var prop = so.FindProperty("_pathCells");
        prop.ClearArray();
        for (int i = 0; i < _pathCells.Count; i++)
        {
            prop.InsertArrayElementAtIndex(i);
            prop.GetArrayElementAtIndex(i).vector2IntValue = _pathCells[i];
        }
        so.ApplyModifiedPropertiesWithoutUndo();

        if (File.Exists(assetPath))
            AssetDatabase.DeleteAsset(assetPath);

        AssetDatabase.CreateAsset(data, assetPath);
        AssetDatabase.SaveAssets();
        EditorGUIUtility.PingObject(data);

        if (_clearAfterSave)
            _pathCells.Clear();

        EditorUtility.DisplayDialog("저장 완료", assetPath, "확인");
    }

    static void EnsureSaveFolderExists()
    {
        if (!AssetDatabase.IsValidFolder("Assets/00_Data"))
            AssetDatabase.CreateFolder("Assets", "00_Data");
        if (!AssetDatabase.IsValidFolder(SaveFolder))
            AssetDatabase.CreateFolder("Assets/00_Data", "EnemyRoutes");
    }

    static string SanitizeFileName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return null;
        string s = Regex.Replace(name.Trim(), @"[\\/:*?""<>|]", "_");
        return string.IsNullOrEmpty(s) ? null : s;
    }

    void OnSceneGUI(SceneView sceneView)
    {
        if (!_previewInScene || _battleGrid == null || _pathCells.Count == 0)
            return;

        Handles.zTest = UnityEngine.Rendering.CompareFunction.LessEqual;
        Color lineCol = new Color(0.2f, 0.95f, 0.3f, 0.95f);
        Color nodeCol = new Color(1f, 0.85f, 0.2f, 1f);

        if (_pathCells.Count >= 2)
        {
            Handles.color = lineCol;
            for (int i = 0; i < _pathCells.Count - 1; i++)
            {
                Vector3 a = _battleGrid.CellToWorldCenter(_pathCells[i]) + Vector3.up * 0.05f;
                Vector3 b = _battleGrid.CellToWorldCenter(_pathCells[i + 1]) + Vector3.up * 0.05f;
                Handles.DrawLine(a, b);
            }
        }

        float r = _battleGrid.CellSize * 0.18f;
        for (int i = 0; i < _pathCells.Count; i++)
        {
            Vector3 p = _battleGrid.CellToWorldCenter(_pathCells[i]) + Vector3.up * 0.06f;
            Handles.color = nodeCol;
            Handles.DrawSolidDisc(p, Vector3.up, r);
            Handles.Label(p + Vector3.up * (r * 2f), $"  {i}");
        }

        Handles.zTest = UnityEngine.Rendering.CompareFunction.Always;
    }
}
