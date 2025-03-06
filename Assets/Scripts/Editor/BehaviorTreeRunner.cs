using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehaviorTreeRunner : MonoBehaviour
{
    public BehaviorTree behaviorTree;
    private AIContext context;

    void Start()
    {
        context = new AIContext();
    }

    void Update()
    {
        if (behaviorTree.rootNode != null)
        {
            behaviorTree.rootNode.Execute(context);
        }
    }
}
