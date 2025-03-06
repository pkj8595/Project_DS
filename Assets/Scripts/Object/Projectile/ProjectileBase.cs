using UnityEngine;
using System;

[RequireComponent(typeof(Poolable))]
public abstract class ProjectileBase : MonoBehaviour
{
    public ParticleSystem _effHit;
    [SerializeField] protected Collider _projectileCollider;

    protected DamageMessage _msg;
    protected Transform _target;
    protected Transform _startTrans;
    protected float _splashRange;
    /// <summary>
    /// 타격 사운드
    /// </summary>
    public const string effectSoundName = "";

    public virtual void Init(Transform startPosition, Transform target, float splashRange, in DamageMessage msg)
    {
        this.transform.SetPositionAndRotation(startPosition.position, startPosition.rotation);
        _startTrans = startPosition;
        _target = target;
        _splashRange = splashRange;
        _msg = msg;
        if (_projectileCollider == null)
            _projectileCollider = GetComponent<Collider>();
        _projectileCollider.enabled = true;
        _projectileCollider.isTrigger = true;
    }

    // 발사체가 타겟에 충돌했을 때 호출
    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger)
            return;
    }

    /// <summary>
    /// 이동 및 충돌로직 종료
    /// </summary>
    /// <param name="targetTransform">맞은 개체가 있을경우 transform 반환</param>
    protected virtual void OnStopProjectile(Transform targetTransform = null)
    {
        _projectileCollider.enabled = false;
    }

    protected virtual void ApplyDamage(Collider other)
    {
        Collider[] colliders;
        if (_splashRange == 0)
        {
            colliders = new Collider[1];
            colliders[0] = other;
        }
        else
        {
            colliders = Physics.OverlapSphere(transform.position, _splashRange);
        }

        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].gameObject.TryGetComponent(out IDamageable damageable))
            {
                damageable.ApplyTakeDamage(_msg);
                PlayEffect();
            }
        }
        OnStopProjectile(other.transform);

    }

    protected abstract void Release();

    protected virtual void PlayEffect()
    {
        if (_effHit != null)
        {
            _effHit.gameObject.SetActive(true);
            _effHit.Play();
        }
    }
}
