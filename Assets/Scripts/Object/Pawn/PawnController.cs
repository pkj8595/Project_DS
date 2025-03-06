using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PawnController : PawnBase
{
    public int _testCharacterNum;

    private void Start()
    {
    }

    public override void Init(int characterNum, bool isUpgrade = false)
    {
        base.Init(characterNum, isUpgrade);
    }

    protected override void Init()
    {
        base.Init();
    }

    public override void Update()
    {
        base.Update();
     
    }


}
