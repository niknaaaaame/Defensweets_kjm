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

                ghostTower = null;
                selectedTower = null;

                ResourceSystem.Instance.TryUseSugar(installCost);
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

        var colliders = ghostTower.GetComponentsInChildren<Collider2D>();
        foreach (var col in colliders)
        {
            col.enabled = false;
        }

        var rbs = ghostTower.GetComponentsInChildren<Rigidbody2D>();
        foreach (var rb in rbs)
        {
            rb.simulated = false;
        }

        var lineRenderers = ghostTower.GetComponentsInChildren<LineRenderer>();
        foreach (var lr in lineRenderers)
        {
            lr.enabled = true;
        }

        SetLayerAlpha(ghostTower, 0.5f);

        installCost = cost;
    }

    void SetLayerAlpha(GameObject obj, float alpha)
    {
        SpriteRenderer sr = obj.GetComponent<SpriteRenderer>();

        if (obj.name == "background" || obj.name == "fill")
        {
            sr.enabled = false;
        }
        else
        {
            Color color = sr.color;
            color.a = alpha;
            sr.color = color;
        }

        foreach (Transform child in obj.transform)
        {
            SetLayerAlpha(child.gameObject, alpha);
        }
    }

    /*
     SpriteRenderer sr = obj.GetComponent<SpriteRenderer>();
        sr.enabled = true;

        Color color = sr.color;
        color.a = alpha;
        if (obj.name == "Range")
        {
            sr.color = color;
        }
        else if (obj.name == "background" || obj.name == "fill")
        {
            sr.enabled = false;
        }

        foreach (Transform child in obj.transform)
        {
            SetLayerAlpha(child.gameObject, alpha);
        }
    */
}
