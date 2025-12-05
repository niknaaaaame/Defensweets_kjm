using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StageNode : MonoBehaviour
{
    [Header("이동할 씬")]
    [SerializeField] private string sceneName;  // 이동할 스테이지 씬 이름
    [Header("인포 캔버스")]
    [SerializeField] private GameObject infoCanvas;
    [SerializeField] private float padding = 1f;
    [Header("맵 캔버스")]
    [SerializeField] private GameObject mapCanvas;
        
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (infoCanvas != null) infoCanvas.SetActive(false);
        if (mapCanvas != null) mapCanvas.SetActive(false);
    }

    public void ShowInfo(bool show)
    {
        if (infoCanvas != null) infoCanvas.SetActive(show);

        if (mapCanvas != null && !show) mapCanvas.SetActive(false);
    }
    
    // 자신과 자식 캔버스 포함한 전체 크기 계산 및 반환
    public Bounds GetTotalBounds()
    {
        Bounds b = spriteRenderer.bounds;

        if (infoCanvas != null)
        {
            RectTransform rt = infoCanvas.GetComponent<RectTransform>();
            if (rt != null)
            {
                Vector3[] corners = new Vector3[4];
                rt.GetWorldCorners(corners);
                foreach (Vector3 corner in corners)
                {
                    b.Encapsulate(corner);
                }
            }
        }
        // 여백 추가
        b.Expand(padding);
        return b;
    }

    public void EnterStage()
    {
        if (!string.IsNullOrEmpty(sceneName)) SceneManager.LoadScene(sceneName);
        else Debug.LogWarning("연결된 씬이 없습니다.");
    }

    public void OpenMap()
    {
        if (mapCanvas != null) mapCanvas.SetActive(true);
    }

    public void CloseMap()
    {
        if (mapCanvas != null) mapCanvas.SetActive(false);
    }
}
