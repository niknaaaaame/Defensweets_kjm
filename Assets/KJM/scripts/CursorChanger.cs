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
    public Vector2 hotspot;
    public CursorMode cursorMode = CursorMode.Auto;
    public Toggle ExploitationState;

    private Texture2D defaultCursor;

    private int tilePosX;
    private int tilePosY;

    void Start()
    {
        defaultCursor = null;
        hotspot = new Vector2(cursorTexture.width / 2f, cursorTexture.height / 2f);
    }

    void Update() //여기도 살짝 바꿨어요 -여영부-
    {
        if (!ExploitationState.isOn)
        {
            Cursor.SetCursor(defaultCursor, Vector2.zero, cursorMode);
            return;
        }

        if (GameManager.Instance != null &&
        GameManager.Instance.CurrentState != GameState.Ready)
        {
            Cursor.SetCursor(defaultCursor, Vector2.zero, cursorMode);
            return;
        } //웨이브상태 커서변경불가 -여영부-

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
