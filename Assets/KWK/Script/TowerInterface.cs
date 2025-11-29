using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface TowerInterface
{
    TowerSO GetTowerData();
    void Upgrade();
    void Destroy();
    void SetLevel(int newLevel);
    int GetLevel();
}
