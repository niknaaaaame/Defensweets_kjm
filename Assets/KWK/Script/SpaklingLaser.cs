using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
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
    
    [SerializeField] private Sprite left;
    [SerializeField] private Sprite right;
    [SerializeField] private Sprite back;
    [SerializeField] private Sprite front3;
    [SerializeField] private Sprite left3;
    [SerializeField] private Sprite right3;
    [SerializeField] private Sprite back3;

    private SpriteRenderer spriteRenderer;
    private List<Collider2D> targets = new List<Collider2D>();
    private Coroutine shootCoroutine;
    private Vector3 originalScale;
    private float energy = 100f;

    private AudioSource audioSource;
    public float GetEnergy() => energy;

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
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
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }

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

                //MonsterTest monster = target.GetComponent<MonsterTest>(); -여영부-
                IDamageable monster = target.GetComponent<IDamageable>();
                if (monster != null)
                {
                    //monster.TakeDamage(towerData.levels[0].damage);
                    monster.TakeDamage(finalDamage); //여기도 특수타일때메 바꿨어 -여영부-
                }
            }

            energy -= towerData.levels[0].usingEnergy;
            audioSource.Play();

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
                //ResourceSystem.Instance.TryUseSugar(towerData.levels[level].upgradeCostSugar); -여영부-
                {
                    int sugarCost = towerData.levels[level].upgradeCostSugar;
                    if (ResourceSystem.Instance.Sugar < sugarCost)
                    {
                        Debug.Log("Not enough sugar to upgrade.");
                        return;
                    }

                    ResourceSystem.Instance.TryUseSugar(sugarCost);
                    level = 1;
                    TowerInfoPanel.Instance.ShowTowerInfo(this.gameObject, level);
                    break;
                }
            case 1:
                //ResourceSystem.Instance.TryUseSugar(towerData.levels[level].upgradeCostSugar); -여영부-
                //ResourceSystem.Instance.TryUseSugar(towerData.levels[level].specialCostCrystal);
                {
                    int sugarCost = towerData.levels[level].upgradeCostSugar;
                    int crystalCost = towerData.levels[level].specialCostCrystal;
                    if (ResourceSystem.Instance.Sugar < sugarCost || ResourceSystem.Instance.Crystal < crystalCost)
                    {

                        Debug.Log("Not enough resources to upgrade.");
                        return;
                    }

                    ResourceSystem.Instance.TryUseSugar(sugarCost);
                    ResourceSystem.Instance.TryUseCrystal(crystalCost);
                    level = 2;

                    if (spriteRenderer.sprite == left)
                    {
                        spriteRenderer.sprite = left3;
                    }
                    else if (spriteRenderer.sprite == right)
                    {
                        spriteRenderer.sprite = right3;
                    }
                    else if (spriteRenderer.sprite == back)
                    {
                        spriteRenderer.sprite = back3;
                    }
                    else
                    {
                        spriteRenderer.sprite = front3;
                    }
                    TowerInfoPanel.Instance.ShowTowerInfo(this.gameObject, level);
                    break;
                }
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