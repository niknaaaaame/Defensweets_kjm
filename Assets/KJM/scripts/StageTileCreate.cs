using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class StageTileCreate : MonoBehaviour
{
    public StageTilesSO tilesData;
    public Tilemap tilemap;

    private int stageNumber;
    //타워 설치 시 특수 타일 판단은 TileBase 에셋으로 확인

    void Start()
    {
        stageNumber = TileEditor.Instance.currentStage;

        if (tilesData == null || tilemap == null)
            return;

        var stages = tilesData.stages.FindAll(s => s.stageNumber == stageNumber);
        if (stages == null)
        {
            Debug.LogWarning($"스테이지 {stageNumber} 데이터 없음!");
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