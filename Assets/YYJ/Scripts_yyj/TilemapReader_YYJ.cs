using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapReader_YYJ : MonoBehaviour
{
    public static TilemapReader_YYJ Instance { get; private set; }

    private Tilemap walkableTilemap;
    private TileEditor tileEditorScript;
    private MapSO mapData;
    private Vector3Int tilemapOrigin;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            return;
        }
        Instance = this;

        tileEditorScript = FindObjectOfType<TileEditor>();
        if (tileEditorScript == null)
        {
            Debug.LogError("TileEditor 스크립트를 찾을수 없습니다");
            return;
        }

        walkableTilemap = tileEditorScript.tilemap;
        if (walkableTilemap == null)
        {
            Debug.LogError("tilemap이 연결되지 않았습니다");
        }

        mapData = tileEditorScript.mapData;
        if (mapData == null)
        {
            Debug.LogError("MapSO가 연결되지 않았습니다");
            return;
        }

        walkableTilemap.CompressBounds();
        tilemapOrigin = walkableTilemap.cellBounds.min;

        Debug.Log($"맵 원점 : {tilemapOrigin} | 맵 데이터 : W:{mapData.mapWidth}, H:{mapData.mapHeight}");
    }

    /*
    public bool IsWithinBounds(Vector2Int arrayPos)
    {
        if (mapData == null) return false;
        return arrayPos.x >= 0 && arrayPos.x < mapData.mapWidth &&
               arrayPos.y >= 0 && arrayPos.y < mapData.mapHeight;
    }
    */
    public bool IsWalkable(Vector2Int arrayPos)
    {
        if (walkableTilemap == null) return false;

        Vector3Int cellPos = new Vector3Int(arrayPos.x + tilemapOrigin.x, arrayPos.y + tilemapOrigin.y, 0);

        return walkableTilemap.GetTile(cellPos) == null;
    }

    public Vector2Int WorldToArray(Vector3 worldPos)
    {
        if (walkableTilemap == null) return Vector2Int.zero;
        Vector3Int cellPos = walkableTilemap.WorldToCell(worldPos);

        int arrayX = cellPos.x - tilemapOrigin.x;
        int arrayY = cellPos.y - tilemapOrigin.y;

        return new Vector2Int(arrayX, arrayY);
    }

    public Vector3 ArrayToWorld(Vector2Int arrayPos)
    {
        if (walkableTilemap == null) return Vector3.zero;

        Vector3Int cellPos = new Vector3Int(arrayPos.x + tilemapOrigin.x, arrayPos.y + tilemapOrigin.y, 0);

        return walkableTilemap.GetCellCenterWorld(cellPos);
    }
}
