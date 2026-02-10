using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface TowerInterface
{
    TowerSO GetTowerData();
    void Upgrade();
    void Destroy();
    void Heal(int amount);
    float GetEnergy();
}

public static class TowerRefundUtility
{
    private const float RefundRate = 0.5f;

    public static void RefundTowerCost(TowerSO towerData, int currentLevel)
    {
        if (towerData == null || towerData.levels == null || towerData.levels.Length == 0)
        {
            return;
        }

        int totalSugarCost = towerData.levels[0].cost;
        int totalCrystalCost = 0;

        for (int i = 0; i < currentLevel && i < towerData.levels.Length; i++)
        {
            totalSugarCost += towerData.levels[i].upgradeCostSugar;
            totalCrystalCost += towerData.levels[i].specialCostCrystal;
        }

        int refundSugar = Mathf.RoundToInt(totalSugarCost * RefundRate);
        int refundCrystal = Mathf.RoundToInt(totalCrystalCost * RefundRate);

        if (refundSugar > 0)
        {
            ResourceSystem.Instance.AddSugar(refundSugar);
        }

        if (refundCrystal > 0)
        {
            ResourceSystem.Instance.AddCrystal(refundCrystal);
        }
    }
}
