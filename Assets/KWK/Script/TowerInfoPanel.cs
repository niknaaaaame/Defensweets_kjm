using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TowerInfoPanel : MonoBehaviour
{
    public static TowerInfoPanel Instance;

    public GameObject towerInfoPanel;

    [SerializeField] TextMeshProUGUI towerNameText;
    [SerializeField] TextMeshProUGUI towerInfoText;
    [SerializeField] Button upgradeButton;
    [SerializeField] Button destroyButton;

    private Transform currentTower = null;
    private TowerSO towerData = null;
    private TowerInterface towerInterface = null;

    private void Awake()
    {
        Instance = this;
        towerInfoPanel.SetActive(false);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ShowTowerInfo(Transform transform)
    {
        currentTower = transform;

        Vector3 screenPos = Camera.main.WorldToScreenPoint(currentTower.position);
        
        towerInfoPanel.transform.position = screenPos + new Vector3(115, 0, 0);

        towerInfoPanel.SetActive(true);
    }

    public void HideTowerInfo()
    {
        towerInfoPanel.SetActive(false);
    }

    public void ToggleTowerInfo(GameObject tower, int level)
    {
        level += 1;
        towerInterface = tower.GetComponent<TowerInterface>();
        towerData = towerInterface.GetTowerData();

        towerNameText.text = $"{towerData.towerName}";
        towerInfoText.text = $"- level {level} -\ndamage: {towerData.levels[level].damage}" +
            $"\nattackspeed: {towerData.levels[level].attackSpeed}\nrange: {towerData.levels[level].range}\n---------------------\n" +
            $"upgrade cost\nsugar: {towerData.levels[level].upgradeCostSugar}\ncrystal: {towerData.levels[level].specialCostCrystal}";

        if (towerInfoPanel.activeSelf && currentTower == tower.transform)
        {
            HideTowerInfo();
        }
        else
        {
            ShowTowerInfo(tower.transform);
        }
    }

    public void OnCilckUpgrade()
    {
        towerInterface.Upgrade();
    }

    public void OnClickDestroy()
    {
        towerInterface.Destroy();
        HideTowerInfo();
    }
}
