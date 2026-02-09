using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class StagePathConnector : MonoBehaviour
{
    [Header("Path Settings")]
    public GameObject pathIconPrefab;
    public float iconSpacing = 1.5f;    //이미지 간격

    [Header("Connection Info")]
    public Transform nextStage;

    private string saveKey;

    // Start is called before the first frame update
    void Start()
    {
        if (nextStage == null) return;

        saveKey = $"Path_{gameObject.name}_To_{nextStage.name}";

        if (PlayerPrefs.GetInt(saveKey, 0) == 1)
        {
            DrawPathImmediate();
        }

    }

    public void OpenPath()
    {
        if (nextStage == null) return;

        if (PlayerPrefs.GetInt(saveKey, 0) == 1) return;

        PlayerPrefs.SetInt(saveKey, 1);
        PlayerPrefs.Save();

        DrawPathImmediate();
    }
    // 이미지 배치
    private void DrawPathImmediate()
    {
        Vector3 startPos = transform.position;
        Vector3 endPos = nextStage.position;
        
        float distance = Vector3.Distance(startPos, endPos);
        Vector3 direction = (endPos - startPos).normalized;
        // 거리에 따라 배치할 아이콘 개수 계산
        int count = Mathf.FloorToInt(distance / iconSpacing);
        // 시작점 i => 0은 처음 1은 2번째 부터
        for (int i = 1; i < count; i++)
        {
            Vector3 spawnPos = startPos + (direction * (i * iconSpacing));

            GameObject icon = Instantiate(pathIconPrefab, spawnPos, Quaternion.identity);

            icon.transform.SetParent(this.transform);
        }
    }
    // 테스트용
    [ContextMenu("Reset Path Data")]
    public void ResetPathData()
    {
        if (string.IsNullOrEmpty(saveKey)) 
            saveKey = $"Path_{gameObject.name}_To_{nextStage.name}";

        PlayerPrefs.DeleteKey(saveKey);
        Debug.Log($"{saveKey} 데이터 삭제");
    }

}
