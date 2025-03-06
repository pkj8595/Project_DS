using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator
{
    public int _width;
    public int _height;
    public string _seed;
    public bool _useRandomSeed;

    [Range(0, 100)]
    public int _randomFillPercent;
    List<List<int>> _map;

    private void Init()
    {
        _map = new List<List<int>>(_height);
        for (int y = 0; y < _height; y++)
        {
            _map.Add(new List<int>(_width));
            for (int x = 0; x < _width; x++)
            {
                _map[y].Add(0);
            }
        }
    }


    public List<List<int>> MakeMap(int maxX, int maxY, string seed, bool useRandomSeed, int fillPercent)
    {
        _width = maxX;
        _height = maxY;
        _seed = seed;
        _useRandomSeed = useRandomSeed;
        _randomFillPercent = fillPercent;

        Init();
        GenerateMap();
        return _map;
    }

    private void RandomFillMap()
    {
        if (_useRandomSeed)
        {
            _seed = Time.time.ToString();
        }
        System.Random pseudoRandoms = new System.Random(_seed.GetHashCode());

        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                if (x == 0 || x == _width - 1 || y == 0 || y == _height - 1)
                {
                    _map[y][x] = 1;
                }
                else
                {
                    _map[y][x] = pseudoRandoms.Next(0, 100) < _randomFillPercent ? 1 : 0;
                }
            }
        }
    }

    private void GenerateMap()
    {
        RandomFillMap();
        SmoothMap();
    }

    void OnDrawGizmos()
    {
        if (_map is null)
        {
            return;
        }

        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                Gizmos.color = _map[y][x] == 1 ? Color.black : Color.white;
                Vector2 pos = new Vector2(-_width / 2 + x + 0.5f, -_height / 2 + y + 0.5f);
                Gizmos.DrawCube(pos, Vector2.one);
            }
        }
    }

    private int GetAdjustCells(int currentX, int currentY)
    {
        int cells = 0;

        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                if (i == 0 && j == 0) continue; // 현재 위치는 인접한 셀이 아니므로 건너뛴다.
                int adjX = currentX + i;
                int adjY = currentY + j;
                if (adjX < 0 || adjY < 0 || adjX >= _width || adjY >= _height) ++cells;
                else cells += _map[adjY][adjX];
            }
        }
        return cells;
    }

    private void SmoothMap()
    {
        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                int neighbourWallTiles = GetAdjustCells(x, y);
                if (neighbourWallTiles > 4)
                {
                    _map[y][x] = 1;
                }
                else if (neighbourWallTiles < 4)
                {
                    _map[y][x] = 0;
                }
            }
        }
    }
}
