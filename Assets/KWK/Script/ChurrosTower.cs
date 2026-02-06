using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChurrosTower : MonoBehaviour
{
    public TowerSO towerData;
    public TowerSO GetTowerData() => towerData;

    [HideInInspector] public int level = 0;

    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform shootPoint;
    [SerializeField] private Transform energyBar;
    [SerializeField] private LineRenderer lr;
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

    private float damageMultiplier = 1f;

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalScale = energyBar.localScale;
        ApplyTileEffect();
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

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Monster"))
        {
            return;
        }

        if (!targets.Contains(other))
        {
            targets.Add(other);
        }

        if (shootCoroutine == null)
        {
            shootCoroutine = StartCoroutine(shoot());
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Monster"))
        {
            return;
        }

        targets.Remove(other);

        if (targets.Count == 0 && shootCoroutine != null)
        {
            StopCoroutine(shootCoroutine);
            shootCoroutine = null;
        }
    }

    IEnumerator shoot()
    {
        while (targets.Count > 0)
        {
            GameObject instance = Instantiate(bulletPrefab, shootPoint.position, shootPoint.rotation);

            Chocochip bullet = instance.GetComponent<Chocochip>();
            //bullet.Setting(targets[0].transform, towerData.levels[level].damage);

            int baseDamage = towerData.levels[level].damage;  //여기서 부터
            int finalDamage = Mathf.RoundToInt(baseDamage * damageMultiplier);
            bullet.Setting(targets[0].transform, finalDamage);  //여기까지 특수타일 배수 구현때문에 살짝 바꿨어-여영부-


            energy -= towerData.levels[level].usingEnergy;
            if (energy <= 0)
            {
                energy = 0;
                yield break;
            }

            yield return new WaitForSeconds(towerData.levels[level].attackSpeed);
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

    private void ApplyTileEffect()   //특수타일 공격력 증가 -여영부-
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
