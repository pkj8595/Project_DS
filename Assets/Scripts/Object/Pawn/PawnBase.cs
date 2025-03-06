using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Cysharp.Threading.Tasks;

public abstract class PawnBase :Unit, ISelectedable, IDamageable, IAttackable, IWaveEvent, ICommand
{
    //data
    public Data.CharacterData CharacterData { get; private set; }

    //설정
    public Define.WorldObject WorldObjectType { get; set; } = Define.WorldObject.Pawn;
    [field: SerializeField] public Define.ETeam Team { get; set; } = Define.ETeam.Playable;
    [field : SerializeField] protected Vector3 DestPos { get; set; }
    [field : SerializeField] public Vector3 OriginPosition { get; set; }
    [SerializeField] protected Define.EPawnAniState _state = Define.EPawnAniState.Idle;
    [SerializeField] protected Transform _projectileTrans;

    //pawn 기능
    [HideInInspector] public NavMeshAgent _navAgent;
    [field : SerializeField] public PawnAnimationController AniController { get; private set; }


    //룬 && 기벽
    [field : SerializeField] public List<Data.RuneData> RuneList { get; private set; } = new(Define.Pawn_Rune_Limit_Count);
    public PawnStat PawnStat { get; protected set; }
    public UnitSkill PawnSkills { get; } = new UnitSkill();
    public UnitAI AI { get; private set; }

    //option
    [field : SerializeField] public IDamageable LockTarget { get; set; }
    public bool HasTarget => LockTarget != null && !LockTarget.IsDead();
    public float SearchRange { get => PawnStat.CombatStat.searchRange; }
    public float LastCombatTime { get; set; } = 0f;
    public Action OnDeadAction { get; set; }
    public Vector3 StateBarOffset => Vector3.up * 1.2f;

    //component
    [SerializeField] private Collider _collider;
    [field: SerializeField] public string AIStateStr { get; set; }
    [field: SerializeField] public SkillRangeView SkillRangeView { get; set; }

    public bool IsSelected { get; set; }
    public virtual Define.EPawnAniState State
    {
        get { return _state; }
        set
        {
            _state = value;
            if (_state == Define.EPawnAniState.Ready || _state == Define.EPawnAniState.Idle)
            {
                if (Time.time < LastCombatTime + 5f)
                    _state = Define.EPawnAniState.Ready;
                else
                    _state = Define.EPawnAniState.Idle;
            }
            
            AniController.SetAniState(_state);
        }
    }

    public bool CanMove { get; } = true;

    public virtual void SetTriggerAni(Define.EPawnAniTriger trigger)
    {
        AniController.SetAniTrigger(trigger);
    }

    public virtual void Update()
    {
        AI?.OnUpdate();
    }

    private void OnDisable()
    {
        if (Team == Define.ETeam.Playable)
        {
            Managers.Game.RemoveWaveObject(this);
            Managers.Game.Inven.CurrentPopulation--;
        }
    }

    public virtual void Init(int characterNum, bool isUpgrade = false)
    {
        //component setting
        if (AniController == null)
            AniController = gameObject.GetComponentInChildren<PawnAnimationController>();

        if (PawnStat == null)
            PawnStat = gameObject.GetOrAddComponent<PawnStat>();

        if (_navAgent == null)
        {
            _navAgent.GetComponent<NavMeshAgent>();
            _navAgent.updateRotation = false;
            _navAgent.updateUpAxis = false;
        }

        if (_collider == null)
        {
            _collider = GetComponent<Collider>();
        }

        //table data setting
        CharacterData = Managers.Data.CharacterDict[characterNum];
        AniController.Init(CharacterData);
        PawnStat.Init(CharacterData.statDataNum, OnDead, OnDeadTarget, OnChangeStatValue, CharacterData.ignoreAttributeType);
        AI ??= new UnitAI();
        AI.Init(this);
        
        PawnSkills.Init(PawnStat.Mana);
        PawnSkills.SetBaseSkill(new Skill(CharacterData.basicSkill,PawnStat));

        //초기화 데이터 리셋
        if (!isUpgrade)
        {
            ResetStatData();
            for (int i = 0; i < 2; i++)
            {
                float randomRange = UnityEngine.Random.value;
                if (randomRange < 0.5f)
                    PawnStat.AddNagativeProperty();
                else
                    PawnStat.AddPositiveProperty();
            }
        }
        
        //룬 데이터 등록
        for (int i = 0; i < CharacterData.arr_rune.Length; i++)
        {
            if (CharacterData.arr_rune[i] != 0)
                SetRuneData(Managers.Data.RuneDict[CharacterData.arr_rune[i]], i);
        }

        //component data setting
        _collider.enabled = true;
        _navAgent.enabled = true;
        _navAgent.speed = PawnStat.MoveSpeed;

        //stateBar setting
        UIStateBarGroup uiStatebarGroup = Managers.UI.ShowUI<UIStateBarGroup>() as UIStateBarGroup;
        uiStatebarGroup.AddUnit(this);

        if (Team == Define.ETeam.Playable)
        {
            Managers.Game.RegisterWaveObject(this);
        }

        Managers.Game.Inven.CurrentPopulation++;

        Init();
    }

