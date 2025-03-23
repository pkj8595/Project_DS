using Cysharp.Threading.Tasks;
using NUnit.Framework;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "SOSkillData", menuName = "Data/Skill/SkillBase", order = 1)]
public class SOSkillData : SOData
{
    public string skillName;
    public string desc;
    public Sprite icon;
    public float coolTime;
    public float requireMana;
    public Define.UnitType unitType;
    [Space]
    [ShowIfEnumAttribute("unitType", (int)Define.UnitType.Pawn)]
    public Define.EPawnAniTriger _castringMotion;

    public Define.EPawnAniTriger _trigerMotion;

    

    //public SkillMotionControlBase castingMotion;
    //public SkillMotionControlBase trigerMotion;
    public SkillGroup[] skills;

}


[System.Serializable]
public class SkillGroup
{
    public SkillTargetingBase targeting;
    public List<SkillEffectBase> effects;
}
