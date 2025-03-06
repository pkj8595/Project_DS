using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "NewActionNode", menuName = "AI/Nodes/Action")]
public class ActionNode : BTNode
{
    public override NodeState Execute(AIContext context)
    {
        Debug.Log("AI가 행동 실행!");
        return NodeState.Success;
    }
}