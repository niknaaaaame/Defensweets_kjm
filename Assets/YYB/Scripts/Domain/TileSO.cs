using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Defensweet/Tile")]
public class TileSO : ScriptableObject
{
    public string tileName;
    public Sprite sprite;
    public bool isWalkable;     // 이동 가능한지
    public bool isBuildable;    // 타워 설치 가능한지
    public TileEffectType effectType; // 타일 효과(없으면 None)
}

