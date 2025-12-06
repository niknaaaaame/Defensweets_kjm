using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState { Ready, Wave, Result }          // 스테이지 루프 상태
public enum TowerType { Basic, Slow, AoE, Stun, Heal }       // 타워 타입 구분 (12/02 Heal 추가: 김원기)
public enum MonsterType { Normal, Fast, Tank, Split }  // 몬스터 타입 구분
public enum TileEffectType{ None, Sticky, Explosive, SweetBoost } // 타일 타입 구분
public enum TargetType { Ground, Air }            // 타겟 타입 구분 (지상, 비행)