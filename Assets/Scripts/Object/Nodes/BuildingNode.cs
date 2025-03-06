using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingNode : NodeBase
{
    public override void InstallationSuccess()
    {
        GetComponent<BuildingBase>().Init();
    }
}
