using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class GameManager
{
    public Inventory Inven { get; private set; } = new Inventory();

    readonly HashSet<IDamageable> _enumyPawnGroup = new HashSet<IDamageable>();
    readonly HashSet<IDamageable> _pawnGroup = new HashSet<IDamageable>();
    public event Action<int> OnSpawnEvent;


    public void Init()
    {
    }

    public void Clear()
    {
        _enumyPawnGroup.Clear();
        _pawnGroup.Clear();

    }



}