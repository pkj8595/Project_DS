using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewSelectorNode", menuName = "AI/Nodes/Selector")]
public class SelectorNode : BTNode
{
    private int runningIndex = 0;

    public override NodeState Execute(AIContext context)
    {
        for (int i = runningIndex; i < children.Count; i++)
        {
            NodeState state = children[i].Execute(context);

            if (state == NodeState.Success)
            {
                runningIndex = 0;
                return NodeState.Success;
            }

            if (state == NodeState.Running)
            {
                runningIndex = i;
                return NodeState.Running;
            }
        }

        runningIndex = 0;
        return NodeState.Failure;
    }
}