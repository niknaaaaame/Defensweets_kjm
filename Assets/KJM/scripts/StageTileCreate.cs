using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class StageTileCreate : MonoBehaviour
{
    public StageTilesSO tilesData;
    public Tilemap tilemap;
    //public TileBase specialTile;
    //public StageTilesSO.TileType createType = StageTilesSO.TileType.Special; //기본값 설정

    public int currentStage = 1; //임시 현재 스테이지 번호
    //타워 설치 시 특수 타일 판단은 TileBase 에셋으로 확인

    void Start()
    {
        if (tilesData == null || tilemap == null)
            return;

        var stages = tilesData.stages.FindAll(s => s.stageNumber == currentStage);
        if (stages == null)
        {
            Debug.LogWarning($"스테이지 {currentStage} 데이터 없음!");
            return;
        }

        foreach (var stage in stages) 
        {
            foreach (var pos in stage.positions) 
            {
                Vector3Int tilePos = new Vector3Int(pos.x + tilemap.origin.x, pos.y + tilemap.origin.y, 0);
                tilemap.SetTile(tilePos, stage.tileAsset);
            }
        }
    }
}