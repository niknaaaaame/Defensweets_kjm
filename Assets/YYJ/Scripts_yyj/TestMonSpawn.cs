using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMonSpawn : MonoBehaviour
{
    public GameObject monsterPrefab1_Normal;
    public GameObject monsterPrefab2_Tank;
    public GameObject monsterPrefab3_Fast;
    public GameObject monsterPrefab4_Split;

    public Transform departPoint;

    void Start()
    {
        GameObject departObject = GameObject.Find("Depart");

        if (departObject != null)
        {
            departPoint = departPoint.transform;
        }
    }

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
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            SpawnMonster(monsterPrefab4_Split);
        }
    }

    void SpawnMonster(GameObject prefab)
    {
        if (prefab != null && TilemapReader_YYJ.Instance != null && departPoint != null)
        {
            Instantiate(prefab, departPoint.position, Quaternion.identity);
        }
        else
        {
            if (departPoint == null)
            {
                Debug.LogError("소환에 실패");
            }
        }
    }
}
