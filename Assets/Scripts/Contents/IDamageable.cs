using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    public Define.ETeam Team { get;}
    public Vector3 StateBarOffset { get; }
    public Transform ProjectileTF { get; }

    public abstract bool ApplyTakeDamage(DamageMessage message);
    public abstract Transform GetTransform();
    public abstract Collider2D GetCollider();
    public abstract bool IsDead();
    public IStat GetStat();

}


public class Unit : MonoBehaviour
{
    public Define.ETeam Team { get; set; }
    public virtual bool UpgradeUnit() { return false; }
    public bool IsAlive { get; set; }
}