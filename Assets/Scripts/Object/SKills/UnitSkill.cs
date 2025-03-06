using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitSkill
{
    public List<Skill> SkillList { get; private set; } = new(Define.Pawn_Rune_Limit_Count);
    private Skill _currentSkill;
    float? _manaAmount;
    public  bool IsRunning { get; private set; } = false;
    public void Init(float? manaAmount)
    {
        _manaAmount = manaAmount;
    }
    public void Release()
    {
        _manaAmount = null;
    }

    public void SetBaseSkill(Skill baseSkill)
    {
        baseSkill.IsBaseSkill = true;
        if (0 == SkillList.Count)
            SkillList.Add(baseSkill);
        else
            SkillList[0] = baseSkill;
    }

    public void SetSkill(Skill skill)
    {
        skill.IsBaseSkill = false;
        SkillList.Add(skill);
    }

    public void RemoveSkill(Skill skill)
    {
        SkillList.Remove(skill);
    }
    public void RemoveSkill(Data.RuneData data)
    {
        
        for (int i = 0; i < SkillList.Count; i++)
        {
            if (SkillList[i].TableNum == data.skillTableNum)
            {
                if (_currentSkill == SkillList[i]) 
                {
                    ClearCurrentSkill();
                }
                SkillList.RemoveAt(i);

                break;
            }
        }
    }

    public void ClearRuneSkill()
    {
        Skill baseSkill = SkillList[0];
        SkillList.Clear();
        SkillList.Add(baseSkill);
    }

    public Skill GetCurrentSkill()
    {
        if (_currentSkill != null && _currentSkill.IsReady(_manaAmount.Value))
            return _currentSkill;

        for (int i = 1; i < SkillList.Count; i++)
        {
            if (SkillList[i] != null && SkillList[i].IsReady(_manaAmount.Value))
            {
                _currentSkill = SkillList[i];
                return _currentSkill;
            }
        }
        _currentSkill = SkillList[0];
        return _currentSkill;
    }

    public Skill GetRunnigSkill()
    {
        return _currentSkill;
    }

    public bool ReadyCurrentSkill(IStat pawnStat)
    {
        bool isReady = GetCurrentSkill().ReadySkill(pawnStat);
        if (isReady)
            IsRunning = true;
        return isReady;
    }

    public void ClearCurrentSkill()
    {
        _currentSkill = null;
        IsRunning = false;
    }
    public Skill GetBaseSkill()
    {
        return SkillList[0];
    }

    public void EndWave()
    {
        for (int i = 0; i < SkillList.Count; i++)
        {
            SkillList[i].ResetCoolTime();
        }
    }
}
