using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Defensweet/SpecialTiles")]
public class SpecialTilesSO : ScriptableObject
{
    [System.Serializable]
    public class StageData
    {
        public int stageNumber;
        public List<Vector2Int> positions;
    }

    public List<StageData> stages = new List<StageData>();

    private void OnValidate()
    {
        if (stages == null || stages.Count == 0)
        {
            stages = new List<StageData>()
            {
                new StageData()
                {
                    stageNumber = 1,
                    positions = new List<Vector2Int>()
                    {
                        new Vector2Int(2, 3),
                        new Vector2Int(2, 4),
                        new Vector2Int(2, 5)
                    }
                },
                new StageData()
                {
                    stageNumber = 2,
                    positions = new List<Vector2Int>()
                    {
                        new Vector2Int(1, 1),
                        new Vector2Int(3, 7)
                    }
                }
            };
        }
    }
}