using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;

public class PlayerController : NetworkBehaviour
{
    public readonly SyncVar<int> Mineral = new(0);
    public readonly SyncVar<int> Gas = new(0);

    public int PlayerIndex { get; internal set; }


    
    private void Update()
    {
        if (!IsOwner)
            return;
        if(Input.GetKeyDown(KeyCode.Mouse0))
        {
            //MapManager.Map.GetGridPos()

            Debug.Log("call Managers.Map.GetGridPos");
            Managers.Map.GetGridPos(Camera.main.ScreenToWorldPoint(Input.mousePosition));
            Debug.Log("call after Managers.Map.GetGridPos");
        }

    }
    public void Init(int playerIndex)
    {
        PlayerIndex = playerIndex;
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
