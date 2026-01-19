using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Defensweet/StartPoint")]
public class StartPointSO : ScriptableObject
{
    [System.Serializable]
    public class StageData
    {
        public int stageNumber;
        public int startPosition;
        public int goalPosition;
    }

    public List<StageData> stages = new List<StageData>();
}