    public void Init(int tableNum, Define.ETeam team)
    {
        Team = team;
        Init(tableNum);
        if (team == Define.ETeam.Enemy)
        {
            EnemyInit();
        }
        else
        {
            EnemyFindTarget enemyFindTarget = gameObject.GetComponent<EnemyFindTarget>();
            if(enemyFindTarget != null)
            {
                enemyFindTarget.enabled = false;
            }
        }
    }

    private void EnemyInit() 
    {
        EnemyFindTarget enemyFindTarget = gameObject.GetOrAddComponent<EnemyFindTarget>();
        enemyFindTarget.Init();
    }

    private void ChangeTeam()
    {
        Team = Define.ETeam.Playable;
        EnemyFindTarget enemyComponent = GetComponent<EnemyFindTarget>();
        if (enemyComponent != null)
            Destroy(enemyComponent);
    }

    protected virtual void Init() { }

    public void UpdateMove()
    {
        switch (_navAgent.pathStatus)
        {
            case NavMeshPathStatus.PathComplete:
                //Debug.Log("경로가 완전히 생성.");
                break;
            case NavMeshPathStatus.PathPartial:
                Debug.Log("일부만 생성.");
                if (_navAgent.path.corners != null)
                    DestPos = _navAgent.path.corners[_navAgent.path.corners.Length - 1];
                break;
            case NavMeshPathStatus.PathInvalid:
                //Debug.Log("유효하지 않는 경로");
                _navAgent.isStopped = true;
                _navAgent.ResetPath();
                State = Define.EPawnAniState.Idle;
                return;
        }

        //naviAgent가 이동을 마쳤을 경우 idle로 돌아감
        if (_navAgent.velocity == Vector3.zero && Vector3.Distance(DestPos, transform.position) < 0.2f)
        {
            //State = _lockTarget == null ? Define.EPawnAniState.Idle : Define.EPawnAniState.Ready;
            AI.SetState(AI.GetIdleState());
        }


        if (_navAgent.velocity.sqrMagnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(_navAgent.velocity.normalized);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 1f);
        }
    }


#if UNITY_EDITOR
    public void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(gameObject.transform.position, gameObject.transform.forward);
    }

