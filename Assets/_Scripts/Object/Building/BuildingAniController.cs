using Cysharp.Threading.Tasks;
using UnityEngine;

public class BuildingAniController : MonoBehaviour, ISkillMotion
{
    [SerializeField] private Animator _animator;

    public bool IsPlaying => throw new System.NotImplementedException();


    void Start()
    {
        if (_animator == null)
            _animator = GetComponent<Animator>();
    }

    public UniTask RunAnimation(ESkillMotionTriger trigerm, float delay)
    {
        throw new System.NotImplementedException();
    }
}
