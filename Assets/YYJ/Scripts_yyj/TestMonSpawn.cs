using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMonSpawn : MonoBehaviour
{
    public GameObject monsterPrefab;
    // MapSO에 시작점이 (0, 4)로 되어있네요
    public Vector3 spawnPosition = new Vector3(0, 4, 0);

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (monsterPrefab != null && TilemapReader_YYJ.Instance != null)
            {
                Instantiate(monsterPrefab, spawnPosition, Quaternion.identity);
                Debug.Log("YYJ 테스트 몬스터 스폰!");
            }
            else
            {
                Debug.LogError("Monster Prefab이 없거나, YYJ_TilemapReader가 씬에 없습니다!");
            }
        }
    }
}
