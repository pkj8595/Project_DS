using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ArrowProjectile : ProjectileBase
{
    [SerializeField] protected float _rotationSpeed = 10f;
    private Vector3 _offsetTarget = new Vector3 (0f, 0.5f,0f);
    public float addHeight = 2f;
    public float duration = 0.5f;
    Sequence sequence;

    public override void Init(Transform startTrans, Transform target, float splashRange, in DamageMessage msg)
    {
        base.Init(startTrans, target, splashRange, msg);
        Shot();
    }

    public void Shot()
    {
        if (_target == null) 
        {
            Release();
        } 

        Vector3 velocity = Vector3.zero;
        if(_target.TryGetComponent(out Rigidbody rigidbody))
        {
            velocity = rigidbody.linearVelocity;
        }
        Vector3 startPosition = _startTrans.position;
        Vector3 targetPosition = _target.position + _offsetTarget + (velocity);
        float height =  (targetPosition.y > startPosition.y ? targetPosition.y : startPosition.y) + addHeight;

        // Path를 위한 waypoints 설정
        Vector3[] path = new Vector3[4];
        path[0] = startPosition;
        path[1] = new Vector3((startPosition.x + targetPosition.x) / 2, height, (startPosition.z + targetPosition.z) / 2);
        path[2] = targetPosition;
        path[3] = targetPosition + ((path[2] - path[1]).normalized * 10);

        sequence = DOTween.Sequence();
        sequence.Append(transform.DOPath(path, duration, PathType.CatmullRom)
            .SetEase(Ease.Linear)
            .SetLookAt(0.01f));
        sequence.AppendCallback(() => { Release(); });
    }

    protected override void Release()
    {
        Managers.Resource.Destroy(gameObject);
    }

    protected override void ApplyDamage(Collider other)
    {
        base.ApplyDamage(other);
        
    }

    protected override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);

        if (other.transform == _target.transform)
        {
            ApplyDamage(other);
            if (!string.IsNullOrEmpty(effectSoundName))
                Managers.Sound.Play(effectSoundName, Define.Sound.Effect);
        }
        else if (1 << other.gameObject.layer == (int)Define.Layer.Ground)
        {
            if (0 < _splashRange)
            {
                ApplyDamage(other);
            }
            OnStopProjectile();
        }
    }

    protected override void OnStopProjectile(Transform targetTransform = null)
    {
        base.OnStopProjectile(targetTransform);
        _projectileCollider.enabled = false;
        sequence.Kill();
        Invoke(nameof(Release), 2f);
        if (targetTransform != null)
            transform.SetParent(targetTransform);
    }

}
