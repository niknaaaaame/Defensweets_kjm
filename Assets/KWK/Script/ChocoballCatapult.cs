using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChocoballCatapult : MonoBehaviour, TowerInterface
{
    public TowerSO towerData;
    public TowerSO GetTowerData() => towerData;

    [HideInInspector] public int level = 0;

    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform shootPoint;
    [SerializeField] private Transform energyBar;
    [SerializeField] private BoxCollider2D outerRange;
    [SerializeField] private BoxCollider2D innerRange;

    private List<Collider2D> targets = new List<Collider2D>();
    private Coroutine shootCoroutine;
    private Vector3 originalScale;
    private float energy = 100f;
    public float GetEnergy() => energy;

    // Start is called before the first frame update
    void Start()
    {
        energy = 100;
        originalScale = energyBar.localScale;
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
        JudgmentTarget(other);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Monster"))
        {
            return;
        }
        JudgmentTarget(other);
    }

    bool IsValidTarget(Collider2D target)
    {
        return outerRange.IsTouching(target) && !innerRange.IsTouching(target);
    }

    void JudgmentTarget(Collider2D other)
    {
        if (IsValidTarget(other))
        {
            targets.Add(other);
            if (shootCoroutine == null)
            {
                shootCoroutine = StartCoroutine(shoot());
            }
        }
        else
        {
            targets.Remove(other);
            if (targets.Count == 0 && shootCoroutine != null)
            {
                StopCoroutine(shootCoroutine);
                shootCoroutine = null;
            }
        }
    }

    IEnumerator shoot()
    {
        while (targets.Count > 0)
        {
            GameObject instance = Instantiate(bulletPrefab, shootPoint.position, shootPoint.rotation);

            Bullet bullet = instance.GetComponent<Bullet>();
            bullet.Setting(targets[0].transform, towerData.levels[level].damage);

            energy -= towerData.levels[0].usingEnergy;
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
                outerRange.size = new Vector2(towerData.levels[level].range, towerData.levels[level].range);
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
}
