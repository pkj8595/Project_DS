using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class PawnController : PawnBase
{
    public int _testCharacterNum;

    private void Start()
    {
        Init(_testCharacterNum);
    }

    public override void Init(int characterNum, bool isUpgrade = false)
    {
        base.Init(characterNum, isUpgrade);
    }

    public override void Update()
    {
        base.Update();
        
        if (Input.GetMouseButtonDown(0))
        {
            OnMove(Camera.main.ScreenToWorldPoint(Input.mousePosition), false);
        }
    }


}
