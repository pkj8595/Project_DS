using Cysharp.Threading.Tasks;
using UnityEngine;

public interface ISkillMotion 
{
    public bool IsPlaying { get; }
    public UniTask RunAnimation(ESkillMotionTriger trigerm, float delay);
}

public enum ESkillMotionTriger
{
    None,
    Cast,
    MeleeAttack,
    RangedAttack,
}