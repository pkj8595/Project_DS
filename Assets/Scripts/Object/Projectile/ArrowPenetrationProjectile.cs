using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ArrowPenetrationProjectile : ProjectileBase
{
    public float duration = 1f;
    Sequence sequence;

    public override void Init(Transform startTrans, Transform target, float splashRange, in DamageMessage msg)
    {
        base.Init(startTrans, target, splashRange, msg);
        Shot();
    }

    public void Shot()
    {
        sequence = DOTween.Sequence();

        Vector3 targetPosition = transform.position + (transform.forward * _msg.skill.MaxRange);
        sequence.Append(transform.DOMove(targetPosition, duration)
                .SetEase(Ease.Linear));
        sequence.AppendCallback(() => { Release(); });
    }

    protected override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);

        if (other.TryGetComponent(out IDamageable damageable))
        {
            damageable.ApplyTakeDamage(_msg);
            OnStopProjectile(other.transform);
        }
        else if (1 << other.gameObject.layer == (int)Define.Layer.Ground)
        {
            OnStopProjectile();
        }
    }

    protected override void OnStopProjectile(Transform targetTransform = null)
    {
        base.OnStopProjectile(targetTransform);
        sequence.Kill();
        Invoke(nameof(Release), 2f);
        if (targetTransform != null)
            transform.SetParent(targetTransform);
    }

    protected override void Release()
    {
        gameObject.SetActive(false);
        Managers.Resource.Destroy(gameObject);
    }
}
