using UnityEngine;
using UnityEngine.UI;

public class StageUIController : MonoBehaviour
{
    [Header("웨이브 시작 버튼")]
    [SerializeField] private Button startWaveButton;      // 웨이브 시작 버튼

    [Header("경로 체크 설정")]
    [SerializeField] private float pathCheckInterval = 0.3f; // Ready 상태에서 경로 재검사 주기(초)

    private float pathCheckTimer;
    private bool hasPath = false;

    private void Start()
    {
        // 처음 한 번은 바로 경로 체크 + UI 갱신
        pathCheckTimer = 0f;
        ForcePathRecheck();
        UpdateUIState();
    }

    private void Update()
    {
        if (GameManager.Instance == null)
            return;

        GameState state = GameManager.Instance.CurrentState;
        bool isReady = state == GameState.Ready;

        // Ready 상태일 때만 주기적으로 경로 재검사
        pathCheckTimer -= Time.deltaTime;
        if (isReady && pathCheckTimer <= 0f)
        {
            pathCheckTimer = pathCheckInterval;
            ForcePathRecheck();
        }

        // 상태/경로에 따라 버튼 인터랙션 갱신
        UpdateUIState();
    }

    /// <summary>
    /// 현재 Start/Goal 기준으로 유효한 경로가 있는지 검사한다.
    /// </summary>
    private void ForcePathRecheck()
    {
        hasPath = false;

        if (GameManager.Instance == null ||
            PathSystem.Instance == null ||
            GameManager.Instance.spawnTransform == null ||
            GameManager.Instance.goalTransform == null)
        {
            return;
        }

        hasPath = PathSystem.Instance.HasValidPath(
            GameManager.Instance.spawnTransform.position,
            GameManager.Instance.goalTransform.position
        );
    }

    /// <summary>
    /// GameState + 경로 유무에 따라 UI(특히 Start 버튼)를 갱신한다.
    /// </summary>
    private void UpdateUIState()
    {
        if (GameManager.Instance == null)
            return;

        GameState state = GameManager.Instance.CurrentState;
        bool isReady = state == GameState.Ready;

        // StartWave 버튼 활성 조건:
        // Ready 상태 && 유효한 경로 있음
        if (startWaveButton != null)
        {
            startWaveButton.interactable = isReady && hasPath;
        }
    }

    /// <summary>
    /// 다른 스크립트에서 즉시 경로 재검사를 요청하고 싶을 때 호출 가능.
    /// (안 써도 상관 없음)
    /// </summary>
    public void RequestImmediatePathRecheck()
    {
        pathCheckTimer = 0f;
        ForcePathRecheck();
        UpdateUIState();
    }
}
