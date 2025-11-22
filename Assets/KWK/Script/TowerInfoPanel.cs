using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerInfoPanel : MonoBehaviour
{
    public static TowerInfoPanel Instance;

    public GameObject towerInfoPanel;
    public GameObject panelText;

    private Transform currentTower = null;

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

    public void ToggleTowerInfo(Transform transform)
    {
        if (towerInfoPanel.activeSelf && currentTower == transform)
        {
            HideTowerInfo();
        }
        else
        {
            ShowTowerInfo(transform);
        }
    }
}
