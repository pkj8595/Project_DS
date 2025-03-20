using System.Collections.Generic;
using UnityEngine;

// 기본 스킬 클래스
public class Skill
{
    private SOSkillData data;
    public Sprite Icon { get => data.icon; }
    public string Name { get => data.name; }
    public string Desc { get => data.desc; }
    public float CoolTime { get => reducedCoolTime; }
    public float BaseCoolTime => data.coolTime;
    public int TableNum => data.tableNum;
    public Define.ETargetType TargetType;

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
            attacker.Mana -= data.requireMane;
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
        return mana >= data.requireMane; 
    }


    public float RemainCoolTime()
    {
        return CoolTime - (Time.time - LastRunTime);
    }

    public void ResetCoolTime()
    {
        LastRunTime = -1000f;
    }
}
