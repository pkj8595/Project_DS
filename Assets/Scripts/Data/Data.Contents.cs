using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Data
{
    [Serializable]
    public class TableBase
    {
        public int tableNum;
    }

    [Serializable]
    public class TableGroupData
    {
        public CharacterData[] CharacterTable;
        public StatData[] StatTable;
        public StatConversionData[] StatConversionTable;
        public WaveData[] WaveTable;
        public BuildingData[] BuildingTable;
        public GoodsData[] GoodsTable;
        public RuneData[] RuneTable;
        public SkillData[] SkillTable;
        public SkillAffectData[] SkillAffectTable;
        public BuffData[] BuffTable;
        public ProductionData[] ProductionTable;
        

        public void MakeTableData(Dictionary<int, CharacterData> tableData)
        {
            for(int i = 0; i < CharacterTable.Length; i++)
            {
                tableData.Add(CharacterTable[i].tableNum, CharacterTable[i]);
            }
        }

        public void MakeTableData(Dictionary<int, StatData> tableData, List<StatData> PositivePropertyList, List<StatData> NagativePropertyList)
        {
            for (int i = 0; i < StatTable.Length; i++)
            {
                if (Utils.CalculateCategory(StatTable[i].tableNum) == 300)
                    PositivePropertyList.Add(StatTable[i]);
                else if(Utils.CalculateCategory(StatTable[i].tableNum) == 301)
                    NagativePropertyList.Add(StatTable[i]);
                else
                    tableData.Add(StatTable[i].tableNum, StatTable[i]);

            }
        }

        public void MakeTableData(Dictionary<int, StatConversionData> tableData)
        {
            for (int i = 0; i < StatConversionTable.Length; i++)
            {
                tableData.Add(StatConversionTable[i].tableNum, StatConversionTable[i]);
            }
        }
        public void MakeTableData(Dictionary<int, WaveData> tableData)
        {
            for (int i = 0; i < WaveTable.Length; i++)
            {
                tableData.Add(WaveTable[i].tableNum, WaveTable[i]);
            }
        }


        public void MakeTableData(Dictionary<int, BuildingData> tableData)
        {
            for (int i = 0; i < BuildingTable.Length; i++)
            {
                tableData.Add(BuildingTable[i].tableNum, BuildingTable[i]);
            }
        }

        public void MakeTableData(Dictionary<int, GoodsData> tableData)
        {
            for (int i = 0; i < GoodsTable.Length; i++)
            {
                tableData.Add(GoodsTable[i].tableNum, GoodsTable[i]);
            }
        }

        public void MakeTableData(Dictionary<int, RuneData> tableData)
        {
            for (int i = 0; i < RuneTable.Length; i++)
            {
                tableData.Add(RuneTable[i].tableNum, RuneTable[i]);
            }
        }


        public void MakeTableData(Dictionary<int, SkillData> tableData)
        {
            for (int i = 0; i < SkillTable.Length; i++)
            {
                tableData.Add(SkillTable[i].tableNum, SkillTable[i]);
            }
        }

        public void MakeTableData(Dictionary<int, SkillAffectData> tableData)
        {
            for (int i = 0; i < SkillAffectTable.Length; i++)
            {
                tableData.Add(SkillAffectTable[i].tableNum, SkillAffectTable[i]);
            }
        }
        public void MakeTableData(Dictionary<int, BuffData> tableData)
        {
            for (int i = 0; i < BuffTable.Length; i++)
            {
                tableData.Add(BuffTable[i].tableNum, BuffTable[i]);
            }
        }

        public void MakeTableData(Dictionary<int, ProductionData> tableData)
        {
            for (int i = 0; i < ProductionTable.Length; i++)
            {
                tableData.Add(ProductionTable[i].tableNum, ProductionTable[i]);
            }
        }

    }
   

    #region 101
    [Serializable]
    public class CharacterData : TableBase
    {
        public const int Table = 101;

        public string name;
        public string desc;
        public int statDataNum;
        public int upgradeChar;
        public int upgradeRequire;
        public int upgradeRequireAmount;
        public Define.EAttributeType ignoreAttributeType;
        public int basicSkill;
        public int[] arr_rune = new int[3];
        public int waveCost;
        public string Head;
        public string Ears;
        public string Eyes;
        public string Body;
        public string Hair;
        public string Armor;
        public string Helmet;
        public string Weapon;
        public string Shield;
        public string Cape;
        public string Back;
        public string Mask;
        public string Horns;
    }
    #endregion


    #region 102 
    [Serializable]
    public class StatData : TableBase
    {
        public const int Table = 102;
        public string name;
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

        public static BaseStat operator +(StatData a, StatData b)
        {
            BaseStat ret = new BaseStat();
            ret.vitality    = a.vitality + b.vitality;
            ret.strength    = a.strength + b.strength;
            ret.agility     = a.agility + b.agility;
            ret.intelligence = a.intelligence + b.intelligence;
            ret.willpower   = a.willpower + b.willpower;
            ret.accuracy    = a.accuracy + b.accuracy;
            return ret;
        }
       
    }

    [Serializable]
    public class StatConversionData : TableBase
    {
        public const int Table = 103;

        public int type;
        public float maxHp;
        public float maxMana;
        public float meleeDamage;
        public float rangedDamage;
        public float magicDamage;
        public float protection;
        public float baseSkillCooldown;
        public float skillCooldown;
        public float movementSpeed;
        public float criticalHitChance;
        public float dodgepChance;
        public float statusEffectResistance;
        public float balance;
        public float hpRegeneration;
        public float manaRegeneration;
        public float searchRange;

        public static CombatStat operator *(float a, StatConversionData b)
        {
            CombatStat ret = new CombatStat();
            ret.maxHp                   = a * b.maxHp;
            ret.maxMana                 = a * b.maxMana;
            ret.meleeDamage             = a * b.meleeDamage;
            ret.rangedDamage            = a * b.rangedDamage;
            ret.magicDamage             = a * b.magicDamage;
            ret.protection              = a * b.protection;
            ret.baseSkillCooldown       = a * b.baseSkillCooldown;
            ret.movementSpeed           = a * b.movementSpeed;
            ret.criticalHitChance       = a * b.criticalHitChance;
            ret.dodgepChance            = a * b.dodgepChance;
            ret.skillCooldown           = a * b.skillCooldown;
            ret.statusEffectResistance  = a * b.statusEffectResistance;
            ret.balance                 = a * b.balance;
            ret.hpRegeneration          = a * b.hpRegeneration;
            ret.manaRegeneration        = a * b.manaRegeneration;
            ret.searchRange             = a * b.searchRange;

            return ret;
        }

    }
    #endregion
    [Serializable]
    public class WaveData : TableBase
    {
        public const int Table = 110;

        public string name;
        public string desc;
        public int difficulty;
        public int[] arr_characterNum = new int[5];
        public int[] arr_charAmount = new int[5];
    }

 

    #region 202
    [Serializable]
    public class BuildingData : TableBase
    {
        public const int Table = 202;

        public string name;
        public string desc;
        public string prefab;
        public bool isDamageable;
        public float maxHp;
        public float maxMana;
        public float damageValue;
        public float protection;
        public float manaRegeneration;
        public int popluation;
        public int baseSkill;
        public int productionTable;
        public int upgradeNum;
        public int upgrade_goods;
        public int upgrade_goods_amount;
        public int waveCost;
    }
    #endregion

    #region 301
    [Serializable]
    public class GoodsData : TableBase
    {
        public const int Table = 301;

        public string name;
        public string desc;
        public string imageStr;
    }
    #endregion

    #region 302
    [Serializable]
    public class RuneData : TableBase
    {
        public const int Table = 302;

        public string name;
        public string desc;
        public string imageStr;
        public int statTableNum;
        public int skillTableNum;
        public int releaseGoods;
        public int releaseAmount;
    }
    #endregion


    [Serializable]
    public class SkillData : TableBase
    {
        public const int Table = 400;

        public string name;
        public string desc;
        public string icon;
        public Define.ESkillType skillType;
        public Define.ETargetType targetType;
        public float manaAmount;
        public float minRange;
        public float maxRange;
        public float splashRange;
        public float coolTime;
        public string effectStr;
        public Define.EPawnAniTriger aniTriger;
        public Define.EPawnAniState motionAni;
        public float motionDuration;
        public string projectile;
        public int[] arr_affect = new int[3];
    }

    [Serializable]
    public class SkillAffectData : TableBase
    {
        public const int Table = 401;

        public Define.EAffectType affectType;
        public Define.EDamageType damageType;
        public Define.ETargetType targetType;
        public Define.EAttributeType attributeType;
        public string effectStr;
        public int value;
    }

    [Serializable]
    public class BuffData : TableBase
    {
        public const int Table = 402;

        public string name;
        public string desc;
        public string icon;
        public int duration;
        public int statTable;
    }

    [Serializable]
    public class ProductionData : TableBase
    {
        public const int Table = 500;

        public string name;
        public string desc;
        public int itemNum;
        public int itemAmount;
        public int waveCount;
    }
}