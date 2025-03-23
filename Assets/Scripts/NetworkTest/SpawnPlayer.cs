using FishNet.Connection;
using FishNet.Object;
using UnityEngine;

public class SpawnPlayer : NetworkBehaviour
{
    [SerializeField] private GameObject _playerPrefab;
    [SerializeField] private Transform[] _spawnPoints;
    private int _spawnCount;

    public override void OnStartClient()
    {
        base.OnStartClient();

        PlayerSpawn();
    }

    [ServerRpc(RequireOwnership = false)]
    private void PlayerSpawn(NetworkConnection client = null)
    {
        if (!(_spawnCount < _spawnPoints.Length))
            return;
        
        GameObject obj = Instantiate(_playerPrefab);
        obj.transform.SetPositionAndRotation(_spawnPoints[_spawnCount].position, _spawnPoints[_spawnCount].rotation);
        var playerController = obj.GetComponent<PlayerController>();
        playerController.PlayerIndex = _spawnCount;
        Managers.Game.SetPlayer(playerController);
        _spawnCount++;

        Spawn(obj, client);

    }
}
