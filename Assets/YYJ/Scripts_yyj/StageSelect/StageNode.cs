using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageNode : MonoBehaviour
{
    [SerializeField] private GameObject infoCanvas;
    [SerializeField] private float padding = 1.5f;

    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (infoCanvas != null) infoCanvas.SetActive(false);
    }

    public void ShowInfo(bool show)
    {
        if (infoCanvas != null) infoCanvas.SetActive(show);
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
}
