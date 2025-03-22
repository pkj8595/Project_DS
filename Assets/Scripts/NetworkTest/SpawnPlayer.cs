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
        GameObject obj = Instantiate(_playerPrefab);
        Spawn(obj, client);

        if (_spawnCount >= _spawnPoints.Length)
            return;
        
        obj.transform.SetPositionAndRotation(_spawnPoints[_spawnCount].position, _spawnPoints[_spawnCount].rotation);
        var playerController = obj.GetComponent<PlayerController>();
        playerController.PlayerIndex = _spawnCount;
        Managers.Game.SetPlayer(playerController);
        _spawnCount++;
    }
}
