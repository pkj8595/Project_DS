using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class UnitAI
{

    public PawnBase Pawn { get; private set; }
    public bool HasTarget => Pawn.HasTarget;
    public float SearchRange => Pawn.SearchRange;
    public Vector3? OriginPosition { get; set; }

    private float _cooltime = 0.0f;
    public void Init(PawnBase pawn)
    {
        Pawn = pawn;
        OriginPosition = pawn.OriginPosition;

    }

    public void OnUpdate()
    {
        _cooltime -= Time.deltaTime;
        if (_cooltime < 0)
        {
            _cooltime += 0.3f;
        }
        else
        {
        }
    }

    /// <summary>
    /// 타겟이 범위 밖으로 나갔다면 target추적 중지
    /// </summary>
    public bool CheckOutRangeTarget() 
    {
        if (SearchRange + 3 < Vector3.Distance(Pawn.LockTarget.GetTransform().position, Pawn.transform.position) && Pawn.Team == Define.ETeam.Playable)
        {
            //SetState(GetReturnToBase());
            return true;
        }
        return false;
    }

    public bool CheckChangeState()
    {
        if (CheckOutRangeTarget())
            return true;

        return false;
    }

}
