using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Defensweet/Tower")]
public class TowerSO : ScriptableObject
{
    public string towerName;
    public TowerType towerType;
    public Sprite icon;
    public GameObject prefab;      // Tower.cs가 붙은 프리팹
    public TowerLevel[] levels;    // 1~3단계 설정
}

[System.Serializable]
public struct TowerLevel
{
    public int damage;
    public float attackSpeed;   // 초당 공격(편의상)
    public float range;         // 유닛 거리 단위(타일 기준 환산)
    public int upgradeCostSugar;
    public int specialCostCrystal; // 특수 강화 없으면 0

    public int energy;
}
