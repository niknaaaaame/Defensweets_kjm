using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class TileEditor : MonoBehaviour
{
    public int currentStage = 1; //임시 현재 스테이지 번호
    //public AudioClip Nomal;
    //public AudioClip Special;
    //public AudioClip Restore;
    public MapSO mapData;
    public Toggle ExploitationState;

    public Tilemap tilemap;
    public TileBase groundTile;
    public TileBase specialTile; 
    public StageTilesSO specialTilesSO;

    public int crystalSpent = 2;
    public int crystalRefunded = 1;
    public int crystalGain_FromTile = 12; 

    private int[,] tileData;
    private bool[,] isSpecialTile;
    private bool[,] isResourceTile;

    private int arrayX;
    private int arrayY;

    private const int PATH = 0; 
    private const int BLOCK = 1;

    private AudioSource audioSource;

    private float soundMinInterval = 0.1f; 
    private bool canPlaySound = true;

    private Transform goal;
    private Transform start;
    private Vector3Int startCell;
    private Vector2Int startIndex;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        tileData = new int[mapData.mapWidth, mapData.mapHeight];
        isSpecialTile = new bool[mapData.mapWidth, mapData.mapHeight];
        isResourceTile = new bool[mapData.mapWidth, mapData.mapHeight];

        for (int x = 0; x < mapData.mapWidth; x++)
        {
            for (int y = 0; y < mapData.mapHeight; y++)
            {
                tileData[x, y] = BLOCK;
                isSpecialTile[x, y] = false;
            }
        }

        foreach (var stage in specialTilesSO.stages.FindAll(s => s.stageNumber == currentStage))
        {
            foreach (var pos in stage.positions)
            {
                if (stage.type == StageTilesSO.TileType.Special)
                    isSpecialTile[pos.x, pos.y] = true;
                else if (stage.type == StageTilesSO.TileType.Currency)
                    isResourceTile[pos.x, pos.y] = true;
            }
        }

        GameObject goalObject = GameObject.Find("Goal");
        if (goalObject == null)
        {
            Debug.LogError("Goal이 존재하지 않습니다.");
            Destroy(gameObject);
            return;
        }
        goal = goalObject.transform;

        GameObject startObject = GameObject.Find("Start");
        if (startObject == null)
        {
            Debug.LogError("Start가 존재하지 않습니다.");
            Destroy(gameObject);
            return;
        }
        start = startObject.transform;
        startCell = tilemap.WorldToCell(start.position);
        startIndex = new Vector2Int(startCell.x - tilemap.cellBounds.min.x, startCell.y - tilemap.cellBounds.min.y);
    }

    void Update()
    {
        if (ExploitationState.isOn)
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
        }

        if (CheckComplete())
        {
            //Debug.Log("연결 완료"); 
        }
        //Debug.Log($"{CheckComplete()}");
    }

    void HandleTile(bool place)
    {
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0;
        Vector3Int cellPos = tilemap.WorldToCell(mouseWorldPos);

        Vector3Int tilemapOrigin = tilemap.cellBounds.min;

        arrayX = cellPos.x - tilemapOrigin.x;
        arrayY = cellPos.y - tilemapOrigin.y;
        //Debug.Log($"HandleTile -> MouseWorld: {mouseWorldPos}, Cell: {cellPos}, Index: ({arrayX},{arrayY})");

        if (arrayX < 0 || arrayX >= mapData.mapWidth || arrayY < 0 || arrayY >= mapData.mapHeight) return;

        if (place)
        {
            if (CanRestorationTile(arrayX, arrayY))
            {
                tilemap.SetTile(cellPos, isSpecialTile[arrayX, arrayY] ? specialTile : groundTile);
                tileData[arrayX, arrayY] = BLOCK;
                ResourceSystem.Instance.AddCrystal(crystalRefunded);
                Debug.Log($"Crystal: {ResourceSystem.Instance.Crystal}");
            }
        }
        else
        {
            if (CanExploitationTile(arrayX, arrayY))
            {
                if (ResourceSystem.Instance.TryUseCrystal(crystalSpent))
                {
                    tilemap.SetTile(cellPos, null);
                    tileData[arrayX, arrayY] = PATH;
                    Debug.Log($"Crystal: {ResourceSystem.Instance.Crystal}");
                    if (isResourceTile[arrayX, arrayY])
                    {
                        ResourceSystem.Instance.AddCrystal(crystalGain_FromTile);
                        Debug.Log($"자원 타일 개척 +{crystalGain_FromTile} Crystals");
                        isResourceTile[arrayX, arrayY] = false;
                    }
                    PlayTileSound();
                }
            }
        }
    }

    bool CanExploitationTile(int x, int y) //개척 가능 조건
    {
        if (x == startIndex.x && y == startIndex.y && tileData[x, y] == BLOCK)
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

    bool CanRestorationTile(int x, int y) //복구 가능 조건 = 잘못된 재화 회수 방지
    {
        if (x < 0 || x >= mapData.mapWidth || y < 0 || y >= mapData.mapHeight) return false; //맵 바깥 검사
        if (tileData[x, y] == BLOCK) return false;

        tileData[x, y] = BLOCK;
        return true;
    }

    bool CheckComplete()
    {
        if (start == null || goal == null) return false;

        Vector3 startWorldPos = start.position;
        Vector3 goalWorldPos = goal.position;

        List<Vector3> path = BFS.FindPath(startWorldPos, goalWorldPos);

        if (path == null || path.Count == 0) return false;

        foreach (var worldPos in path)
        {
            Vector3Int cellPos = tilemap.WorldToCell(worldPos);
            int x = cellPos.x - tilemap.cellBounds.min.x;
            int y = cellPos.y - tilemap.cellBounds.min.y;

            if (x < 0 || x >= mapData.mapWidth || y < 0 || y >= mapData.mapHeight)
                return false;

            if (tileData[x, y] != PATH)
                return false;
        }

        return true; 
    }

    void PlayTileSound()
    {
        if (canPlaySound)
        {
            audioSource.Play();
            StartCoroutine(SoundCooldown());
        }
    }

    IEnumerator SoundCooldown()
    {
        canPlaySound = false;
        yield return new WaitForSeconds(soundMinInterval);
        canPlaySound = true;
    }
}