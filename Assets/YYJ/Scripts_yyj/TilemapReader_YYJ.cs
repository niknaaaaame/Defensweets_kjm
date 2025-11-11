using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapReader_YYJ : MonoBehaviour
{
    public static TilemapReader_YYJ Instance { get; private set; }

    private Tilemap walkableTilemap;

    // 이동 가능 구역 오브젝트 위치로 고정 (스크립트로만 처리하려 했으나 도저히 오류가 해결이 안되서 방식 변경)
    public Transform boundCorner1;
    public Transform boundCorner2;

    private Vector3Int minCellBounds;
    private Vector3Int maxCellBounds;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            return;
        }
        Instance = this;

        TileEditor tileEditorScript = FindObjectOfType<TileEditor>();
        if (tileEditorScript == null)
        {
            Debug.LogError("TileEditor 스크립트를 찾을수 없습니다");
            return;
        }

        walkableTilemap = tileEditorScript.tilemap;
        if (walkableTilemap == null)
        {
            Debug.LogError("tilemap이 연결되지 않았습니다");
            return;
        }

        if (boundCorner1 == null || boundCorner2 == null)
        {
            Debug.LogError("boundCorner가 없습니다");
            return;
        }

        Vector3Int cell1 = walkableTilemap.WorldToCell(boundCorner1.position);
        Vector3Int cell2 = walkableTilemap.WorldToCell(boundCorner2.position);

        minCellBounds = Vector3Int.Min(cell1, cell2);
        maxCellBounds = Vector3Int.Max(cell1, cell2);

        Debug.Log($"Min : {minCellBounds}, Max : {maxCellBounds}");
    }

    public bool IsWithinBounds(Vector3Int cellPos)
    {
        return cellPos.x >= minCellBounds.x && cellPos.x <= maxCellBounds.x &&
               cellPos.y >= minCellBounds.y && cellPos.y <= maxCellBounds.y;
    }

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
        if (walkableTilemap == null) return Vector3.zero;      
        return walkableTilemap.GetCellCenterWorld(cellPos);
    }
}
