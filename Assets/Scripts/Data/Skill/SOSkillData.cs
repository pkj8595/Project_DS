using Cysharp.Threading.Tasks;
using NUnit.Framework;
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
    [Space]
    public SkillMotionControlBase castingMotion;
    public SkillMotionControlBase trigerMotion;
    public SkillGroup[] skills;

    public async UniTask Cast(Unit caster)
    {
        List<UniTask> skillTasks = new List<UniTask>();

        foreach (var skill in skills)
        {
            List<Unit> targets = skill.targeting.GetTargets(caster); // 타겟 스냅샷 저장

            // 애니메이션 시작 및 대기
            UniTask animationTask = caster.PlaySkillAnimation(skill); // 애니메이션 시작 (예시)

            await animationTask; // 애니메이션이 끝나고 실행

            // 스킬 실행 시점에 타겟이 유효한지 다시 확인
            List<Unit> validTargets = targets.Where(t => t.IsAlive).ToList();

            if (validTargets.Count == 0)
            {
                // 타겟이 없으면 실패 처리 (예: 마나 소모 적용 X)
                return;
            }

            foreach (Unit target in validTargets)
            {
                foreach (var effect in skill.effects)
                {
                    skillTasks.Add(effect.ApplyEffect(caster, target));
                }
            }
        }

        await UniTask.WhenAll(skillTasks); // 모든 스킬 효과가 완료될 때까지 대기
    }
}

public static class SkillAnimationExtensions
{
    public static async UniTask PlaySkillAnimation(this Unit caster, SkillGroup skill)
    {
        // 애니메이션 실행 예제 (애니메이션 재생 후 특정 이벤트에서 완료 처리)
        Animator animator = caster.GetComponent<Animator>();
        animator.SetTrigger("Skill");

        // 애니메이션 이벤트를 대기하는 코드 (예: 특정 프레임에서 콜백 발생)
        await UniTask.Delay(500); // 실제로는 애니메이션 이벤트로 처리해야 함
    }
}



[System.Serializable]
public class SkillGroup
{
    
    public SkillTargetingBase targeting;
    public List<SkillEffectBase> effects;
    public List<SkillVisualEffectBase> visualEffects;
}
