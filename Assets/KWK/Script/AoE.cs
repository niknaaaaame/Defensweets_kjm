using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class AoE : MonoBehaviour, TowerInterface
{
    public TowerSO towerData;
    public TowerSO GetTowerData() => towerData;

    private float damageMultiplier = 1f; //여기도 특수타일용 변수 추가 -여영부-

    public int level = 0;

    [SerializeField] private GameObject prefab;
    [SerializeField] private Transform shootPoint;
    [SerializeField] private float prefabDestroyTime;

    private List<Collider2D> targets = new List<Collider2D>();
    private Coroutine shootCoroutine;

    // Start is called before the first frame update
    void Start()
    {
        ApplyTileEffect(); //-여영부-
    }

    // Update is called once per frame
    void Update()
    {
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

            yield return new WaitForSeconds(towerData.levels[0].attackSpeed);
        }

        shootCoroutine = null;
    }

    public void SetLevel(int newLevel)
    {
        level = newLevel;
    }

    public int GetLevel()
    {
        return level;
    }

    public void Upgrade()
    {
        TowerManager.Instance.UpgradeTower(towerData, level);
    }

    public void Destroy()
    {
        Destroy(this.gameObject);
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