#endif

    #region combat system
    /// <summary>
    /// 피격시 실행되는 함수 전달받은 매개변수로 캐릭터 데이터 갱신
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    public  bool ApplyTakeDamage(DamageMessage message)
    {
        SetTriggerAni(Define.EPawnAniTriger.Hit);
        PawnStat.ApplyDamageMessage(ref message);
        LastCombatTime = Time.time;

        //공격 당했을때 공격의 대상이 공격 범위내의 적이라면 target를 수정 
        if (message.attacker != null && message.attacker.TryGetComponent(out IDamageable attacker))
        {
            //때린 대상이 적이고 죽지 않았다면
            if (attacker.GetTargetType(Team) == Define.ETargetType.Enemy &&
                !attacker.IsDead())
            {
                //타겟이 없다면
                if (!HasTarget)
                {
                    LockTarget = attacker;
                }
                //타겟이 있는데 범위 내에 없다면 타겟을 변경
                else if (!(PawnSkills.GetCurrentSkill().MaxRange > 
                           Vector3.Distance(LockTarget.GetTransform().position, transform.position)))
                {
                    LockTarget = attacker;
                }
            }
        }
        return false;
    }


    /// <summary>
    /// 추적 대상이 있을때 이동 및 공격
    /// </summary>
    public void TrackingAndAttackTarget()
    {
        //collider에서 현재 캐릭터의 위치를 계산
        Vector3 targetPosition;
        Collider targetColl = LockTarget.GetCollider();
        if (targetColl == null)
        {
            targetPosition = LockTarget.GetTransform().position;
        }
        else
        {
            targetPosition = targetColl.ClosestPoint(transform.position);
        }
        float targetDistance = Vector3.Distance(transform.position, targetPosition);

        //distanceType 계산
        Define.ESkillDistanceType distanceType = PawnSkills.GetCurrentSkill().
            IsExecuteableRange(targetDistance);
       
        switch (distanceType)
        {
            case Define.ESkillDistanceType.LessMin:
                //방향 벡터를 구해서 target의 반대 방향으로 이동한다.
                Vector3 fleeDirection = transform.position - LockTarget.GetTransform().position;
                SetDestination(fleeDirection.normalized * 
                    (PawnSkills.GetCurrentSkill().MinRange + PawnSkills.GetCurrentSkill().MinRange) * 0.5f);
                break;

            case Define.ESkillDistanceType.Excuteable:
                //스킬이 실행 가능한지 체크 및 자원 소모
                if (PawnSkills.ReadyCurrentSkill(PawnStat))
                {
                    Skill skill = PawnSkills.GetRunnigSkill();
                    if (!skill.IsBaseSkill)
                    {
                        Managers.UI.ShowPawnDialog(transform, skill.Name);
                    }

                    AI.SetState(AI.GetSkillState());
                    transform.LookAt(LockTarget.GetTransform());
                    //모션 시간이 있다면 unitask 실행
                    if (skill.MotionDuration > 0)
                    {
                        TaskExecuteSkill(skill).Forget();
                    }
                    else
                    {
                        AniController.SetAniTrigger(skill.AniTriger);
                    }
                }
                break;

            case Define.ESkillDistanceType.MoreMax:
                SetDestination(targetPosition);
                break;
        }
    }

    private async UniTaskVoid TaskExecuteSkill(Skill skill)
    {
        State = skill.MotionAni;
        await UniTask.Delay((int)(skill.MotionDuration * 1000));
        if (!IsDead())
            AniController.SetAniTrigger(skill.AniTriger);
    }

    public void SetRuneData(Data.RuneData data, int index)
    {
        for (int i = RuneList.Count; i < Define.Pawn_Rune_Limit_Count; i++)
        {
            RuneList.Add(null);
        }

        if (index < Define.Pawn_Rune_Limit_Count && index < RuneList.Count)
        {
            RuneList[index] = data;
            if (data.statTableNum != 0)
            {
                PawnStat.AddRuneStat(Managers.Data.StatDict[data.statTableNum], index);
            }
            if (data.skillTableNum != 0)
            {
                PawnSkills.SetSkill(new Skill(data.skillTableNum,PawnStat));
            }
            
        }
        else
        {
            Debug.LogError($"인덱스의 크기가 범위를 넘었습니다.. : {index}");
        }

    }

    public void RemoveRuneData(Data.RuneData data)
    {
        RuneList.Remove(data);
        PawnStat.RemoveRuneStat(data);
        PawnSkills.RemoveSkill(data);
    }

    public void RemoveRuneData(int index)
    {
        RemoveRuneData(RuneList[index]);
    }

    public void ResetStatData()
    {
        for (int i = 0; i < RuneList.Count; i++)
        {
            RuneList[i] = null;
        }
        PawnStat.ClearPropertyAndRune();
        PawnSkills.ClearRuneSkill();
    }


    /// <summary>
    /// 타격타이밍에 실행 -> animation에서 OnHitEvent 호출시 
    /// </summary>
    public virtual void BegineAniAttack()
    {
        //projectile 발사
        Skill skill = PawnSkills.GetRunnigSkill();
        DamageMessage msg = new DamageMessage(PawnStat,
                                           Vector3.zero,
                                           Vector3.zero,
                                           skill);
        if (skill.DetectTargetInSkillRange(this, out List<IDamageable> unitList))
        {
            ProjectileBase projectile = skill.MakeProjectile();

            if (projectile == null)
            {
                for (int i = 0; i < unitList.Count; i++)
                {
                    unitList[i].ApplyTakeDamage(msg);
                }
            }
            else
            {
                projectile.Init(_projectileTrans, unitList[0].GetTransform(), skill.SplashRange, msg);
            }
        }
    }

    /// <summary>
    ///  Animation Event 함수 실행시
    /// </summary>
    public virtual void EndAniAttack()
    {
        PawnSkills.ClearCurrentSkill();
        LastCombatTime = Time.time;
        PawnStat.IncreadHPMana();
        AI.SetState(AI.GetIdleState());
    }
    #endregion

    /// <summary>
    /// pawn이 죽었을 때 실행되는 함수
    /// </summary>
    private void OnDead()
    {
        OnDeadAction?.Invoke();
        AI.SetState(AI.GetDeadState());
        UIStateBarGroup uiStatebarGroup = Managers.UI.GetUI<UIStateBarGroup>() as UIStateBarGroup;
        uiStatebarGroup.SetActive(this, false);
        _navAgent.enabled = false;
        _collider.enabled = false;
        OnDeadEnemy().Forget();

        //사용하고 있는 pawn이 죽으면 50프로 확률로 부정기벽 획득
        if (Team == Define.ETeam.Playable)
        {
            float randomRange = UnityEngine.Random.value;
            if (randomRange < 0.5f)
            {
                PawnStat.AddNagativeProperty();
            }
        }
    }

    private void OnChangeStatValue()
    {
        _navAgent.speed = PawnStat.MoveSpeed;
    }

    async UniTaskVoid OnDeadEnemy()
    {
        await UniTask.Delay(3000);
        if (Team == Define.ETeam.Enemy)
        {
            Managers.Game.Despawn(this);
            //Destroy(GetComponent<EnemyFindTarget>());
            Managers.Resource.Destroy(this.gameObject);
        }
    }


    private void OnLive()
    {
        if (IsDead())
        {
            PawnStat.OnLive();
            AI.SetState(AI.GetIdleState());
            UIStateBarGroup uiStatebarGroup = Managers.UI.GetUI<UIStateBarGroup>() as UIStateBarGroup;
            uiStatebarGroup.SetActive(this, true);
            _navAgent.enabled = true;
            _collider.enabled = true;
        }
    }

    private void OnDeadTarget()
    {
        LockTarget = IAttackable.SearchTarget(this, SearchRange, PawnSkills.GetCurrentSkill().TargetType);
        if (LockTarget == null)
        {
            AI.SetState(AI.GetIdleState());
        }
    }

    public void SetDestination(Vector3 position, bool isChase = true)
    {
        if (DestPos == position)
            return;
        if (BoardManager.Instance.GetMoveablePosition(position, out Vector3 moveablePosition))
        {
            OnMove(moveablePosition, isChase);
        }
    }

    protected virtual void OnMove(Vector3 destPosition, bool isChase)
    {
        if (DestPos == destPosition)
            return;
        DestPos = destPosition;
        if (!_navAgent.isActiveAndEnabled)
        {
            Debug.LogError($"_navAgent.isActiveAndEnabled : {_navAgent.isActiveAndEnabled}");
            Debug.LogError(System.Environment.StackTrace);
        }
        _navAgent.SetDestination(DestPos);
        if (isChase)
            AI.SetState(AI.GetChaseState());
        else
            AI.SetState(AI.GetMoveState());
    }


    /// <summary>
    /// 길찾기 종료
    /// </summary>
    public void OnMoveStop()
    {
        DestPos = gameObject.transform.position;
        _navAgent.ResetPath();
    }

    public override bool UpgradeUnit()
    {
        if (CharacterData.upgradeChar == 0 || Team == Define.ETeam.Enemy)
        {
            Managers.UI.ShowToastMessage("적은 업그레이드 할 수 없습니다.");
            return false;
        }

        if (Managers.Game.Inven.SpendItem(CharacterData.upgradeRequire, CharacterData.upgradeRequireAmount))
        {
            Init(CharacterData.upgradeChar);
            return true;
        }

        Managers.UI.ShowToastMessage("업그레이드 비용이 부족합니다.");
        return false;
    }

    public Sprite GetIdleSprite()
    {
        return AniController.GetIdleSprite();
    }

    public virtual void OnSelect()
    {
        IsSelected = true;
        UIData data = new UIUnitData { unitGameObject = this };
        Managers.UI.ShowUIPopup<UIUnitPopup>(data);

        Skill skill = PawnSkills.GetCurrentSkill();
        SkillRangeView.SetRange(skill.MinRange, skill.MaxRange);
        SkillRangeView.SetActive(true);
    }

    public virtual void OnDeSelect()
    {
        IsSelected = false;
        SkillRangeView.SetActive(false);
    }


    public Transform GetTransform()
    {
        return transform;
    }

    public bool IsDead()
    {
        return PawnStat.IsDead;
    }

    public IStat GetStat()
    {
        return PawnStat;
    }

    public void EndWave()
    {
        PawnStat.EndWaveEvent();
        PawnSkills.EndWave();

        Managers.Game.Inven.SpendMoveItem(transform, StateBarOffset, Define.EGoodsType.food, CharacterData.waveCost, (isSpend) =>
        {
            if (isSpend)
                PawnStat.SpendCost();
            else
                PawnStat.DontSpendCost();
        });
    }

    public void ReadyWave()
    {
        //OnLive();
        PawnStat.Mana = 0;
    }

    public Collider GetCollider()
    {
        return _collider;
    }

    public void CommandMoveTo(Vector3 targetPosition)
    {
        OriginPosition = targetPosition;
        SetDestination(targetPosition, false);
    }
}
