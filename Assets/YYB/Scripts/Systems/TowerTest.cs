using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class TowerTest : MonoBehaviour
{
    public static TowerTest Instance { get; private set; }

    [SerializeField] private Tilemap tilemap;

    [Header("Path Edit Toggle")]
    [SerializeField] private Toggle exploitationToggle;

    private GameObject selectedTower;
    private GameObject ghostTower;

    private int installCost;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Update()
    {
        if (exploitationToggle != null && exploitationToggle.isOn)
        {
            if (ghostTower != null)
            {
                Destroy(ghostTower);
                ghostTower = null;
                selectedTower = null;
            }
            return;
        }

        if (ghostTower == null) return;

        // 마우스 위치 → 타일 좌표로 스냅
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorld.z = 0;

        Vector3Int cellPos = tilemap.WorldToCell(mouseWorld);
        Vector3 snappedPos = tilemap.GetCellCenterWorld(cellPos);
        ghostTower.transform.position = snappedPos;

        // 설치 가능 여부 : 맵 안 + 길이 아닌 곳(= 타일이 있는 곳)
        bool inBounds = TilemapReader_YYJ.Instance.IsWithinBounds(cellPos);
        bool isPath = TilemapReader_YYJ.Instance.IsWalkable(cellPos);   // null = 길
        bool canBuildHere = inBounds && !isPath;                         // 길이 아니어야 설치

        if (Input.GetMouseButtonDown(0) && canBuildHere)
        {
            if (ResourceSystem.Instance.TryUseSugar(installCost))
            {
                Instantiate(selectedTower, snappedPos, Quaternion.identity);
                ClearSelection();
            }
            else
            {
                Debug.Log("설탕이 부족합니다!");
            }
        }
        else if (Input.GetMouseButtonDown(1))
        {
            ClearSelection();
        }
    }

    public void SelectTower(GameObject towerPrefab, int cost)
    {
        if (ghostTower != null)
        {
            Destroy(ghostTower);
        }

        selectedTower = towerPrefab;
        ghostTower = Instantiate(towerPrefab);
        SetLayerAlpha(ghostTower, 0.5f); 

        installCost = cost;
    }

    private void ClearSelection()
    {
        if (ghostTower != null) Destroy(ghostTower);
        ghostTower = null;
        selectedTower = null;
    }

    private void SetLayerAlpha(GameObject obj, float alpha)
    {
        var sr = obj.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            Color c = sr.color;
            c.a = alpha;
            sr.color = c;
        }

        foreach (Transform child in obj.transform)
        {
            SetLayerAlpha(child.gameObject, alpha);
        }
    }
}
