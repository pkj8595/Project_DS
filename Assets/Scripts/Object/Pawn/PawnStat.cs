using Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PawnStat : Stat
{
    /// <summary>
    /// 전투 스탯
    /// </summary>
    [SerializeField]
    private CombatStat _combatStat;
    /// <summary>
    /// 캐릭터 스탯 
    /// </summary>
    [SerializeField]
    private BaseStat _currentBaseStat;

    private Data.StatData _baseStat;
    private readonly List<Data.StatData> _runeStatList = new List<Data.StatData>(Define.Rune_Count);
    private readonly List<Data.StatData> _propertyStatList = new List<Data.StatData>(Define.Trait_Count);
    private int LevelUpCount = 0;
    private Define.EAttributeType ignoreAttributeType;

    [field: SerializeField] public int KillCount { get; set; }
    [field: SerializeField] public int WaveCount { get; set; }
    public override float MaxHp { get => _combatStat.maxHp; }
    public override float MaxMana { get => _combatStat.maxMana; }
    public override float Protection { get => _combatStat.protection;}
    public int EXP { get { return KillCount + (WaveCount * 10) + (-LevelUpCount * 100); }}
    public CombatStat CombatStat { get => _combatStat; set => _combatStat = value; }
    public BaseStat CurrentBaseStat { get => _currentBaseStat; set => _currentBaseStat = value; }
    public List<StatData> PropertyStatList => _propertyStatList;
    public float MoveSpeed { get 
        {
            return 2f * (1f + (_combatStat.movementSpeed));
        }
    }


    public override void Init(int statDataNum, System.Action onDead, System.Action onDeadTarget, System.Action onChangeStatValue)
    {
        base.Init(statDataNum, onDead, onDeadTarget, onChangeStatValue);

        SetBaseStat(Managers.Data.StatDict[statDataNum]);
        CalculateCombatStat();
        Hp = _combatStat.maxHp;
        Mana = 0;
    }

    public void Init(int statDataNum, System.Action onDead, System.Action onDeadTarget, System.Action onChangeStatValue, Define.EAttributeType eAttributeType)
    {
        Init(statDataNum, onDead, onDeadTarget, onChangeStatValue);
        ignoreAttributeType = eAttributeType;
    }


    public void ClearPropertyAndRune()
    {
        _propertyStatList.Clear();
        _runeStatList.Clear();
        CalculateCombatStat();
    }


    #region ChangeStat
    private void SetBaseStat(Data.StatData statData)
    {
        _baseStat = statData;
        CalculateCombatStat();
    }

    public void AddRuneStat(Data.StatData statData,int index)
    {
        if (_runeStatList.Count < Define.Pawn_Rune_Limit_Count)
        {
            for (int i = _runeStatList.Count; i < Define.Pawn_Rune_Limit_Count; i++)
            {
                _runeStatList.Add(null);
            }
        }

        if (index < Define.Pawn_Rune_Limit_Count && index < _runeStatList.Count)
        {
            _runeStatList[index] = statData;
            CalculateCombatStat();
        }
    }

    public void RemoveRuneStat(int index)
    {
        if (index < _runeStatList.Count)
        {
            _runeStatList[index] = null;
            CalculateCombatStat();
        }
        else
            Debug.Log($"index : {index} _runeStatList.Count : {_runeStatList.Count}");
    }

    public void RemoveRuneStat(RuneData data)
    {
        if (data.statTableNum != 0 && _runeStatList.Remove(Managers.Data.StatDict[data.statTableNum]))
        {
            CalculateCombatStat();
        }
    }

    public void AddPropertyStat(Data.StatData statData)
    {
        if (_propertyStatList.Count < Define.Trait_Count)
        {
            _propertyStatList.Add(statData);
        }
        else
        {
            for (int i = 0; i < _propertyStatList.Count; i++)
            {
                if (_propertyStatList[i] == null)
                {
                    _propertyStatList[i] = statData;
                }
            }
        }

        CalculateCombatStat();
    }

    public void RemovePropertyStat(Data.StatData statData)
    {
        if (_propertyStatList.Remove(statData))
        {
            CalculateCombatStat();
        }
    }

    private void CalculateCombatStat()
    {
        _currentBaseStat.Reset();
        _currentBaseStat += _baseStat;
        for (int i = 0; i < _runeStatList.Count; i++)
        {
            if (_runeStatList[i] != null)
                _currentBaseStat += _runeStatList[i];
        }
        for (int i = 0; i < _propertyStatList.Count; i++)
        {
            if (_propertyStatList[i] != null)
                _currentBaseStat += _propertyStatList[i];
        }
        _combatStat = CombatStat.ConvertStat(_currentBaseStat);
        OnChangeStatValue();
    }

    
    #endregion


    public override float GetAttackValue(Define.EDamageType damageType)
    {
        // ((Attack * 밸런스) * skill value) )  * (randomValue > 80) ? 1f : 1.5f
        float damageTypeValue = 0f;
        switch (damageType)
        {
            case Define.EDamageType.Melee:
                damageTypeValue = CombatStat.meleeDamage;
                break;
            case Define.EDamageType.Ranged:
                damageTypeValue = CombatStat.rangedDamage;
                break;
            case Define.EDamageType.Magic:
                damageTypeValue = CombatStat.magicDamage;
                break;
            default:
                Debug.LogError($"EDamageType.{damageType}이 케이스에 없습니다.");
                break;
        }
        float baseBalance = 50f;
        float balanceValue = Random.Range(baseBalance + (CombatStat.balance * 0.5f), 100f) * 0.01f;
        float ret = damageTypeValue * 
                    balanceValue * 
                    (Random.Range(0, 100) < CombatStat.criticalHitChance ? 1f : 1.5f);
        
        return ret;
    }

    public override void OnAttacked(float damageAmount, Stat attacker)
    {
        // 회피스탯 적용
        if (Random.Range(0, 1000) < _combatStat.dodgepChance)
        {
            string[] dodgeStr = {"느려", "운 좋게 피했다."};
            Managers.UI.ShowPawnDialog(transform, dodgeStr[Random.Range(0, dodgeStr.Length)]);
            return;
        }

        //todo effectManager damageNum
        float damage = Mathf.Max(0, CalculateDamage(damageAmount, _combatStat.protection));
        Hp -= damage;
        if (Hp < 0)
        {
            Hp = 0;
            OnDead(attacker);
        }
    }
    protected override void ApplyAffect(AffectBase[] affects, Stat attacker)
    {
        for (int i = 0; i < affects.Length; i++)
        {
            if (ignoreAttributeType == Define.EAttributeType.none || affects[i]?.AttributeType != ignoreAttributeType)
                affects[i]?.ApplyAffect(attacker, this);
        }
    }

    public override void OnDeadTarget()
    {
        base.OnDeadTarget();
        KillCount++;
    }

    public void IncreadHPMana()
    {
        Mana += _combatStat.manaRegeneration;
        Hp += _combatStat.hpRegeneration;
    }

    public void EndWaveEvent()
    {
        if (!IsDead)
        {
            WaveCount++;
            if (EXP >= 100)
            {
                LevelUpCount++;
                AddPositiveProperty();
            }
        }
    }

    public void AddPositiveProperty()
    {
        var data = Managers.Data.PositivePropertyList;
        AddPropertyStat(data[Random.Range(0, data.Count)]);
    }

    public void AddNagativeProperty()
    {
        var data = Managers.Data.NagativePropertyList;
        AddPropertyStat(data[Random.Range(0, data.Count)]);
    }

    public override float GetSkillCooldown()
    {
        return _combatStat.skillCooldown;
    }
    public override float GetBaseSkillCooldown()
    {
        return _combatStat.baseSkillCooldown;
    }

  
}
