using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TowerManager : MonoBehaviour
{
    public static TowerManager Instance { get; private set; }

    public Tilemap tilemap;

    private GameObject selectedTower;
    private GameObject ghostTower;

    private int installCost;
    
    private Dictionary<string, int> defaultLevels = new Dictionary<string, int>();

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(ghostTower != null)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0;

            Vector3Int cellPos = tilemap.WorldToCell(mousePos);
            Vector3 snappedPos = tilemap.GetCellCenterWorld(cellPos);

            ghostTower.transform.position = snappedPos;

            if (Input.GetMouseButtonDown(0))
            {
                Destroy(ghostTower);
                GameObject placed = Instantiate(selectedTower, snappedPos, Quaternion.identity);

                TowerInterface towerInterface = placed.GetComponent<TowerInterface>();
                if(towerInterface != null)
                {
                    int def = GetDefaultLevel(towerInterface.GetTowerData().towerName);
                    towerInterface.SetLevel(def);
                }

                ghostTower = null;
                selectedTower = null;

                ResourceTest.Instance.UseSugar(installCost);
            }

            else if (Input.GetMouseButtonDown(1))
            {
                Destroy(ghostTower);
                ghostTower = null;
                selectedTower = null;
            }
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
        ghostTower.GetComponent<Collider2D>().enabled = false;

        SetLayerAlpha(ghostTower, 0.5f);

        installCost = cost;
    }

    void SetLayerAlpha(GameObject obj, float alpha)
    {
        SpriteRenderer sr = obj.GetComponent<SpriteRenderer>();
        
        Color color = sr.color;
        color.a = alpha;
        sr.color = color;

        foreach (Transform child in obj.transform)
        {
            SetLayerAlpha(child.gameObject, alpha);
        }
    }

    public int GetDefaultLevel(string towerName)
    {
        if (defaultLevels.TryGetValue(towerName, out int value))
        {
            return value;
        }

        return 0;
    }

    public void SetDefaultLevel(string towerName, int level)
    {
        defaultLevels[towerName] = level;
    }

    public void UpgradeTower(TowerSO towerData, int level)
    {
        if (level < 3)
        {
            ResourceTest.Instance.UseSugar(towerData.levels[level].upgradeCostSugar);

            MonoBehaviour[] all = FindObjectsOfType<MonoBehaviour>();
            foreach (MonoBehaviour mb in all)
            {
                TowerInterface towerInterface = mb as TowerInterface;
                if (towerInterface == null)
                {
                    continue;
                }

                TowerSO data = towerInterface.GetTowerData();
                if (data.towerName == towerData.towerName)
                {
                    int currentLevel = towerInterface.GetLevel();
                    towerInterface.SetLevel(currentLevel + 1);
                }
            }
            SetDefaultLevel(towerData.towerName, level + 1);
        }
        else
        {
            Debug.Log("Max Level");
        }
    }
}
