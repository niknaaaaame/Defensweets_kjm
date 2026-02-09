using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WorldNode : MonoBehaviour
{
    [Header("월드 설정")]
    public int worldID; // 월드 번호
    [SerializeField] private string stageSelectSceneName; // 이동할 스트이지 선택 씬 이름

    [Header("해금 조건")]
    public int requiredClearStageID = 0; // 해금에 필요한 클리어 스테이지 ID (0이면 무조건 해금)

    [Header("UI")]
    [SerializeField] private GameObject infoCanvas;

    [Header("시각적 요소")]
    [SerializeField] private SpriteRenderer statusIndicator;
    [SerializeField] private Color lockedColor = Color.gray; // 잠긴 상태 색상
    [SerializeField] private Color unlockedColor = Color.white; // 해금된 상태 색상
    [SerializeField] private GameObject lockIcon;   // 잠금 이미지 있을 시
    public GameObject dimmerObject;

    public bool IsUnlocked { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        UpdateWorldState();
        SetDimmed(false);
        if (infoCanvas != null) infoCanvas.SetActive(false);
    }

    public void SetDimmed(bool isActive)
    {
        if (dimmerObject != null)
        {
            dimmerObject.SetActive(isActive);
        }
    }

    public void UpdateWorldState()
    {
        // 해금 조건 확인
        if (requiredClearStageID == 0 || PlayerPrefs.GetInt($"Stage_{requiredClearStageID}_Cleared", 0) == 1)
        {
            IsUnlocked = true;
        }
        else
        {
            IsUnlocked = false;
        }

        UpdateVisuals();
    }

    private void UpdateVisuals()
    {
        if (statusIndicator != null)
        {
            if (IsUnlocked == true)
            {
                statusIndicator.color = unlockedColor;
            }
            else
            {
                statusIndicator.color = lockedColor;
            }
        }

        if (lockIcon != null)
        {
            lockIcon.SetActive(!IsUnlocked);
        }
    }

    public void EnterWorld()
    {
        if (IsUnlocked)
        {
            if (!string.IsNullOrEmpty(stageSelectSceneName))
            {
                SceneManager.LoadScene(stageSelectSceneName);
            }
            else
            {
                Debug.LogWarning("월드에 연결된 씬이 없습니다.");
            }
        }
        else
        {
            Debug.Log("이 월드는 잠겨 있습니다.");
        }
    }

    public void ShowInfo(bool show)
    {
        if (!IsUnlocked) return;

        if (infoCanvas != null)
        {
            infoCanvas.SetActive(show);
        }
        else
        {
            Debug.LogWarning("인포 캔버스가 설정되지 않았습니다.");
        }
    }


    public void LoadWorldScene()
    {
        if (!string.IsNullOrEmpty(stageSelectSceneName))
        {
            SceneManager.LoadScene(stageSelectSceneName);
        }
        else
        {
            Debug.LogWarning("연결된 씬이 없습니다.");
        }
    }

}
