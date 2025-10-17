using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Defensweet/Monster")]
public class MonsterSO : ScriptableObject
{
    public string monsterName;
    public MonsterType monsterType;
    public GameObject prefab;  // Monster.cs가 붙은 프리팹
    public int hp;
    public float speed;
    public int rewardSugar;
    public float slowResist;   // 0~1 (슬로우 저항), 필요 없으면 0
    public bool splitsOnDeath; // 젤리 큐브 같은 분열형
}