using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "NewBehaviorTree", menuName = "AI/Behavior Tree")]
public class BehaviorTree : ScriptableObject
{
    public BTNode rootNode;
    public List<BTNode> nodes = new List<BTNode>();
}

public enum NodeState
{
    Failure,
    Running,
    Success,
}

