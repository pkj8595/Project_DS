using Cysharp.Threading.Tasks;
using UnityEngine;

public class SOSkillEffect_Damage : SkillEffectBase
{
    public int damagePer;
    public override UniTask ApplyEffect(IDamageable caster, IDamageable target)
    {
        //target.ApplyTakeDamage(new DamageMessage(caster, target, damagePer));
        return UniTask.CompletedTask;
    }

}
