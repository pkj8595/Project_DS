using Cysharp.Threading.Tasks;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "Damage", menuName = "Data/Skill/Affect/Damage", order = 1)]
public class SOSkillEffect_Damage : SkillEffectBase
{
    public int damagePer;
    public override UniTask ApplyEffect(IDamageable caster, IDamageable target)
    {
        //todo
        target.GetStat().Hp -= damagePer;
        return UniTask.CompletedTask;
    }

}
