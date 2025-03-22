using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SkillTargeting", menuName = "Data/Skill/SkillTargeting", order = 1)]
public class SOSkillTargetingData : SkillTargetingBase
{
    public int testint;
    public override List<Unit> GetTargets(Unit caster)
    {
        throw new System.NotImplementedException();
    }
}
