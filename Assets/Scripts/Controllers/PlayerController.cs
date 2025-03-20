using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;

public class PlayerController : NetworkBehaviour
{
    public readonly SyncVar<int> Mineral = new();
    public readonly SyncVar<int> Gas = new();

    

    private void Update()
    {
        if (!IsOwner)
            return;


    }

    [Server]
    public void Test()
    {

    }


}
