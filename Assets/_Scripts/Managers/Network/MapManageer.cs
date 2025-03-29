using FishNet.Object;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : NetworkBehaviour
{
    [SerializeField] private Grid _grid;
    [SerializeField] private Dictionary<Vector2, BuildingBase> _dicBuilding = new();

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

    private void Update()
    {
        if (IsServerInitialized && Input.GetMouseButtonDown(0))
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            GetGridPos(mousePos);
        }
    }

    [ServerRpc]
    public void GetGridPos(Vector3 targetPos)
    {
        Vector3Int cellWorldToCellPos = _grid.WorldToCell(targetPos);
        Vector2Int cell2Pos = new Vector2Int(cellWorldToCellPos.x, cellWorldToCellPos.y);
        Debug.Log(cell2Pos);
    }
}
