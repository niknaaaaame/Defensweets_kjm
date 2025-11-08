using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileEditor : MonoBehaviour
{
    public MapSO mapData;

    public Tilemap tilemap;
    public TileBase groundTile;
    public TileBase specialTile; 
    public SpecialTilesSO specialTilesSO; 

    private int[,] tileData;
    private bool[,] isSpecialTile; 

    private int arrayX;
    private int arrayY;

    private const int PATH = 0; 
    private const int BLOCK = 1; 

    void Start()
    {
        tileData = new int[mapData.mapWidth, mapData.mapHeight];
        isSpecialTile = new bool[mapData.mapWidth, mapData.mapHeight];

        for (int x = 0; x < mapData.mapWidth; x++)
        {
            for (int y = 0; y < mapData.mapHeight; y++)
            {
                tileData[x, y] = BLOCK;
                isSpecialTile[x, y] = false;
            }
        }

        if (specialTilesSO != null)
        {
            var stage = specialTilesSO.stages.Find(s => s.stageNumber == 1); 
            if (stage != null)
            {
                foreach (var pos in stage.positions)
                {
                    isSpecialTile[pos.x, pos.y] = true;
                }
            }
        }
    }

    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            HandleTile(false);
        }
        else if (Input.GetMouseButton(1))
        {
            HandleTile(true);
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            for (int x = 0; x < mapData.mapWidth; x++)
            {
                for (int y = 0; y < mapData.mapHeight; y++)
                {
                    tileData[x, y] = BLOCK;
                    Vector3Int tilePos = new Vector3Int(x + tilemap.origin.x, y + tilemap.origin.y, 0);
                    tilemap.SetTile(tilePos, isSpecialTile[x, y] ? specialTile : groundTile);
                }
            }
        }

        if (CheckComplete())
        {
            Debug.Log("연결 완료");
        }
    }

    void HandleTile(bool place)
    {
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0;
        Vector3Int cellPos = tilemap.WorldToCell(mouseWorldPos);

        Vector3Int tilemapOrigin = tilemap.cellBounds.min;

        arrayX = cellPos.x - tilemapOrigin.x;
        arrayY = cellPos.y - tilemapOrigin.y;

        if (arrayX < 0 || arrayX >= mapData.mapWidth || arrayY < 0 || arrayY >= mapData.mapHeight) return;

        if (place)
        {
            tilemap.SetTile(cellPos, isSpecialTile[arrayX, arrayY] ? specialTile : groundTile);
            tileData[arrayX, arrayY] = BLOCK;
            //ResourceSystem.Instance.AddCrystal(1);
            
        }
        else
        {
            if (CanExploitationTile(arrayX, arrayY))
            {
                tilemap.SetTile(cellPos, null);
                tileData[arrayX, arrayY] = PATH;
                ResourceSystem.Instance.TryUseCrystal(2);
                Debug.Log($"Crystal: {ResourceSystem.Instance.Crystal}");
            }
        }
    }

    bool CanExploitationTile(int x, int y)
    {
        if (x == mapData.startPos.x && y == mapData.startPos.y && tileData[x, y] == BLOCK)
            return true;

        if (tileData[x, y] == PATH)
            return false;

        Vector2Int[] directions = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };
        foreach (var dir in directions)
        {
            int nx = x + dir.x;
            int ny = y + dir.y;
            if (nx >= 0 && nx < mapData.mapWidth && ny >= 0 && ny < mapData.mapHeight)
            {
                if (tileData[nx, ny] == PATH)
                    return true;
            }
        }
        return false;
    }

    //bool CanRestorationTile(int x, int y)
    //{
    //    if (x == mapData.startPos.x && y == mapData.startPos.y)
    //    {
    //        Vector2Int[] mustBeFilled = { Vector2Int.up, Vector2Int.down, Vector2Int.right };
    //        foreach (var dir in mustBeFilled)
    //        {
    //            int nx = x + dir.x;
    //            int ny = y + dir.y;
    //            if (nx < 0 || nx >= mapData.mapWidth || ny < 0 || ny >= mapData.mapHeight || tileData[nx, ny] != 1)
    //                return false;
    //        }
    //        return true;
    //    }

    //    if (tileData[x, y] == 1) return false;

    //    Vector2Int[] directions = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };
    //    int emptyCount = 0;

    //    foreach (var dir in directions)
    //    {
    //        int nx = x + dir.x;
    //        int ny = y + dir.y;

    //        if (nx < 0 || nx >= mapData.mapWidth || ny < 0 || ny >= mapData.mapHeight)
    //        {
    //            emptyCount++;
    //            continue;
    //        }

    //        if (tileData[nx, ny] == 1) emptyCount++;
    //    }

    //    return emptyCount >= 3;
    //}

    bool CheckComplete()
    {
        Vector3 startWorldPos = tilemap.GetCellCenterWorld(new Vector3Int(mapData.startPos.x, mapData.startPos.y, 0));
        Vector3 goalWorldPos = tilemap.GetCellCenterWorld(new Vector3Int(mapData.goalPos.x, mapData.goalPos.y, 0));

        List<Vector3> path = BFS.FindPath(startWorldPos, goalWorldPos);

        return path != null && path.Count > 0;
    }
}