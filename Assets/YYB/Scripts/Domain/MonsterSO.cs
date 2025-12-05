using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Defensweet/Monster")]
public class MonsterSO : ScriptableObject
{
    public string monsterName;
    public MonsterType monsterType;
    public GameObject prefab;
    public int hp;
    public float speed;
    public int rewardSugar;
    public float slowResist;
    public bool splitsOnDeath;

    [Header("Split Ability")]
    public MonsterSO_YYJ splitMonsterSO;
    public int splitCount;
}