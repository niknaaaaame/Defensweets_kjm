using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapReader_YYJ : MonoBehaviour
{
    public static TilemapReader_YYJ Instance { get; private set; }

    private Tilemap walkableTilemap;

    //private Vector3Int minCellBounds;
    //private Vector3Int maxCellBounds;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            return;
        }
        Instance = this;

        Tile tileEditorScript = FindObjectOfType<Tile>();
        if (tileEditorScript == null)
        {
            Debug.LogError("Tile 스크립트를 찾을수 없습니다");
            return;
        }

        walkableTilemap = tileEditorScript.tilemap;
        if (walkableTilemap == null)
        {
            Debug.LogError("tilemap이 연결되지 않았습니다");
        }

        /*
        MapSO mapData = tileEditorScript.mapData;
        if (mapData == null)
        {
            return;
        }

        // 타일맵 원점 가져옴
        Vector3Int tilemapOrigin = walkableTilemap.cellBounds.min;

        // 경계 계산
        minCellBounds = new Vector3Int(tilemapOrigin.x, tilemapOrigin.y, 0);
        maxCellBounds = new Vector3Int(tilemapOrigin.x + mapData.mapWidth - 1, tilemapOrigin.y + mapData.mapHeight - 1, 0);
        */
    }

    /*
    public bool IsWithinBounds(Vector3Int cellPos)
    {
        return cellPos.x >= minCellBounds.x && cellPos.x <= maxCellBounds.x &&
               cellPos.y >= minCellBounds.y && cellPos.y <= maxCellBounds.y;
    }
    */

    public bool IsWalkable(Vector3Int cellPos)
    {
        if (walkableTilemap == null) return false;

        return walkableTilemap.GetTile(cellPos) == null;
    }

    public Vector3Int WorldToCell(Vector3 worldPos)
    {
        if (walkableTilemap == null) return Vector3Int.zero;
        return walkableTilemap.WorldToCell(worldPos);
    }

    public Vector3 CellToWorld(Vector3Int cellPos)
    {
        if (walkableTilemap == null) return Vector3Int.zero;
        return walkableTilemap.GetCellCenterWorld(cellPos);
    }
}
