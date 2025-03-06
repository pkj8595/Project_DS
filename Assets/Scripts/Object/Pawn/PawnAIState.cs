using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public interface IUnitState
{
    void Init(UnitAI unitAI);
    void EnterState();
    void UpdateState();
    void AdjustUpdate();
    void ExitState();

    string GetNames();
}

public class IdleState : IUnitState
{
    UnitAI _unitAI;
    PawnBase _pawn;
    public string GetNames() => GetType().Name;

    public void Init(UnitAI unitAI)
    {
        _unitAI = unitAI;
        _pawn = unitAI.Pawn;
    }

    public void EnterState()
    {
        _unitAI.Pawn.State = Define.EPawnAniState.Idle;
    }

    public void UpdateState()
    {
        _unitAI.TrackingAndAttackTarget();
    }

    public void AdjustUpdate()
    {
        if (_pawn.HasTarget)
        {

        }
        else
        {
            _unitAI.SearchTarget();
        }
    }

    public void ExitState()
    {
        
    }
}

public class ChaseState : IUnitState
{
    private UnitAI _unitAI;
    PawnBase _pawn;
    public string GetNames() => GetType().Name;

    public void Init(UnitAI unitAI)
    {
        _unitAI = unitAI;
        _pawn = unitAI.Pawn;
    }

    public void EnterState()
    {
        _pawn.State = Define.EPawnAniState.Running;

    }

    public void UpdateState()
    {
        //이동 체크
        _unitAI.Pawn.UpdateMove();
       
    }

    public void AdjustUpdate()
    {
        //타겟이 있다면 범위 체크 밖에 나갔다면 
        if (_pawn.HasTarget)
        {
            if (!_unitAI.CheckChangeState())
            {
                _unitAI.TrackingAndAttackTarget();
            }
        }
        else
        {
            _unitAI.SearchTarget();
        }
    }

    public void ExitState()
    {
        _unitAI.Pawn.OnMoveStop();
    }
   
}


public class DeadState : IUnitState
{
    private UnitAI _unitAI;
    public string GetNames() => GetType().Name;

    public void Init(UnitAI unitAI)
    {
        _unitAI = unitAI;
    }

    public void EnterState()
    {
        _unitAI.Pawn.State = Define.EPawnAniState.Dead;
    }

    public void UpdateState()
    {

    }
    public void AdjustUpdate()
    {
    }

    public void ExitState()
    {
    }

   
}


public class ReturnToBaseState : IUnitState
{
    private UnitAI _unitAI;
    PawnBase _pawn;
    public string GetNames() => GetType().Name;

    public void Init(UnitAI unitAI)
    {
        _unitAI = unitAI;
        _pawn = unitAI.Pawn;
    }

    public void EnterState()
    {
        if (_unitAI.OriginPosition == null || _unitAI.OriginPosition == Vector3.zero)
            _unitAI.SetState(_unitAI.GetIdleState());
        else
            _pawn.SetDestination(_unitAI.OriginPosition.Value,false);
    }

    public void UpdateState()
    {
        _unitAI?.Pawn.UpdateMove();
    }
    public void AdjustUpdate()
    {
    }

    public void ExitState()
    {
        _unitAI.Pawn.OnMoveStop();
    }


}


public class MoveState : IUnitState
{
    private UnitAI _unitAI;
    PawnBase _pawn;
    public string GetNames() => GetType().Name;

    public void Init(UnitAI unitAI)
    {
        _unitAI = unitAI;
        _pawn = unitAI.Pawn;
    }

    public void EnterState()
    {
        _pawn.State = Define.EPawnAniState.Running;
    }

    public void UpdateState()
    {
        _unitAI.Pawn.UpdateMove();

    }
    public void AdjustUpdate()
    {
    }

    public void ExitState()
    {
        _unitAI.Pawn.OnMoveStop();
    }
}

public class SkillState : IUnitState
{
    private UnitAI _unitAI;
    PawnBase _pawn;
    public string GetNames() => GetType().Name;

    public void Init(UnitAI unitAI)
    {
        _unitAI = unitAI;
        _pawn = unitAI.Pawn;
    }

    public void EnterState()
    {
        _unitAI.Pawn.State = Define.EPawnAniState.Idle;
    }

    public void UpdateState()
    {

    }
    public void AdjustUpdate()
    {
    }

    public void ExitState()
    {
    }
}
