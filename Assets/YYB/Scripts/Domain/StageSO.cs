using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Defensweet/Stage")]
public class StageSO : ScriptableObject
{
    public string stageId;
    public int initialSugar;    // 시작 설탕
    public int initialCrystal;  // 시작 크리스탈(스테이지마다 초기화)
    public int gateHp;          // 성문 체력(또는 침투 허용치)
    public WaveSO[] waves;      // 이 스테이지의 웨이브(3~6개)
}
