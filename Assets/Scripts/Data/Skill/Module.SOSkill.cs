using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

public abstract class SkillEffectBase : SOSkillModuleBase
{
    public abstract UniTask ApplyEffect(Unit caster, Unit target);
}

public abstract class SkillTargetingBase : SOSkillModuleBase
{
    public abstract List<Unit> GetTargets(Unit caster);
}

public abstract class SkillVisualEffectBase : SOSkillModuleBase
{
    public abstract void PlayEffect(Unit caster, List<Unit> targets);
}

public abstract class SkillMotionControlBase : SOSkillModuleBase
{
    public abstract UniTask AwaitMotion(string motionStr);
}