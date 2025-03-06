using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class BuildingSkill : MonoBehaviour, IAttackable
{
    public Transform _shotTrans;
    public Animator _animator;
    public IDamageable LockTarget;
    public IDetectComponent DetectComponent;

    public UnitSkill Skills { get; protected set; } = new UnitSkill();
    private BuildingBase _buildingBase;
    private bool isInit = false;

    public float SearchRange => Skills.GetCurrentSkill().MaxRange;
    public Define.ETeam Team => _buildingBase.Team;

    public void Init(BuildingBase buildingBase)
    {
        _buildingBase = buildingBase;
        Skills.Init(_buildingBase.Stat.Mana);
        Skills.SetBaseSkill(new Skill( buildingBase.BuildingData.baseSkill, _buildingBase.Stat));
        DetectComponent = GetComponent<IDetectComponent>();
        isInit = true;
        StartSkillTask().Forget(); 
    }

    async UniTaskVoid StartSkillTask()
    {
        while (true)
        {
            Skill skill = Skills.GetCurrentSkill();
            if (isInit && skill.IsReady(_buildingBase.Stat.Mana))
            {
                Collider[] colliders = DetectComponent.DetectCollision();
                for (int i = 0; i < colliders.Length; i++)
                {
                    if (colliders[i] == null)
                        continue;

                    if (colliders[i].TryGetComponent(out IDamageable damageable))
                    {
                        if (SearchTargetToRay(damageable, skill))
                        {
                            StartSkill();
                        }
                    }
                }
            }

            await UniTask.Delay(200, cancellationToken: gameObject.GetCancellationTokenOnDestroy());
        }
    }

    private void StartSkill()
    {
        if(_animator != null)
        {
            _animator.Play("Attack");
        }
        else
        {
            if (Skills.ReadyCurrentSkill(_buildingBase.Stat))
            {
                Skill skill = Skills.GetRunnigSkill();
                DamageMessage msg = new DamageMessage(_buildingBase.Stat,
                                                      Vector3.zero,
                                                      Vector3.zero,
                                                      skill);

                ProjectileBase projectileBase = skill.MakeProjectile();
                projectileBase.Init(_shotTrans.transform,
                                    LockTarget.GetTransform(),
                                    Skills.GetRunnigSkill().SplashRange,
                                    msg);
                EndSkill();
            }
        }
    }

    /// <summary>
    /// animation callback 
    /// </summary>
    public void BegineSkill()
    {
        if (Skills.ReadyCurrentSkill(_buildingBase.Stat))
        {
            Skill skill = Skills.GetRunnigSkill();
            DamageMessage msg = new DamageMessage(_buildingBase.Stat,
                                                  Vector3.zero,
                                                  Vector3.zero,
                                                  skill);

            LockTarget.ApplyTakeDamage(msg);
        }
    }

    /// <summary>
    /// animation callback
    /// </summary>
    public void EndSkill()
    {
        _buildingBase.Stat.IncreadMana();
        Skills.ClearCurrentSkill();
    }

    /// <summary>
    /// 스킬에 맞는 타겟을 찾아서 ray에 맞고 그 개체가 죽지 않았다면 true
    /// </summary>
    /// <param name="other"></param>
    /// <param name="skill"></param>
    /// <returns></returns>
    private bool SearchTargetToRay(IDamageable other, Skill skill)
    {
        if (this.GetTargetType(other.Team) != skill.TargetType)
        {
            return false;
        }

        float skillRange = skill.MaxRange;
        int layer = (int)Define.Layer.Pawn;
        Vector3 directionEnumy = other.GetTransform().position - _shotTrans.position;
        if (Physics.Raycast(_shotTrans.position, directionEnumy, out RaycastHit hit, skillRange, layer)) 
        {
            if (hit.transform == other.GetTransform())
            {
                if (!other.IsDead())
                {
                    LockTarget = other;
                    return true;
                }
            }
        }
        return false;
    }

    public void EndWave()
    {
        Skills.EndWave();
    }


}
