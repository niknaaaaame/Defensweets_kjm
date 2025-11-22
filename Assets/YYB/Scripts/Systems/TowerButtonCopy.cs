using UnityEngine;

public class TowerButtonCopy : MonoBehaviour
{
    public GameObject towerPrefab;

    private TowerSO data;

    void Awake()
    {
        TowerInterface tower = towerPrefab.GetComponent<TowerInterface>();
        data = tower.GetTowerData();
    }

    public void OnClick()
    {
        TowerTest.Instance.SelectTower(towerPrefab, data.levels[0].cost);
    }
}
