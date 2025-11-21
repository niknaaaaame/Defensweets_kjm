using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerButton : MonoBehaviour
{
    public GameObject towerPrefab;

    private TowerSO data;

    void Awake()
    {
        TowerInterface tower = towerPrefab.GetComponent<TowerInterface>();
        data = tower.GetTowerData();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnClick()
    {
        TowerManager.Instance.SelectTower(towerPrefab, data.levels[0].cost);
    }
}
