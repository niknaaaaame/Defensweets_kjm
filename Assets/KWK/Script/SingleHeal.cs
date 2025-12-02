using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEditor.U2D.Aseprite;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class SingleHeal : MonoBehaviour, TowerInterface
{
    public TowerSO towerData;
    public TowerSO GetTowerData() => towerData;

    [HideInInspector] public int level = 0;

    private List<Collider2D> targets = new List<Collider2D>();
    private Coroutine healCoroutine;

    // Start is called before the first frame update
    void Start()
    {
        
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
        }

        if (healCoroutine == null)
        {
            healCoroutine = StartCoroutine(heal());
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

    IEnumerator heal()
    {
        while (targets.Count > 0)
        {
            float lowestEnergy = float.MaxValue;
            int lowestTowerIndex = 0;

            for (int i=0; i < targets.Count -1; i++)
            {
                TowerInterface tower = targets[i].GetComponent<TowerInterface>();
                float tempEnergy = tower.GetEnergy();
                if (tempEnergy < lowestEnergy)
                {
                    lowestEnergy = tempEnergy;
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
        if (level < 2)
        {
            level += 1;
            TowerInfoPanel.Instance.ShowTowerInfo(this.gameObject, level);
        }
        else
        {
            Debug.Log("Max Level Reached");
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
