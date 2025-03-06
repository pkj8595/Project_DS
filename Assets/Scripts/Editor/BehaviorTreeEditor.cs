using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;

public class BehaviorTreeEditor : EditorWindow
{
    private BehaviorTreeGraphView graphView;
    private BehaviorTree _behaviorTree;

    private void LoadBehaviorTree()
    {
        string path = "Assets/AI/BehaviorTree.asset"; // 🔥 원하는 경로 설정
        _behaviorTree = AssetDatabase.LoadAssetAtPath<BehaviorTree>(path);

        if (_behaviorTree == null)
        {
            _behaviorTree = ScriptableObject.CreateInstance<BehaviorTree>();
            AssetDatabase.CreateAsset(_behaviorTree, path);
            AssetDatabase.SaveAssets();
        }
    }
    [MenuItem("AI/Behavior Tree Editor")]
    public static void OpenWindow()
    {
        BehaviorTreeEditor window = GetWindow<BehaviorTreeEditor>();
        window.titleContent = new GUIContent("Behavior Tree Editor");
        window.Show();
    }

    private void OnEnable()
    {
        graphView = new BehaviorTreeGraphView()
        {
            style = { flexGrow = 1 }
        };
        rootVisualElement.Add(graphView);
    }

    private void OnDisable()
    {
        rootVisualElement.Remove(graphView);
    }
}