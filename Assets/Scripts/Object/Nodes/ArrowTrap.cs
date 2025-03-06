using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowTrap : NodeBase
{
    public SpriteRenderer _renderer;
    public GameObject _target;
    public CircleCollider2D _attackRangeCollider;

    void Start()
    {
        
    }

    void Update()
    {
        
    }


    public void SetTarget(GameObject target)
    {
        _target = target;
    }

    private void StartAnimation_Attack()
    {

    }

    private void StartAnimation_Rotate()
    {

    }

}
