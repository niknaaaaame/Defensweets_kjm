using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Defensweet/Tower")]
public class TowerSO : ScriptableObject
{
    public string towerName;
    public TowerType towerType;
    public Sprite icon;
    public GameObject prefab;
    public TowerLevel[] levels;    // 1~3단계 설정
}

[System.Serializable]
public struct TowerLevel
{
    public int damage;
    public float attackSpeed;  
    public float range;        
    public int upgradeCostSugar;
    public int specialCostCrystal;

    public int cost;
    public int usingEnergy;

    public Sprite preview;
    public string name;
    public string summary;
}
