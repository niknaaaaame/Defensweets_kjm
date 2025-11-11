using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName = "Defensweet/SpecialTiles")]
public class StageTilesSO : ScriptableObject
{

    public enum TileType
    {
        Special,
        Currency
    }

    [System.Serializable]
    public class StageData
    {
        public int stageNumber;
        public TileType type;
        public TileBase tileAsset;
        public List<Vector2Int> positions;
    }

    public List<StageData> stages = new List<StageData>();

}