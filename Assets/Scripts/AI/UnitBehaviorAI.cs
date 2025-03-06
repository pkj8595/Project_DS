using System.Linq;
using Unity.Behavior;
using UnityEngine;
using UnityEngine.AI;

public class UnitBehaviorAI : MonoBehaviour
{
    private Transform _target;
    private NavMeshAgent _agent;
    private BehaviorGraphAgent _behaviorAgent;

    public void Setup(Transform target, GameObject[] wayPoints)
    {
        _target = target;
        _agent.updateRotation = false;
        _agent.updateUpAxis = false;

        _behaviorAgent.SetVariableValue("PartrolPoints", wayPoints.ToList());
    }
}
