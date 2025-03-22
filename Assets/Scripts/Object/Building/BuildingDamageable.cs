using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingDamageable : MonoBehaviour, IDamageable
{
    public Define.ETeam Team { get=> _buildingBase.Team;}
    public Define.WorldObject WorldObjectType { get => _buildingBase.WorldObjectType; }

    public Vector3 StateBarOffset => _buildingBase.StateBarOffset;

    [SerializeField] private Vector3 BuildingStateBarOffset;

    BuildingBase _buildingBase;
    public void Init(BuildingBase buildingBase)
    {
        _buildingBase = buildingBase;
    }


    public virtual bool ApplyTakeDamage(DamageMessage message)
    {
        _buildingBase.Stat.ApplyDamageMessage(ref message);

        return false;
    }

    public virtual IStat GetStat()
    {
        return _buildingBase.Stat;
    }

    public Transform GetTransform()
    {
        return transform;
    }

    public bool IsDead()
    {
        return _buildingBase.Stat.IsDead;
    }

    public Collider2D GetCollider()
    {
        return _buildingBase.Collider;
    }
}
