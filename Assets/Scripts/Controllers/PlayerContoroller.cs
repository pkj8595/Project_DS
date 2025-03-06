using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class PlayerContoroller : PawnBase
{
    //float wait_run_retio = 0.0f;

    int _mask = Define.Layer.Ground.ToInt();

    protected override void Init()
    {
        WorldObjectType = Define.WorldObject.Pawn;
        PawnStat = gameObject.GetComponent<PawnStat>();

        //Managers.Input.KeyAction -= OnKeyboard;
        //Managers.Input.KeyAction += OnKeyboard;
        Managers.Input.MouseAction -= OnMouseEvent;
        Managers.Input.MouseAction += OnMouseEvent;

    }


    void OnKeyboard()
    {
        
    }

    void OnMouseEvent(Define.MouseEvent evt)
    {

    }

    void OnMouseEvent_IdleRun(Define.MouseEvent evt)
    {
        
    }

}