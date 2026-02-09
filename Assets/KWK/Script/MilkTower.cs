using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEditor.U2D.Aseprite;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class MilkTower : MonoBehaviour, TowerInterface
{
    public TowerSO towerData;
    public TowerSO GetTowerData() => towerData;

    [HideInInspector] public int level = 0;

    private List<Collider2D> targets = new List<Collider2D>();
    private Coroutine healCoroutine;
    [SerializeField] private float activationDelay = 0.2f;
    private bool isActive = false;

    [SerializeField] private Sprite left;
    [SerializeField] private Sprite right;
    [SerializeField] private Sprite back;
    [SerializeField] private Sprite front3;
    [SerializeField] private Sprite left3;
    [SerializeField] private Sprite right3;
    [SerializeField] private Sprite back3;

    private SpriteRenderer spriteRenderer;
    private AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
        StartCoroutine(activationDelayCoroutine());
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

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Tower"))
        {
            return;
        }

        if (!targets.Contains(other))
        {
            targets.Add(other);
            Debug.Log("Tower added to heal targets.");
        }

        if (healCoroutine == null && isActive)
        {
            healCoroutine = StartCoroutine(heal());
            audioSource.Play();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Tower"))
        {
            return;
        }

        targets.Remove(other);

        if (targets.Count == 0 && healCoroutine != null)
        {
            StopCoroutine(healCoroutine);
            healCoroutine = null;
        }
    }

    IEnumerator activationDelayCoroutine()
    {
        yield return new WaitForSeconds(activationDelay);
        isActive = true;

        if (targets.Count > 0 && healCoroutine == null)
        {
            healCoroutine = StartCoroutine(heal());
            audioSource.Play();
        }
    }

    IEnumerator heal()
    {
        while (targets.Count > 0)
        {
            float lowestEnergy = float.MaxValue;
            int lowestTowerIndex = 0;

            for (int i = 0; i < targets.Count; i++)
            {
                TowerInterface tower = targets[i].GetComponent<TowerInterface>();
                float temp = tower.GetEnergy();
                if (temp < lowestEnergy)
                {
                    lowestEnergy = temp;
                    lowestTowerIndex = i;
                }
            }

            TowerInterface targetTower = targets[lowestTowerIndex].GetComponent<TowerInterface>();
            targetTower.Heal(towerData.levels[level].damage);

            yield return new WaitForSeconds(towerData.levels[level].attackSpeed);
        }

        healCoroutine = null;
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
                //ResourceSystem.Instance.TryUseSugar(towerData.levels[level].upgradeCostSugar);
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
        return;
    }

    public float GetEnergy() => float.MaxValue;
}
