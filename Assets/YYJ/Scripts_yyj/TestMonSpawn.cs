using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMonSpawn : MonoBehaviour
{
    public GameObject monsterPrefab1_Normal;
    public GameObject monsterPrefab2_Tank;
    public GameObject monsterPrefab3_Fast;

    public Vector3 spawnPosition = new Vector3(0, 4, 0);

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SpawnMonster(monsterPrefab1_Normal);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SpawnMonster(monsterPrefab2_Tank);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SpawnMonster(monsterPrefab3_Fast);
        }
    }

    void SpawnMonster(GameObject prefab)
    {
        if (prefab != null && TilemapReader_YYJ.Instance != null)
        {
            Instantiate(prefab, spawnPosition, Quaternion.identity);
        }
        else
        {
            Debug.LogError("몬스터 소환에 실패했습니다.");
        }
    }
}
