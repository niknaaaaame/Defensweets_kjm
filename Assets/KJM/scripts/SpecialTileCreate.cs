using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SpecialTileCreate : MonoBehaviour
{
    public SpecialTilesSO special;
    public Tilemap tilemap;
    public TileBase specialTile;

    public int currentStage = 1; //임시 현재 스테이지 번호
    //타워 설치 시 특수 타일 판단은 TileBase 에셋으로 확인

    void Start()
    {
        if (special == null || tilemap == null || specialTile == null)
        {
            Debug.LogWarning("SpecialTileCreate: 필요한 값이 비어 있음!");
            return;
        }

        var stage = special.stages.Find(s => s.stageNumber == currentStage);
        if (stage == null)
        {
            Debug.LogWarning($"스테이지 {currentStage} 데이터 없음!");
            return;
        }

        foreach (var pos in stage.positions)
        {
            Vector3Int tilePos = new Vector3Int(pos.x + tilemap.origin.x, pos.y + tilemap.origin.y, 0);
            tilemap.SetTile(tilePos, specialTile);
        }

        //Debug.Log($"스테이지 {currentStage}의 특수 타일 {stage.positions.Count}개 설치 완료!");
    }
}