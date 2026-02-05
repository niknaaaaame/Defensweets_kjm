using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class SpaklingLaser : MonoBehaviour, TowerInterface
{
    public TowerSO towerData;
    public TowerSO GetTowerData() => towerData;

    private float damageMultiplier = 1f; //여기도 특수타일용 변수 추가 -여영부-

    [HideInInspector] public int level = 0;

    [SerializeField] private GameObject prefab;
    [SerializeField] private Transform shootPoint;
    [SerializeField] private Transform energyBar;
    [SerializeField] private float prefabDestroyTime;
    [SerializeField] private Sprite lev3;
    [SerializeField] private Sprite left;
    [SerializeField] private Sprite right;
    [SerializeField] private Sprite back;

    private SpriteRenderer spriteRenderer;
    private List<Collider2D> targets = new List<Collider2D>();
    private Coroutine shootCoroutine;
    private Vector3 originalScale;
    private float energy = 100f;
    public float GetEnergy() => energy;

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalScale = energyBar.localScale;

        ApplyTileEffect(); //-여영부-
    }

    // Update is called once per frame
    void Update()
    {
        // 에너지 바 갱신
        float ratio = energy / 100;
        energyBar.localScale = new Vector3(originalScale.x * ratio, originalScale.y, originalScale.z);

        float widthDifference = originalScale.x - energyBar.localScale.x;
        energyBar.localPosition = new Vector3(-widthDifference / 2f, energyBar.localPosition.y, energyBar.localPosition.z);

        // 정보창 표시
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0;
            RaycastHit2D[] hits = Physics2D.RaycastAll(mousePos, Vector2.zero);

            foreach (RaycastHit2D hit in hits)
            {
                if (hit.collider is PolygonCollider2D && hit.collider.gameObject == this.gameObject)
                {
                    TowerInfoPanel.Instance.ToggleTowerInfo(this.gameObject, level);
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(!other.CompareTag("Monster"))
        {
            return;
        }

        if(!targets.Contains(other))
        {
            targets.Add(other);
        }

        if(shootCoroutine == null)
        {
            shootCoroutine = StartCoroutine(shoot());
        }

    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if(!other.CompareTag("Monster"))
        {
            return;
        }

        targets.Remove(other);

        if(targets.Count == 0 && shootCoroutine != null)
        {
            StopCoroutine(shootCoroutine);
            shootCoroutine = null;
        }
    }

    private IEnumerator shoot()
    {
        while (targets.Count > 0)
        {
            GameObject instance = Instantiate(prefab, shootPoint.position, shootPoint.rotation);

            Destroy(instance, prefabDestroyTime);

            for (int i = targets.Count - 1; i >= 0; i--)
            {
                var target = targets[i];
                if (target == null)
                {
                    targets.RemoveAt(i);
                    continue;
                }

                int baseDamage = towerData.levels[level].damage; //-여영부-
                int finalDamage = Mathf.RoundToInt(baseDamage * damageMultiplier); //-여영부-

                MonsterTest monster = target.GetComponent<MonsterTest>();
                if (monster != null)
                {
                    //monster.TakeDamage(towerData.levels[0].damage);
                    monster.TakeDamage(finalDamage); //여기도 특수타일때메 바꿨어 -여영부-
                }
            }

            energy -= towerData.levels[0].usingEnergy;
            if (energy < 0)
            {
                energy = 0;
                yield break;
            }

            yield return new WaitForSeconds(towerData.levels[0].attackSpeed);
        }

        shootCoroutine = null;
    }

    public void Upgrade()
    {
        switch (level)
        {
            case 0:
                ResourceSystem.Instance.TryUseSugar(towerData.levels[level].upgradeCostSugar);
                level = 1;
                TowerInfoPanel.Instance.ShowTowerInfo(this.gameObject, level);
                break;
            case 1:
                ResourceSystem.Instance.TryUseSugar(towerData.levels[level].upgradeCostSugar);
                ResourceSystem.Instance.TryUseSugar(towerData.levels[level].specialCostCrystal);
                level = 2;
                spriteRenderer.sprite = lev3;
                TowerInfoPanel.Instance.ShowTowerInfo(this.gameObject, level);
                break;
            case 2:
                Debug.Log("Max Level Reached");
                break;
        }
    }

    public void Destroy()
    {
        Destroy(this.gameObject);
    }

    public void Heal(int amount)
    {
        energy += amount;

        if (energy > 100)
        {
            energy = 100;
        }
    }

    private void ApplyTileEffect()
    {
        if (TilemapReader_YYJ.Instance == null)
            return;

        TileEffectType effect = TilemapReader_YYJ.Instance.GetEffectAtWorldPos(transform.position);

        switch (effect)
        {
            case TileEffectType.SweetBoost:
                damageMultiplier = 1.5f;
                break;

            default:
                damageMultiplier = 1f;
                break;
        }
    }
}