using UnityEngine;
using UnityEngine.Serialization;

#if UNITY_EDITOR
using Sirenix.OdinInspector;
using UnityEditor;
#endif

/// <summary>전투 보드(맵) 루트. 그리드·홈·스포너 등 씬 참조를 묶습니다.</summary>
public class Board : MonoBehaviour
{
    [SerializeField] private BoardData _boardData;
    [SerializeField] private Transform _battleGridOrigin;

    [Button(ButtonSizes.Large)]
public void OpenBoardAuthoring()
{
#if UNITY_EDITOR
    // 1. 창을 직접 생성하거나 이미 열려있는 창을 가져옵니다.
    var window = EditorWindow.GetWindow<BoardAuthoringWindow>();
    
    // 2. 창을 즉시 화면에 띄웁니다.
    window.Show();

    // 3. (중요) 창에 현재 Board가 가진 데이터를 직접 꽂아줄 수도 있습니다.
    // window.Init(_boardData); 
#endif
}
}
