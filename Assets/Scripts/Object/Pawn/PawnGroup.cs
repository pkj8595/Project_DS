using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class PawnGroup : MonoBehaviour
{
    public List<PawnBase> _pawnList;
    [field : SerializeField]private float _spacing = 1f;
    [field : SerializeField]private float _rangeFlagToPawn = 4f;
    public GameObject GroupFlag;
    public bool IsSeleced { get => GroupFlag.activeInHierarchy; }

    private System.Threading.CancellationTokenSource _sourceCheckPosition;


    private void Start()
    {
        StartTask();
    }

    private void OnEnable()
    {
        StartTask();
    }
   
    private void OnDisable()
    {
        CancelTask();
    }

    private void OnDestroy()
    {
        CancelTask();
    }

    public void Init(Vector2Int point)
    {

    }

    private void StartTask()
    {
        CancelTask();
        _sourceCheckPosition = new();
        CheckPawnPosition().Forget();
    }

    private void CancelTask()
    {
        if (_sourceCheckPosition != null)
        {
            _sourceCheckPosition.Cancel();
            _sourceCheckPosition.Dispose();
            _sourceCheckPosition = null;
        }
    }

    public void MoveGroupTo(Vector3 targetPosition)
    {
        //game object 이동
        GroupFlag.transform.position = targetPosition;

        for (int i = 0; i < _pawnList.Count; i++)
        {
            MovePawnDesignatedPosition(i);
        }
    }

    private void MovePawnDesignatedPosition(int index)
    {
        Vector3 offset = CalculateOffset(index, _pawnList.Count, _spacing);
        _pawnList[index].SetDestination(GroupFlag.transform.position + offset);
    }

    // 개별 에이전트의 위치 오프셋 계산
    private Vector3 CalculateOffset(int index, int totalAgents, float spacing)
    {
        float angle = (360f / totalAgents) * index;
        Vector3 offset = new Vector3(Mathf.Sin(angle), 0, Mathf.Cos(angle)) * spacing;
        return offset;
    }

    public void Selected()
    {
        GroupFlag.SetActive(true);
    }

    public void NotSelected()
    {
        GroupFlag.SetActive(false);
    }
    

    async UniTaskVoid CheckPawnPosition()
    {
        await UniTask.NextFrame();

        while (true)
        {
            for(int i =0; i< _pawnList.Count; i++)
            {
                //if (_pawnList[i].IsDead())
                //    continue;

                if (_rangeFlagToPawn * _rangeFlagToPawn < 
                    (GroupFlag.transform.position - _pawnList[i].transform.position).sqrMagnitude)
                {
                    MovePawnDesignatedPosition(i);
                }
            }

            await UniTask.Delay(1000, cancellationToken : _sourceCheckPosition.Token);
        }
    }



}
