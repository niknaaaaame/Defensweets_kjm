using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Tile : MonoBehaviour
{
    public MapSO mapData;

    public Tilemap tilemap;
    public TileBase groundTile;

    private int[,] tileData;

    private int arrayX;
    private int arrayY;

    // Start is called before the first frame update
    void Start()
    {
        tileData = new int[mapData.mapWidth, mapData.mapHeight];

        for (int x = 0; x < mapData.mapWidth; x++)
        {
            for (int y = 0; y < mapData.mapHeight; y++)
            {
                tileData[x, y] = 1;
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
                    tileData[x, y] = 1;

                    Vector3Int tilePos = new Vector3Int(x + tilemap.origin.x, y + tilemap.origin.y, 0);
                    tilemap.SetTile(tilePos, groundTile);
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
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition); //화면 좌표를 월드 좌표로
        mouseWorldPos.z = 0;
        Vector3Int cellPos = tilemap.WorldToCell(mouseWorldPos); //월드 좌표를 셀 좌표로 (화면 가운데가 0, 0)

        Vector3Int tilemapOrigin = tilemap.cellBounds.min;

        arrayX = cellPos.x - tilemapOrigin.x; //실제 타일맵 위치와 오프셋 빼기
        arrayY = cellPos.y - tilemapOrigin.y;

        if (arrayX < 0 || arrayX >= mapData.mapWidth || arrayY < 0 || arrayY >= mapData.mapHeight) return;

        if (place)
        {
            if (CanRestorationTile(arrayX, arrayY))
            {
                tilemap.SetTile(cellPos, groundTile);
                tileData[arrayX, arrayY] = 1;
            }
        }
        else
        {
            if(CanExploitationTile(arrayX, arrayY))
            {
                tilemap.SetTile(cellPos, null);
                tileData[arrayX, arrayY] = 0;
            }
            
        }
        
    }

    bool CanExploitationTile(int x, int y)
    {
        if (x == mapData.startPos.x && y == mapData.startPos.y && tileData[x, y] == 1)
            return true;

        if (tileData[x, y] == 0)
            return false;

        Vector2Int[] directions = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };
        foreach (var dir in directions)
        {
            int nx = x + dir.x;
            int ny = y + dir.y;
            if (nx >= 0 && nx < mapData.mapWidth && ny >= 0 && ny < mapData.mapHeight)
            {
                if (tileData[nx, ny] == 0)
                    return true;
            }
        }

        return false;
    }

    bool CanRestorationTile(int x, int y)
    {
        if (x == mapData.startPos.x && y == mapData.startPos.y)
        {
            Vector2Int[] mustBeFilled = { Vector2Int.up, Vector2Int.down, Vector2Int.right };
            foreach (var dir in mustBeFilled)
            {
                int nx = x + dir.x;
                int ny = y + dir.y;
                if (nx < 0 || nx >= mapData.mapWidth || ny < 0 || ny >= mapData.mapHeight || tileData[nx, ny] != 1)
                    return false;
            }
            return true;
        }

        if (tileData[x, y] == 1) return false;

        Vector2Int[] directions = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };
        int emptyCount = 0;

        foreach (var dir in directions)
        {
            int nx = x + dir.x;
            int ny = y + dir.y;

            if (nx < 0 || nx >= mapData.mapWidth || ny < 0 || ny >= mapData.mapHeight)
            {
                emptyCount++;
                continue;
            }

            if (tileData[nx, ny] == 1) emptyCount++;
        }

        return emptyCount >= 3;
    }

    bool CheckComplete()
    {
        if (tileData[mapData.goalPos.x, mapData.goalPos.y] == 0) return true;
        else return false;
    }
}
