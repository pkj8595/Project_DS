using FishNet.Object;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : NetworkBehaviour
{
    [SerializeField] private Grid _grid;
    [SerializeField] private Dictionary<Vector2, BuildingBase> _dicBuilding = new();

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            GetGridPos(mousePos);
        }
    }

    private void GetGridPos(Vector3 targetPos)
    {
        Vector3Int cellWorldToCellPos = _grid.WorldToCell(targetPos);
        Vector2Int cell2Pos = new Vector2Int(cellWorldToCellPos.x, cellWorldToCellPos.y);
        Debug.Log(cell2Pos);
    }
}
