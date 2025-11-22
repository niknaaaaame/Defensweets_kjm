using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MonsterCountUI : MonoBehaviour     // GameManager에서 받아오는 방식 웨이브로 소환해야지 읽혀짐 다른 방식의 소환은 작동 X
{       
    [SerializeField] private Text countText;
    [SerializeField] private string pretext = "Monsters : ";
    
    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance != null)
        {
            // countText.text = $"{pretext}{GameManager.Instance.AliveMonsterCount}"; // 일단 잠시 주석
        }
    }
}
