using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// 기본 스킬 클래스
public class Skill
{
    public SOSkillData data { get; private set; }
    public Sprite Icon { get => data.icon; }
    public string Name { get => data.name; }
    public string Desc { get => data.desc; }
    public float CoolTime { get => reducedCoolTime; }
    public float BaseCoolTime => data.coolTime;
    public int TableNum => data.tableNum;

    public List<SkillEffectBase> AffectList { get; } = new ();
    public float LastRunTime { get; set; }
    public bool IsBaseSkill { get; set; }
    float reducedCoolTime = 0f;

    private Stat _stat;
    public Skill(SOSkillData skillData, Stat stat)
    {
        data = skillData;
        LastRunTime = -1000f;
        _stat = stat;
        _stat.SetActionOnChangeValue(CulcalateCoolTime);
        CulcalateCoolTime();
      
    }

    public void CulcalateCoolTime()
    {
        if (IsBaseSkill)
            reducedCoolTime = BaseCoolTime / (1f + _stat.GetBaseSkillCooldown());
        else
            reducedCoolTime = BaseCoolTime / (1f + _stat.GetSkillCooldown());
    }

    public float GetCulcalatePercentCoolTime()
    {
        return 1f - Mathf.Clamp(((Time.time - LastRunTime) / reducedCoolTime), 0f, 1f);
    }

    /// <summary>
    /// 스킬이 실행 가능한지 체크
    /// </summary>
    /// <param name="attacker"></param>
    /// <returns>실행 가능하면 자원을 소모하고 true 반환</returns>
    public bool ReadySkill(IStat attacker)
    {
        if (IsReady(attacker.Mana))
        {
            LastRunTime = Time.time;
            attacker.Mana -= data.requireMana;
            return true;
        }
        return false;
    }

    public bool IsReady(float mana)
    {
        return !IsCooltime() && IsUseableMana(mana);
    }

    public bool IsCooltime()
    {
        bool isCooltime = Time.time < LastRunTime + reducedCoolTime;

        return isCooltime;
    }

    public bool IsUseableMana(float mana)
    {
        return mana >= data.requireMana; 
    }

    public float RemainCoolTime()
    {
        return CoolTime - (Time.time - LastRunTime);
    }

    public void ResetCoolTime()
    {
        LastRunTime = -1000f;
    }

    public async UniTask<bool> Cast(IDamageable caster)
    {
        SOSkillData skillData = data;
        List<UniTask> skillTasks = new List<UniTask>();

        foreach (var skill in skillData.skills)
        {
            List<IDamageable> targets = skill.targeting.GetTargets(caster); // 타겟 스냅샷 저장

            // 애니메이션 시작 및 대기
            //UniTask animationTask = caster.PlaySkillAnimation(skill); // 애니메이션 시작 (예시)
            //await animationTask; // 애니메이션이 끝나고 실행

            // 스킬 실행 시점에 타겟이 유효한지 다시 확인
            List<IDamageable> validTargets = targets.Where(t => !t.IsDead()).ToList();

            if (validTargets.Count == 0)
            {
                // 타겟이 없으면 실패 처리 (예: 마나 소모 적용 X)
                return false;
            }

            foreach (IDamageable target in validTargets)
            {
                foreach (var effect in skill.effects)
                {
                    skillTasks.Add(effect.ApplyEffect(caster, target));
                }
            }
        }

        await UniTask.WhenAll(skillTasks); // 모든 스킬 효과가 완료될 때까지 대기
        return true;
    }
}
