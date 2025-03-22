using FishNet.Object;
using System;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    public PlayerController[] _players = new PlayerController[2];

    internal void SetPlayer(PlayerController playerController)
    {
        _players[playerController.PlayerIndex] = playerController;
    }
}
