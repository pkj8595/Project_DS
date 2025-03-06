using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameView : MonoSingleton<GameView>
{
    [field: SerializeField] public GameObject PawnObj { get; set; }
    [field: SerializeField] public GameObject ProjectileObj { get; set; }
    [field: SerializeField] public GameObject EffectObj { get; set; }
    [field : SerializeField] public List<GameObject> GateList { get; set; } = new();


    [field: SerializeField] public List<BuildingNode> ConstructedBuildingList { get; set; }

    public GameObject GetParentObj(Define.EParentObj obj)
    {
        switch (obj)
        {
            case Define.EParentObj.Pawn:
                return PawnObj;
            case Define.EParentObj.Projectile:
                return ProjectileObj;
            case Define.EParentObj.Effect:
                return EffectObj;
            default:
                Debug.LogError("찾을 수 없는 ParentObj");
                return null;
        }

    }


}
