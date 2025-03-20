using Cysharp.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "SOSkillAffectData", menuName = "Data/SkillAffect", order = 1)]
public class SOSkillAffectData : SkillEffectBase
{
    public string title;

    public override UniTask ApplyEffect(Unit caster, Unit target)
    {
        throw new System.NotImplementedException();
    }

  
}
