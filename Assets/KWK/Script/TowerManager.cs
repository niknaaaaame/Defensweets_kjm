using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TowerManager : MonoBehaviour
{
    public static TowerManager Instance { get; private set; }

    public Tilemap tilemap;

    private GameObject selectedTower;
    private GameObject ghostTower;

    private int installCost;

    private enum Orientation { Default, Left, Right, Back }
    private Orientation ghostOrientation = Orientation.Default;
    private Sprite ghostDefaultSprite;

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
        if (ghostTower != null)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0;

            Vector3Int cellPos = tilemap.WorldToCell(mousePos);
            Vector3 snappedPos = tilemap.GetCellCenterWorld(cellPos);
            snappedPos.y += 0.16f; // Adjust for tower pivot

            ghostTower.transform.position = snappedPos;

            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                ApplyOrientationToGhost(Orientation.Left);
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                ApplyOrientationToGhost(Orientation.Right);
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                ApplyOrientationToGhost(Orientation.Back);
            }
            else if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                ApplyOrientationToGhost(Orientation.Default);
            }

            if (Input.GetMouseButtonDown(0))
            {
                Sprite chosenSprite = GetGhostRootSprite(ghostTower);

                Destroy(ghostTower);
                GameObject placed = Instantiate(selectedTower, snappedPos, Quaternion.identity);

                var placedRootSr = placed.GetComponent<SpriteRenderer>();
                if(placedRootSr != null && chosenSprite != null)
                {
                    placedRootSr.sprite = chosenSprite;
                }

                ghostTower = null;
                selectedTower = null;
                ghostOrientation = Orientation.Default;

                ResourceSystem.Instance.TryUseSugar(installCost);
            }

            else if (Input.GetMouseButtonDown(1))
            {
                Destroy(ghostTower);
                ghostTower = null;
                selectedTower = null;
                ghostOrientation = Orientation.Default;
                ghostDefaultSprite = null;
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

        ghostDefaultSprite = ghostTower.GetComponent<SpriteRenderer>().sprite;
        ghostOrientation = Orientation.Default;

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

    Sprite GetGhostRootSprite(GameObject ghost)
    {
        var sr = ghost.GetComponent<SpriteRenderer>();
        return sr.sprite;
    }

    void ApplyOrientationToGhost(Orientation orientation)
    {
        Sprite leftSprite = null;
        Sprite rightSprite = null;
        Sprite backSprite = null;

        var comps = ghostTower.GetComponents<MonoBehaviour>();
        foreach (var comp in comps)
        {
            if (comp == null) continue;
            var type = comp.GetType();
            var leftField = type.GetField("left", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            var rightField = type.GetField("right", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            var backField = type.GetField("back", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

            if (leftField != null && leftField.FieldType == typeof(Sprite))
            {
                leftSprite = leftField.GetValue(comp) as Sprite;
            }
            if (rightField != null && rightField.FieldType == typeof(Sprite))
            {
                rightSprite = rightField.GetValue(comp) as Sprite;
            }
            if (backField != null && backField.FieldType == typeof(Sprite))
            {
                backSprite = backField.GetValue(comp) as Sprite;
            }
        }

        Sprite spriteToApply = null;
        switch (orientation)
        {
            case Orientation.Left:
                spriteToApply = leftSprite;
                break;

            case Orientation.Right:
                spriteToApply = rightSprite;
                break;

            case Orientation.Back:
                spriteToApply = backSprite;
                break;

            case Orientation.Default:
                spriteToApply = ghostDefaultSprite;
                break;
        }

        var rootSr = ghostTower.GetComponent<SpriteRenderer>();
        if(rootSr != null)
        {
            if(spriteToApply != null)
            {
                rootSr.sprite = spriteToApply;
            }
            else
            {
                Debug.LogWarning("해당 타워 프리팹에 left/right 스프라이트 필드가 없습니다: " + ghostTower.name);
            }
        }
        ghostOrientation = orientation;
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
