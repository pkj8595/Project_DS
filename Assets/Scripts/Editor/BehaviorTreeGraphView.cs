using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using System;
using UnityEditor;

public class BehaviorTreeGraphView : GraphView
{
    private BehaviorTree _behaviorTree;

    public BehaviorTreeGraphView()
    {
        this.AddManipulator(new ContentDragger());
        this.AddManipulator(new SelectionDragger());
        this.AddManipulator(new RectangleSelector());

        var grid = new GridBackground();
        Insert(0, grid);

        // 🔥 우클릭 메뉴 추가
        this.AddManipulator(new ContextualMenuManipulator(BuildContextualMenu));
    }

    public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
    {
        base.BuildContextualMenu(evt);
        evt.menu.AppendAction("Add Action Node", action => AddNode(typeof(ActionNode)));
        evt.menu.AppendAction("Add Sequence Node", action => AddNode(typeof(SequenceNode)));
        evt.menu.AppendAction("Add Selector Node", action => AddNode(typeof(SelectorNode)));
    }

    private void AddNode(Type type)
    {
        if (_behaviorTree == null)
        {
            Debug.LogError("BehaviorTree가 존재하지 않습니다. 먼저 생성하세요!");
            return;
        }

        BTNode node = ScriptableObject.CreateInstance(type) as BTNode;
        if (node == null)
        {
            Debug.LogError($"노드 생성 실패: {type}");
            return;
        }

        node.name = type.Name;
        _behaviorTree.nodes.Add(node); // 🔥 노드를 트리에 추가

        // ScriptableObject로 저장
        AssetDatabase.AddObjectToAsset(node, _behaviorTree);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        // 그래프에 추가
        var nodeView = new BTNodeView(node);
        nodeView.SetPosition(new Rect(100, 100, 200, 150));
        AddElement(nodeView);
    }
}
