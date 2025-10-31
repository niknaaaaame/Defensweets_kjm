using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapReader_YYJ : MonoBehaviour
{
    public static TilemapReader_YYJ Instance { get; private set; }

    private Tilemap walkableTilemap;

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
            Debug.LogError("Tile ��ũ��Ʈ�� ã���� �����ϴ�");
            return;
        }

        walkableTilemap = tileEditorScript.tilemap;
        if (walkableTilemap == null)
        {
            Debug.LogError("tilemap�� ������� �ʾҽ��ϴ�");
        }

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
        if (walkableTilemap == null) return Vector3Int.zero;
        return walkableTilemap.GetCellCenterWorld(cellPos);
    }
}
