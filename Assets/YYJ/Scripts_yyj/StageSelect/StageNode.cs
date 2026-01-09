using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum StageState // 스테이지 상태 정의
{
    Locked,     // 잠김 (회색)
    Unlocked,   // 플레이 가능 (붉은색)
    Cleared     // 클리어됨 (연두색)
}
public class StageNode : MonoBehaviour
{
    [Header("스테이지 번호")]
    public int stageID;

    [Header("잠김 해재 조건 스테이지")]
    public int requiredStageID = 0; // 이 스테이지를 클리어해야 잠김 해재
    
    [Header("이동할 씬")]
    [SerializeField] private string sceneName;
    
    [Header("UI 세팅")]
    [SerializeField] private GameObject infoCanvas;
    [SerializeField] private GameObject mapCanvas;

    [Header("스테이지 상태 표시")]
    [SerializeField] private SpriteRenderer statusIndicator; // 스테이지 상태 표시용 스프라이트 렌더러
    [SerializeField] private Color lockedColor = Color.gray;
    [SerializeField] private Color unlockedColor = Color.red;
    [SerializeField] private Color clearedColor = Color.green;

    [Header("여백")]
    [SerializeField] private float padding = 1f;

    [Header("보상 표시")]
    [SerializeField] private GameObject rewardCanvas;

    //public float overrideZoomSize = 0f; // 0보다 크면 이 값으로 설정

    [Header("연결 선 시각화")]
    public StageNode parentStageNode; // 연결된 이전 스테이지 (선 시작점)
    [SerializeField] private LineRenderer pathLine;
    [SerializeField] private float drawDuration = 0.5f; // 선이 그려지는 시간

    private bool isLineDrawn = false;
        
    private SpriteRenderer spriteRenderer;
    public StageState CurrentState { get; private set; } // 현재 상태 프로퍼티

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (infoCanvas != null) infoCanvas.SetActive(false);
        if (mapCanvas != null) mapCanvas.SetActive(false);
        if (rewardCanvas != null) rewardCanvas.SetActive(false);

        if (pathLine != null)   // 라인 렌더러 초기화
        {
            pathLine.positionCount = 2; // 시작점, 끝점 2개
            pathLine.enabled = false;   // 처음엔 꺼둠
        }
        // 렌더링 순서 자동 정리
        if (spriteRenderer != null)
        {
            // 기준 노드 순서
            int baseOrder = spriteRenderer.sortingOrder;
            // 연결 선은 노드보다 1칸 위
            if (pathLine != null)
            {
                pathLine.sortingLayerID = spriteRenderer.sortingLayerID;    // 같은 레이어
                pathLine.sortingOrder = baseOrder + 1;
            }
            // 상태는 1칸 더 위
            if (statusIndicator != null)
            {
                statusIndicator.sortingLayerID = spriteRenderer.sortingLayerID;
                statusIndicator.sortingOrder = baseOrder + 2;
            }
        }

        UpdateState();  // 게임 시작 시 상태 체크 및 색상 적용
    }

    public void UpdateState()
    {   // 이미 클리어 했는지 확인
        bool isCleared = PlayerPrefs.GetInt($"Stage_{stageID}_Cleared", 0) == 1;
        
        if (isCleared)
        {
            CurrentState = StageState.Cleared;
        }
        else
        {
            bool isUnlocked = requiredStageID == 0 || PlayerPrefs.GetInt($"Stage_{requiredStageID}_Cleared", 0) == 1;
            if (isUnlocked == true) CurrentState = StageState.Unlocked;
            else CurrentState = StageState.Locked;
        }

        UpdateVisuals();
        UpdatePathLine();   // 상태에 따라 선 그리기
    }

    private void UpdateVisuals()
    {
        if (statusIndicator == null) return;

        switch (CurrentState)
        {
            case StageState.Locked:
                statusIndicator.color = lockedColor;
                break;
            case StageState.Unlocked:
                statusIndicator.color = unlockedColor;
                break;
            case StageState.Cleared:
                statusIndicator.color = clearedColor;
                break;
        }
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
        if (!string.IsNullOrEmpty(sceneName))
        {   
            SceneManager.LoadScene(sceneName);
        }
        else Debug.LogWarning("연결된 씬이 없습니다.");
    }

    public void OpenMap()
    {
        if (mapCanvas != null) mapCanvas.SetActive(true);
        else Debug.LogWarning("맵UI가 연결되지 않았습니다.");
    }

    public void CloseMap()
    {
        if (mapCanvas != null) mapCanvas.SetActive(false);
    }

    public void ShowReward(bool show)
    {
        if (rewardCanvas != null)
        {
            rewardCanvas.SetActive(show);
        }
    }

    private void UpdatePathLine()
    {
        // 연결 부모 노드 or 라인 렌더러 없으면 패스
        if (parentStageNode == null || pathLine == null) return;
        // 잠김 상태라면 선을 끄고 리턴
        if (CurrentState == StageState.Locked)
        {
            pathLine.enabled = false;
            isLineDrawn = false;
            return;
        }
        // 선이 안그려져 있다면 그리기
        if (!isLineDrawn)
        {
            StartCoroutine(DrawLineRoutine());
        }
    }

    private IEnumerator DrawLineRoutine()
    {
        isLineDrawn = true;
        pathLine.enabled = true;

        Vector3 startPos = parentStageNode.transform.position;
        Vector3 endPos = transform.position;
        // z값 조정
        startPos.z = 0f;
        endPos.z = 0f;

        pathLine.SetPosition(0, startPos);
        pathLine.SetPosition(1, startPos);  // 처음엔 끝점도 시작점에 위치

        float timer = 0f;
        while (timer < drawDuration)
        {
            timer += Time.deltaTime;
            float t = timer / drawDuration;
            // 선형 보간으로 끝점을 서서히 이동
            Vector3 currentEndPos = Vector3.Lerp(startPos, endPos, t);
            pathLine.SetPosition(1, currentEndPos);

            yield return null;
        }
        // 최종 위치 확정
        pathLine.SetPosition(1, endPos);
    }

    // 이 밑은 테스트용
    [ContextMenu("테스트: 이 스테이지 클리어 처리")]
    public void TestClearStage()
    {
        PlayerPrefs.SetInt($"Stage_{stageID}_Cleared", 1);
        PlayerPrefs.Save();
        Debug.Log($"[Test] 스테이지 {stageID} 클리어 처리");

        StageNode[] allNodes = FindObjectsOfType<StageNode>();
        foreach (var node in allNodes)
        {
            node.UpdateState();
        }
    }

    [ContextMenu("테스트: 데이터 전체 초기화")]
    public void TestResetData()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        Debug.Log("[Test] 모든 세이브 데이터 초기화");

        StageNode[] allNodes = FindObjectsOfType<StageNode>();
        foreach (var node in allNodes)
        {
            node.UpdateState();
        }
    }
}
