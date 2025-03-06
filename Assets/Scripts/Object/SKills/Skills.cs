using System.Collections.Generic;
using UnityEngine;

// 기본 스킬 클래스
public class Skill
{
    private Data.SkillData data;
    public string Icon { get => data.icon; }
    public string Name { get => data.name; }
    public string Desc { get => data.desc; }
    public Define.ESkillType SkillType { get => data.skillType; }
    public Define.ETargetType TargetType { get => data.targetType; }
    public float ManaAmount { get => data.manaAmount; }
    public float MinRange { get => data.minRange; }
    public float MaxRange { get => data.maxRange; }
    public float SplashRange { get => data.splashRange; }
    public float CoolTime { get => reducedCoolTime; }
    public float BaseCoolTime => data.coolTime;
    public Define.EPawnAniState MotionAni { get => data.motionAni; }
    public Define.EPawnAniTriger AniTriger { get => data.aniTriger; }
    public float MotionDuration { get => data.motionDuration; }
    public string EffectStr { get => data.effectStr; }
    public string ProjectileStr { get => data.projectile; }
    public int TableNum => data.tableNum;

    public List<AffectBase> AffectList { get; } = new List<AffectBase>(Define.Affect_Count);
    public float LastRunTime { get; set; }
    public bool IsBaseSkill { get; set; }
    float reducedCoolTime = 0f;

    private Stat _stat;
    public Skill(int skillTableNum, Stat stat)
    {
        data = Managers.Data.SkillDict[skillTableNum];
        LastRunTime = -1000f;
        _stat = stat;
        _stat.SetActionOnChangeValue(CulcalateCoolTime);
        CulcalateCoolTime();
        for (int i = 0; i < data.arr_affect.Length; i++)
        {
            if (data.arr_affect[i] != 0)
            {
                Data.SkillAffectData affectData = Managers.Data.SkillAffectDict[data.arr_affect[i]];
                AffectList.Add(AffectFactory.CreateAffect(affectData));
            }
        }
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
            attacker.Mana -= ManaAmount;
            return true;
        }
        return false;
    }

    public bool DetectTargetInSkillRange(IDamageable obj, out List<IDamageable> units)
    {
        if (SkillType == Define.ESkillType.one)
        {
            units = new List<IDamageable>(1);
            int layer = (int)Define.Layer.Building | (int)Define.Layer.Pawn;
            var colliders = Physics.OverlapSphere(obj.GetTransform().position, MaxRange, layer);
            foreach (Collider coll in colliders)
            {
                //minRange보다 작으면 공격 범위에서 벗어남
                if (Vector3.Distance(obj.GetTransform().position, coll.transform.position) < MinRange)
                    continue;

                IDamageable unit =  coll.attachedRigidbody.GetComponent<IDamageable>();
                if (unit == null || unit.IsDead())
                    continue;

                if (unit.GetTargetType(obj.Team) == TargetType)
                {
                    units.Add(unit);
                    return true;
                }
            }
        }
        else /*if (SkillType == Define.ESkillType.Range)*/
        {
            units = new List<IDamageable>(10);
            int layer = (int)Define.Layer.Building | (int)Define.Layer.Pawn;
            var colliders = Physics.OverlapSphere(obj.GetTransform().position, MaxRange, layer);
            foreach (Collider coll in colliders)
            {
                IDamageable unit = coll.attachedRigidbody.GetComponent<IDamageable>();
                if (unit == null)
                    continue;

                if (unit.GetTargetType(obj.Team) == TargetType)
                {
                    units.Add(unit);
                }
            }
        }

        return units.Count > 0;
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
        return mana >= ManaAmount; 
    }

    public Define.ESkillDistanceType IsExecuteableRange(float distance)
    {
        if (distance < MinRange)
            return Define.ESkillDistanceType.LessMin;
        else if (distance > MaxRange)
            return Define.ESkillDistanceType.MoreMax;
        else
            return Define.ESkillDistanceType.Excuteable;
    }


    public ProjectileBase MakeProjectile(Transform parent = null)
    {
        if (string.IsNullOrEmpty(data.projectile))
            return null;

        if (parent == null)
            parent = GameView.Instance.GetParentObj(Define.EParentObj.Projectile).transform;

        var obj = Managers.Resource.Instantiate(Define.Path.Prefab_Bullet + data.projectile, parent);
        return obj.GetComponent<ProjectileBase>();
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
