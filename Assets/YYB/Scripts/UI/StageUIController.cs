using UnityEngine;
using UnityEngine.UI;

public class StageUIController : MonoBehaviour
{
    [Header("개척 토글 / 웨이브 버튼")]
    [SerializeField] private Toggle exploitationToggle;   // 개척 토글
    [SerializeField] private Button startWaveButton;      // 웨이브 시작 버튼

    [Header("경로 체크 설정")]
    [SerializeField] private float pathCheckInterval = 0.3f;

    private float pathCheckTimer;
    private bool hasPath = false;

    private void Start()
    {
        if (exploitationToggle != null)
            exploitationToggle.onValueChanged.AddListener(OnExploitationToggleChanged);

        pathCheckTimer = 0f;
        ForcePathRecheck();
        UpdateUIState();
    }

    private void OnDestroy()
    {
        if (exploitationToggle != null)
            exploitationToggle.onValueChanged.RemoveListener(OnExploitationToggleChanged);
    }

    private void Update()
    {
        if (GameManager.Instance == null)
            return;

        // Ready 상태일 때만 주기적으로 경로 재검사
        bool isReady = GameManager.Instance.CurrentState == GameState.Ready;

        pathCheckTimer -= Time.deltaTime;
        if (isReady && pathCheckTimer <= 0f)
        {
            pathCheckTimer = pathCheckInterval;
            ForcePathRecheck();
        }

        UpdateUIState();
    }

    private void OnExploitationToggleChanged(bool isOn)
    {
        // 토글이 켜지면 StartWave를 막는 로직만 다시 반영
        UpdateUIState();
    }

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

    private void UpdateUIState()
    {
        if (GameManager.Instance == null)
            return;

        GameState state = GameManager.Instance.CurrentState;
        bool isReady = state == GameState.Ready;
        bool exploitationOn = exploitationToggle != null && exploitationToggle.isOn;

        // 개척 토글은 Ready 상태에서만 조작 가능
        if (exploitationToggle != null)
        {
            exploitationToggle.interactable = isReady;

            if (!isReady && exploitationToggle.isOn)
            {
                exploitationToggle.isOn = false;
                exploitationOn = false;
            }
        }

        // StartWave 버튼 활성 조건:
        // Ready 상태 && 개척 토글 OFF && 경로 있음
        if (startWaveButton != null)
        {
            bool canStart = isReady && !exploitationOn && hasPath;
            startWaveButton.interactable = canStart;
        }
    }
}
