using System.Collections;
using System.Collections.Generic;
using UnityEditor;
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

    private TileEditor tileEditorScript;
    private Vector3Int tilemapOrigin;
    private MapSO mapData;
    private StageTilesSO specialTilesSO;

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
            return;
        }

        mapData = tileEditorScript.mapData;
        if (mapData == null)
        {
            Debug.LogError("mapSO를 찾을 수 없습니다");
            return;
        }

        specialTilesSO = tileEditorScript.specialTilesSO;   
        if (specialTilesSO == null)
        {
            Debug.LogError("specialTilesSO를 찾을 수 없습니다");
            return;
        }

        walkableTilemap.CompressBounds();
        tilemapOrigin = walkableTilemap.cellBounds.min;

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

    public TileEffectType GetEffectAtWorldPos(Vector3 worldPos)
    {
        if (walkableTilemap == null || mapData == null || specialTilesSO == null)
        {
            return TileEffectType.None;
        }

        Vector3Int cellPos = walkableTilemap.WorldToCell(worldPos);

        // 셀 -> TileEditor 인덱스
        int arrayX = cellPos.x - tilemapOrigin.x;
        int arrayY = cellPos.y - tilemapOrigin.y;

        if (arrayX < 0 || arrayX >= mapData.mapWidth || arrayY < 0 || arrayY >= mapData.mapHeight)
        {
            return TileEffectType.None;
        }

        // 현재 스테이지 (임시 1, 이후 GameManager에서 받아오기)
        int currentStage = 1;

        foreach (var stage in specialTilesSO.stages.FindAll(s => s.stageNumber == currentStage))
        {
            foreach (var pos in stage.positions)
            {
                if (pos.x == arrayX && pos.y == arrayY)
                {
                    // 특수 타일 발견
                    // 타일 에셋 이름 임시로 구분

                    TileBase tileAsset = stage.tileAsset;
                    if (tileAsset == null) continue;

                    if (tileAsset.name.Contains("Sticky") || tileAsset.name.Contains("끈적"))
                    {
                        return TileEffectType.Sticky;
                    }
                    if (tileAsset.name.Contains("Explosive") || tileAsset.name.Contains("분화구"))
                    {
                        return TileEffectType.Explosive;
                    }
                    if (tileAsset.name.Contains("Sweet") || tileAsset.name.Contains("딸기")) 
                    {
                        return TileEffectType.SweetBoost;
                    }

                    // 임시로 special 타입들 sticky로 취급
                    if (stage.type == StageTilesSO.TileType.Special)
                    {
                        return TileEffectType.Sticky;
                    }
                    
                    return TileEffectType.None;
                }
            }
        }

        // 몬스터가 밟은 곳이 일반 길이라면 효과 X
        if (IsWalkable(cellPos))
        {
            return TileEffectType.None;
        }

        return TileEffectType.None; // 길, 특수 타일도 아니면
    }
}
