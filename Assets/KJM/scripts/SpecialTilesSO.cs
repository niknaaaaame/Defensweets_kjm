using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Defensweet/SpecialTiles")]
public class SpecialTilesSO : ScriptableObject
{
    //[System.Serializable] //직렬화(인스펙터에서 보임)
    //public class StageTileData //구조체 비슷한 형태, 스테이지 번호와 특수 타일 위치를 묶어서 저장
    //{
    //    public string stageNumber;            
    //    public List<Vector2Int> specialTiles; 
    //}

    //public List<StageTileData> stages = new List<StageTileData>(); //위에서 묶은 정보들을 담을 리스트를 생성 0번 인덱스에 스테이지 번호와 특수타일 위치가 모두 들어가 있음

    ////배열에 데이터를 추가하는 방식이므로 기존에 남아있는 배열 데이터를 초기화한 뒤 새로 추가해줘야 함. 추가하는 방식을 쓰는 이유는 vector2Int 배열의 길이를 모르기 때문
    //public void InitializeData()
    //{
    //    stages.Clear(); // 매번 새로 초기화

    //    var stage1 = new StageTileData()
    //    {
    //        stageNumber = "Stage1",
    //        specialTiles = new List<Vector2Int>()
    //        {
    //            new Vector2Int(1, 2),
    //            new Vector2Int(3, 4)
    //        }
    //    };

    //    var stage2 = new StageTileData()
    //    {
    //        stageNumber = "Stage2",
    //        specialTiles = new List<Vector2Int>()
    //        {
    //            new Vector2Int(0, 1),
    //            new Vector2Int(2, 5)
    //        }
    //    };

    //    stages.Add(stage1);
    //    stages.Add(stage2);
    //}



    //그냥 vector2int 리스트로 만들어서 인덱스로 스테이지 접근하기
}
