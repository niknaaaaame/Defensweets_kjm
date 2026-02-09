using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Unity.VisualScripting;
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

    private enum Orientation { Default, Right, Up, Left }
    private Orientation ghostOrientation = Orientation.Default;
    private Sprite ghostDefaultSprite;

    private Transform ghostRangeTransform;
    private Quaternion ghostRangeDefaultRotation;
    private BoxCollider2D ghostBoxCollider;

    private bool isSpaklingLaser = false;

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
                ApplyOrientationToGhost(Orientation.Default);
            }
            else if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                ApplyOrientationToGhost(Orientation.Up);
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
                if (isSpaklingLaser)
                {
                    BoxCollider2D placedBox = placed.GetComponent<BoxCollider2D>();
                    placedBox.offset = ghostBoxCollider.offset;
                    placedBox.size = ghostBoxCollider.size;
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
                ghostRangeTransform = null;
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

        ghostRangeTransform = FindChildRecursive(ghostTower.transform, "Range");
        ghostRangeDefaultRotation = ghostRangeTransform.localRotation;
        ghostBoxCollider = ghostTower.GetComponent<BoxCollider2D>();

        isSpaklingLaser = false;
        isSpaklingLaser = towerPrefab.GetComponent<SpaklingLaser>() != null;

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

        float angle = 0f;

        Sprite spriteToApply = null;
        switch (orientation)
        {
            case Orientation.Left:
                spriteToApply = leftSprite;
                if (isSpaklingLaser)
                {
                    ghostBoxCollider.offset = new Vector2(-2.5f, -0.26f);
                    ghostBoxCollider.size = new Vector2(6f, 1f);
                    ghostRangeTransform.localPosition = new Vector3(-2.76f, -0.26f, 0);
                    angle = 90f;
                }
                break;

            case Orientation.Right:
                spriteToApply = rightSprite;
                if (isSpaklingLaser)
                {
                    ghostBoxCollider.offset = new Vector2(2.5f, -0.26f);
                    ghostBoxCollider.size = new Vector2(6f, 1f);
                    ghostRangeTransform.localPosition = new Vector3(2.24f, -0.26f, 0);
                    angle = 90f;
                }
                break;

            case Orientation.Up:
                spriteToApply = backSprite;
                if (isSpaklingLaser)
                {
                    ghostBoxCollider.offset = new Vector2(0, -2.76f);
                    ghostBoxCollider.size = new Vector2(1f, 6f);
                    ghostRangeTransform.localPosition = new Vector3(0, 2.5f, 0);
                    angle = 0f;
                }
                break;

            case Orientation.Default:
                spriteToApply = ghostDefaultSprite;
                if (isSpaklingLaser)
                {
                    ghostBoxCollider.offset = new Vector2(0, -2.76f);
                    ghostBoxCollider.size = new Vector2(1f, 6f);
                    ghostRangeTransform.localPosition = new Vector3(0, -2.5f, 0);
                    angle = 0f;
                }
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
        ghostRangeTransform.localRotation = Quaternion.Euler(0, 0, angle);
    }

    Transform FindChildRecursive(Transform parent, string name)
    {
        foreach (Transform child in parent)
        {
            if (child.name == name) return child;
            Transform found = FindChildRecursive(child, name);
            if (found != null) return found;
        }
        return null;
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
