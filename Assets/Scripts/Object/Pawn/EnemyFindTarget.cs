using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Cysharp.Threading.Tasks;

public class EnemyFindTarget : MonoBehaviour
{
    PawnBase _pawnBase;
    NavMeshAgent _agent;
    
    public void Init()
    {
        _pawnBase = GetComponent<PawnBase>();
        _agent = GetComponent<NavMeshAgent>();
        StartSearchTask().Forget();
    }

    async UniTaskVoid StartSearchTask()
    {
        await UniTask.Delay(1000);

        while (true)
        {
            if (_pawnBase.IsDead())
                return;

            if (!_pawnBase.HasTarget &&
                (_pawnBase.State == Define.EPawnAniState.Idle ||
                 _pawnBase.State == Define.EPawnAniState.Ready))
            {
                if (SearchRandomBuildingPosition(out Vector3 retPosition))
                {
                    _pawnBase.SetDestination(retPosition);
                }
            }

            await UniTask.Delay(2000,cancellationToken: gameObject.GetCancellationTokenOnDestroy());
        }
    }

    public bool SearchRandomBuildingPosition(out Vector3 retPosition)
    {
        retPosition = Vector3.zero;
        List<BuildingNode> buildingList = GameView.Instance.ConstructedBuildingList;
        List<Vector3> moveablePositionList = new();
        for (int i = 0; i < buildingList.Count; i++)
        {
            if (buildingList[i].TryGetComponent(out IDamageable damageable) && !damageable.IsDead())
            {
                Vector3 closetPoint = damageable.GetCollider().ClosestPoint(transform.position);
                if (BoardManager.Instance.GetMoveablePosition(closetPoint,
                                                        out Vector3 moveablePosition, 4f))
                {
                    moveablePositionList.Add(moveablePosition);
                }
            }
        }

        if (0 < moveablePositionList.Count)
        {
            int index = Random.Range(0, moveablePositionList.Count);
            retPosition = moveablePositionList[index];
            return true;
        }
        else
            return false;
    }

    public bool SearchTargetPosition(out Vector3 retPosition)
    {
        bool isFind = false;
        retPosition = Vector3.zero;
        float distance = float.MaxValue;
        NavMeshPath path = new NavMeshPath();
        var buildingList = GameView.Instance.ConstructedBuildingList;
        foreach(var building in buildingList)
        {
            if (building.TryGetComponent(out IDamageable damageable) && !damageable.IsDead())
            {
                if (BoardManager.Instance.GetMoveablePosition(damageable.GetTransform().position,
                                                              out Vector3 moveablePosition,3f))
                {
                    _agent.CalculatePath(moveablePosition, path);
                    if (path.status == NavMeshPathStatus.PathComplete)
                    {
                        float pathLength = GetPathLength(path);
                        if (pathLength < distance)
                        {
                            distance = pathLength;
                            retPosition = moveablePosition;
                            isFind = true;
                        }
                    }

                }
            }
        }
        return isFind;
    }
    float GetPathLength(NavMeshPath path)
    {
        float length = 0.0f;
        if (path.corners.Length < 2)
            return length;

        for (int i = 1; i < path.corners.Length; i++)
        {
            length += Vector3.Distance(path.corners[i - 1], path.corners[i]);
        }
        return length;
    }

}
