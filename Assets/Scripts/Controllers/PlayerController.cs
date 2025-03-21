using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;

public class PlayerController : NetworkBehaviour
{
    public readonly SyncVar<int> Mineral = new(0);
    public readonly SyncVar<int> Gas = new(0);

    private void Update()
    {
        if (!IsOwner)
            return;


    }

    public void RequestBuild(int tableNum, Vector3 position)
    {

    }

    public void UpgradeBuilding(BuildingBase building)
    {

    }

    public void OnReceiveInput(/*InputData data*/)
    {

    }



}
