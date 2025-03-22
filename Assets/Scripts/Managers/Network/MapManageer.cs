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
            Vector3Int cellPos = _grid.WorldToCell(mousePos);
            Debug.Log(cellPos);
        }
    }
}
