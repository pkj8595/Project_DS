using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StairwayNode : BlockNode
{

    public override void SetNodeRotation(Vector3 normal)
    {
        /*base.SetNodeRotation(normal);
        if (normal == Vector3.zero || normal == Vector3.up || normal == Vector3.down)
            return;
        transform.rotation = Quaternion.LookRotation(normal, Vector3.up);*/

    }
}
