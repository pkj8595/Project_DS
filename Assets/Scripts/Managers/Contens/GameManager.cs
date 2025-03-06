using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class GameManager
{
    public Inventory Inven { get; private set; } = new Inventory();

    readonly HashSet<IDamageable> _enumyPawnGroup = new HashSet<IDamageable>();
    readonly HashSet<IDamageable> _pawnGroup = new HashSet<IDamageable>();
    List<BuildingNode> BuildingGroup { get => GameView.Instance.ConstructedBuildingList; }
    public Action<int> OnSpawnEvent;


    public void Init()
    {
        Inven.Init();
        InitWave();
    }

    public void Clear()
    {
        _enumyPawnGroup.Clear();
        _pawnGroup.Clear();

    }

    public PawnBase SpawnPawn(int tableNum, Define.ETeam team)
    {
        GameObject go = Managers.Resource.Instantiate("Pawn/Pawn",
                    GameView.Instance.GetParentObj(Define.EParentObj.Pawn).transform);

        PawnBase pawn = go.GetComponent<PawnBase>();
        pawn.Init(tableNum, team);

        if (team == Define.ETeam.Playable)
            _pawnGroup.Add(pawn);
        else
            _enumyPawnGroup.Add(pawn);

        return pawn;
    }


    public void Despawn(IDamageable go)
    {
        switch (go.WorldObjectType)
        {
            case Define.WorldObject.Pawn:
                {
                    if (go.Team == Define.ETeam.Enemy)
                    {
                        if (_enumyPawnGroup.Contains(go))
                        {
                           _enumyPawnGroup.Remove(go);
                            OnSpawnEvent?.Invoke(-1);
                        }
                    }
                    else
                    {
                        if (_pawnGroup.Contains(go))
                            _pawnGroup.Remove(go);
                    }
                }
                break;
            case Define.WorldObject.Building:
                {
                   
                }
                break;
        }
    }

    public void SetPawnInScene(PawnController pawn)
    {
        pawn.Init(pawn._testCharacterNum);
        if (pawn.Team == Define.ETeam.Playable)
            _pawnGroup.Add(pawn);
        else
            _enumyPawnGroup.Add(pawn);
    }


}