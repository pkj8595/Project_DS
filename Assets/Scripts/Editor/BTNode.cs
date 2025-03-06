using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class BTNode : ScriptableObject
{
    [HideInInspector] public List<BTNode> children = new List<BTNode>();

    public abstract NodeState Execute(AIContext context);
}


