using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Defensweet/Wave")]
public class WaveSO : ScriptableObject
{
    public float prepareTime;      // 웨이브 시작 전 준비 시간(Ready 페이즈 길이)
    public SpawnGroup[] spawns;    // 스폰 묶음(종류/수량/간격)
    public Reward reward;          // 웨이브 종료 보상(설탕가루 등)
}

[System.Serializable]
public struct SpawnGroup
{
    //public MonsterSO monster;
    public MonsterSO_YYJ monster;
    public int count;
    public float interval;
    public float startDelay;
}

[System.Serializable]
public struct Reward
{
    public int sugar;          // 웨이브 클리어 시 지급
    public int crystalBonus;   // 옵션: 필요 없으면 0
}
