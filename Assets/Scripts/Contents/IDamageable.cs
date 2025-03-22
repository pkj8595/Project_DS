using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    public Define.ETeam Team { get;}
    public Define.WorldObject WorldObjectType { get;}
    public Vector3 StateBarOffset { get; }

    public abstract bool ApplyTakeDamage(DamageMessage message);
    public abstract Transform GetTransform();
    public abstract Collider2D GetCollider();
    public abstract bool IsDead();
    public IStat GetStat();

}

public interface IAttackable
{
    public Define.ETeam Team {get;}
    public float SearchRange { get;}

    /// <summary>
    /// 적 
    /// </summary>
    /// <param name="searchRange"></param>
    /// <returns></returns>
    public static IDamageable SearchTarget(IDamageable transform, float searchRange, Define.ETargetType targetType)
    {
        if (Define.ETargetType.Self == targetType)
            return transform;

        int layerTarget = (int)Define.Layer.Pawn | (int)Define.Layer.Building;
        Collider[] colliders = Physics.OverlapSphere(transform.GetTransform().position, searchRange, layerTarget);

        float minDistance = float.MaxValue;
        IDamageable target = null;
        foreach (var collider in colliders)
        {
            if (collider.transform == transform.GetTransform())
                continue;

            IDamageable unit = collider.attachedRigidbody.GetComponent<IDamageable>();
            if (unit != null && !unit.IsDead() && unit.GetTargetType(transform.Team) == targetType)
            {
                float value = Vector3.Distance(transform.GetTransform().position, collider.transform.position);
                if (value < minDistance)
                {
                    minDistance = value;
                    target = unit;
                }
            }
        }

        return target;
    }
}

public class Unit : MonoBehaviour
{
    public virtual bool UpgradeUnit() { return false; }
    public bool IsAlive { get; set; }
}