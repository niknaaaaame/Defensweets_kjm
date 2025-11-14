using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class CursorChanger : MonoBehaviour
{
    public Tilemap tilemap;        
    public MapSO mapData;         
    public Texture2D cursorTexture; 
    public Vector2 hotspot = Vector2.zero;
    public CursorMode cursorMode = CursorMode.Auto;
    public Toggle ExploitationState;

    private Texture2D defaultCursor;

    private int tilePosX;
    private int tilePosY;

    void Start()
    {
        defaultCursor = null; 
    }

    void Update()
    {
        if (ExploitationState.isOn)
        {
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseWorldPos.z = 0;
            Vector3Int cellPos = tilemap.WorldToCell(mouseWorldPos);

            Vector3Int tilemapOrigin = tilemap.cellBounds.min;

            tilePosX = cellPos.x - tilemapOrigin.x;
            tilePosY = cellPos.y - tilemapOrigin.y;

            if (tilePosX >= 0 && tilePosX < mapData.mapWidth &&
                tilePosY >= 0 && tilePosY < mapData.mapHeight)
            {
                Cursor.SetCursor(cursorTexture, hotspot, cursorMode);
            }
            else
            {
                Cursor.SetCursor(defaultCursor, Vector2.zero, cursorMode);
            }
        }
    }
}
