using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "NewSequenceNode", menuName = "AI/Nodes/Sequence")]
public class SequenceNode : BTNode
{
    private int runningIndex = 0;

    public override NodeState Execute(AIContext context)
    {
        for (int i = runningIndex; i < children.Count; i++)
        {
            NodeState state = children[i].Execute(context);

            if (state == NodeState.Running)
            {
                runningIndex = i;
                return NodeState.Running;
            }

            if (state == NodeState.Failure)
            {
                runningIndex = 0;
                return NodeState.Failure;
            }
        }

        runningIndex = 0;
        return NodeState.Success;
    }
}

