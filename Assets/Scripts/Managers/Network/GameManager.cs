using FishNet.Object;
using System;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    public PlayerController[] _players = new PlayerController[2];

    public override void OnStartClient()
    {
        base.OnStartClient();
        Debug.Log($"{this.name} 클라시작");
    }
    public override void OnStartServer()
    {
        base.OnStartServer();
        Debug.Log($"{this.name} 서버시작");
    }

    internal void SetPlayer(PlayerController playerController)
    {
        _players[playerController.PlayerIndex] = playerController;
    }
}
