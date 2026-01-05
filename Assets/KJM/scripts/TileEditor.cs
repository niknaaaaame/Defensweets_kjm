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
    public TileBase resourceTile;
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

    public AudioClip exploitationClip;
    //public AudioClip resourcetileClip;
    //public AudioClip specialtileClip;
    public AudioClip restorationClip;
    public AudioClip resetClip;

    private float soundMinInterval = 0.1f; 
    private bool canPlaySound = true;

    private Transform goal;
    private Transform start;
    private Vector3Int startCell;
    private Vector2Int startIndex;

    public Button ResetButton;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        ResetButton.onClick.AddListener(ResetTile);

        tileData = new int[mapData.mapWidth, mapData.mapHeight];
        isSpecialTile = new bool[mapData.mapWidth, mapData.mapHeight];
        isResourceTile = new bool[mapData.mapWidth, mapData.mapHeight];

        for (int x = 0; x < mapData.mapWidth; x++)
        {
            for (int y = 0; y < mapData.mapHeight; y++)
            {
                tileData[x, y] = BLOCK;
                //isSpecialTile[x, y] = false;
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

    void Update() //조금 추가했습니다 -여영부-
    {
        if (!ExploitationState.isOn)
        {
            return;

            //if (Input.GetKeyDown(KeyCode.R))
            //{
            //    for (int x = 0; x < mapData.mapWidth; x++)
            //    {
            //        for (int y = 0; y < mapData.mapHeight; y++)
            //        {
            //            tileData[x, y] = BLOCK;
            //            Vector3Int tilePos = new Vector3Int(x + tilemap.origin.x, y + tilemap.origin.y, 0);
            //            tilemap.SetTile(tilePos, isSpecialTile[x, y] ? specialTile : groundTile);
            //        }
            //    }
            //}
        }

        if (GameManager.Instance != null &&
        GameManager.Instance.CurrentState != GameState.Ready)
        {
            return;
        }  //웨이브단계 개척불가용 코드 -여영부-

        if (Input.GetMouseButton(0))
        {
            HandleTile(false);
        }
        else if (Input.GetMouseButton(1))
        {
            HandleTile(true);
        }

        if (CheckComplete())
        {
            Debug.Log("연결 완료");  // 업데이트에 넣어서 계속떠서 주석처리 좀 할게요 -여영부-
        }
        //Debug.Log($"{CheckComplete()}");
    }

    public void ResetTile()
    {
        int n = 0;
        for (int x = 0; x < mapData.mapWidth; x++)
        {
            for (int y = 0; y < mapData.mapHeight; y++)
            {
                
                Vector3Int tilePos = new Vector3Int(x + tilemap.origin.x, y + tilemap.origin.y, 0);
                if (isSpecialTile[x, y]) tilemap.SetTile(tilePos, specialTile);
                else if (isResourceTile[x, y]) tilemap.SetTile(tilePos, resourceTile);
                else if (tileData[x, y] == 0)
                {
                    n += 1;
                    tilemap.SetTile(tilePos, groundTile);
                }
                else tilemap.SetTile(tilePos, groundTile);
                tileData[x, y] = BLOCK;

            }
        }
        ResourceSystem.Instance.AddCrystal(crystalRefunded * n);
        Debug.Log($"{n}개 타일 복구: {crystalRefunded * n}개 회수");
        audioSource.PlayOneShot(resetClip);
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
                PlayTileSound(restorationClip);
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
                    PlayTileSound(exploitationClip);
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

        //tileData[x, y] = BLOCK;
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
            //Debug.Log($"인덱스: ({x},{y})");

            if (x < 0 || x >= mapData.mapWidth || y < 0 || y >= mapData.mapHeight) //맵 끝과 붙어있으면 연결성 검사 안되는 문제
                return false;
            //Debug.Log($"{x}, {y}");

            if (tileData[x, y] != PATH)
                return false;
        }

        return true; 
    }

    void PlayTileSound(AudioClip clip)
    {
        if (canPlaySound)
        {
            audioSource.PlayOneShot(clip);
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