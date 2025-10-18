using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Tile : MonoBehaviour
{
    public Tilemap tilemap;
    public TileBase groundTile;

    public int mapWidth = 29;
    public int mapHeight = 12;
    public int tileSize = 60;

    private int[,] tileData;

    // Start is called before the first frame update
    void Start()
    {
        tileData = new int[mapWidth, mapHeight];
        tilemap.SetTile(new Vector3Int(30, 0, 0), groundTile);
        tilemap.SetTile(new Vector3Int(-1, 0, 0), groundTile);
    }

    // Update is called once per frame
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
    }

    void HandleTile(bool place)
    {
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0;
        //메인 카메라가 보고 있는 화면의 마우스 위치를 월드 위치로 바꿔줌
        Vector3Int cellPos = tilemap.WorldToCell(mouseWorldPos);

        Vector3Int tilemapOrigin = tilemap.cellBounds.min;

        int arrayX = cellPos.x - tilemapOrigin.x;
        int arrayY = cellPos.y - tilemapOrigin.y;

        if (arrayX < 0 || arrayX >= mapWidth || arrayY < 0 || arrayY >= mapHeight) return;

        if (place)
        {
            if (CanPlaceTile(arrayX, arrayY))
            {
                tilemap.SetTile(cellPos, groundTile);
                tileData[arrayX, arrayY] = 1;
            }
        }
        else
        {
            if(CanRemoveTile(arrayX, arrayY))
            {
                tilemap.SetTile(cellPos, null);
                tileData[arrayX, arrayY] = 0;
            }
            
        }
        
    }

    bool CanRemoveTile(int x, int y)
    {
        if (x == 0 && y == 4 && tileData[x, y] == 1) return true;

        if (tileData[x, y] == 0)
        {
            return false;
        }

        if (x > 0 && tileData[x - 1, y] == 0) return true;
        if (x < mapWidth - 1 && tileData[x + 1, y] == 0) return true;
        if (y > 0 && tileData[x, y-1] == 0) return true;
        if (y < mapHeight - 1 && tileData[x, y + 1] == 0) return true;

        return false;
    }

    bool CanPlaceTile(int x, int y)
    {
        if (tileData[x, y] == 1) return false;

        int emptyCount = 0;

        if (x == 0 || tileData[x - 1, y] == 1) emptyCount++;
        if (x == mapWidth - 1 || tileData[x + 1, y] == 1) emptyCount++;
        if (y == 0 || tileData[x + 1, y] == 1) emptyCount++;
        if (y == mapHeight - 1 || tileData[x, y+1] == 1) emptyCount++;

        return emptyCount >= 3;
    }
}
