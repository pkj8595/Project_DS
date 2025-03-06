using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public struct CombatStat
{
    public float maxHp;
    public float maxMana;
    public float meleeDamage;
    public float rangedDamage;
    public float magicDamage;
    public float protection;
    public float baseSkillCooldown;
    public float movementSpeed;
    public float criticalHitChance;
    public float dodgepChance;
    public float skillCooldown;
    public float statusEffectResistance;
    public float balance;
    public float hpRegeneration;
    public float manaRegeneration;
    public float searchRange;



    public static CombatStat operator *(float a, CombatStat b)
    {
        CombatStat ret = new CombatStat();
        ret.maxHp = a * b.maxHp;
        ret.maxMana = a * b.maxMana;
        ret.meleeDamage = a * b.meleeDamage;
        ret.rangedDamage = a * b.rangedDamage;
        ret.magicDamage = a * b.magicDamage;
        ret.protection = a * b.protection;
        ret.baseSkillCooldown = a * b.baseSkillCooldown;
        ret.movementSpeed = a * b.movementSpeed;
        ret.criticalHitChance = a * b.criticalHitChance;
        ret.dodgepChance = a * b.dodgepChance;
        ret.skillCooldown = a * b.skillCooldown;
        ret.statusEffectResistance = a * b.statusEffectResistance;
        ret.balance = a * b.balance;
        ret.hpRegeneration = a * b.hpRegeneration;
        ret.manaRegeneration = a * b.manaRegeneration;
        ret.searchRange = a * b.searchRange;

        return ret;
    }

    public static CombatStat operator +(CombatStat a, CombatStat b)
    {
        CombatStat ret = new CombatStat();
        ret.maxHp = a.maxHp + b.maxHp;
        ret.maxMana = a.maxMana + b.maxMana;
        ret.meleeDamage = a.meleeDamage + b.meleeDamage;
        ret.rangedDamage = a.rangedDamage + b.rangedDamage;
        ret.magicDamage = a.magicDamage + b.magicDamage;
        ret.protection = a.protection + b.protection;
        ret.baseSkillCooldown = a.baseSkillCooldown + b.baseSkillCooldown;
        ret.movementSpeed = a.movementSpeed + b.movementSpeed;
        ret.criticalHitChance = a.criticalHitChance + b.criticalHitChance;
        ret.dodgepChance = a.dodgepChance + b.dodgepChance;
        ret.skillCooldown = a.skillCooldown + b.skillCooldown;
        ret.statusEffectResistance = a.statusEffectResistance + b.statusEffectResistance;
        ret.balance = a.balance + b.balance;
        ret.hpRegeneration = a.hpRegeneration + b.hpRegeneration;
        ret.manaRegeneration = a.manaRegeneration + b.manaRegeneration;
        ret.searchRange = a.searchRange + b.searchRange;
        return ret;
    }

    public static CombatStat ConvertStat(Data.StatData statData)
    {
        CombatStat ret = new();

        ret += statData.vitality * Managers.Data.StatConversionDict[103000000];
        ret += statData.strength * Managers.Data.StatConversionDict[103000001];
        ret += statData.agility * Managers.Data.StatConversionDict[103000002];
        ret += statData.intelligence * Managers.Data.StatConversionDict[103000003];
        ret += statData.willpower * Managers.Data.StatConversionDict[103000004];
        ret += statData.accuracy * Managers.Data.StatConversionDict[103000005];

        return ret;
    }

    public static CombatStat ConvertStat(BaseStat statData)
    {
        CombatStat ret = new();

        ret += statData.vitality * Managers.Data.StatConversionDict[103000000];
        ret += statData.strength * Managers.Data.StatConversionDict[103000001];
        ret += statData.agility * Managers.Data.StatConversionDict[103000002];
        ret += statData.intelligence * Managers.Data.StatConversionDict[103000003];
        ret += statData.willpower * Managers.Data.StatConversionDict[103000004];
        ret += statData.accuracy * Managers.Data.StatConversionDict[103000005];

        return ret;
    }
}

[Serializable]
public struct BaseStat 
{
    /// <summary>
    /// 건강
    /// </summary>
    public int vitality;

    /// <summary>
    /// 힘
    /// </summary>
    public int strength;

    /// <summary>
    /// 민첩
    /// </summary>
    public int agility;

    /// <summary>
    /// 지력
    /// </summary>
    public int intelligence;

    /// <summary>
    /// 정신력
    /// </summary>
    public int willpower;

    /// <summary>
    /// 정확
    /// </summary>
    public int accuracy;

    public void Reset()
    {
        vitality = 0;
        strength = 0;
        agility = 0;
        intelligence = 0;
        willpower = 0;
        accuracy = 0;
    }

    public static BaseStat operator +(BaseStat a, Data.StatData b)
    {
        BaseStat ret = new BaseStat();
        ret.vitality = a.vitality + b.vitality;
        ret.strength = a.strength + b.strength;
        ret.agility = a.agility + b.agility;
        ret.intelligence = a.intelligence + b.intelligence;
        ret.willpower = a.willpower + b.willpower;
        ret.accuracy = a.accuracy + b.accuracy;
        return ret;
    }
    public static BaseStat operator +(Data.StatData a, BaseStat b)
    {
        BaseStat ret = new BaseStat();
        ret.vitality = a.vitality + b.vitality;
        ret.strength = a.strength + b.strength;
        ret.agility = a.agility + b.agility;
        ret.intelligence = a.intelligence + b.intelligence;
        ret.willpower = a.willpower + b.willpower;
        ret.accuracy = a.accuracy + b.accuracy;
        return ret;
    }
}