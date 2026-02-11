using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour   //타이머 코드 수정했습니다 -여영부-
{
    [Header("준비 단계 시간 (초)")]
    [SerializeField] private float readyDuration = 60f;      // 준비 단계 1분

    [Header("UI")]
    public Text timerText;                                   // 타이머 / 몬스터수 표시용 텍스트

    private float readyTimeLeft;                             // 준비 단계 남은 시간
    private GameState lastState;
    private bool autoWaveTriggered;

    private void Start()
    {
        readyTimeLeft = readyDuration;

        if (GameManager.Instance != null)
        {
            lastState = GameManager.Instance.CurrentState;
        }
        else
        {
            lastState = GameState.Ready;
        }

        UpdateDisplay();
    }

    private void Update()
    {
        if (GameManager.Instance == null || timerText == null)
            return;

        GameState state = GameManager.Instance.CurrentState;

        // 상태가 바뀌었을 때 처리
        if (state != lastState)
        {
            OnStateChanged(state);
            lastState = state;
        }

        // 상태별 동작
        if (state == GameState.Ready)
        {
            if (!autoWaveTriggered)
            {
                if (readyTimeLeft > 0f)
                {
                    readyTimeLeft -= Time.deltaTime;
                    if (readyTimeLeft < 0f)
                        readyTimeLeft = 0f;
                }

                // 0초 도달 시: 여기서만 경로 실패 ⇒ 게임 실패
                if (readyTimeLeft <= 0f)
                {
                    GameManager.Instance.StartNextWave(true); // failIfNoPath = true
                    autoWaveTriggered = true;
                }
            }
        }

        // 화면에 표시 업데이트
        UpdateDisplay();
    }

    private void OnStateChanged(GameState newState)
    {
        // Ready 상태로 들어갈 때마다 타이머를 1분으로 리셋
        if (newState == GameState.Ready)
        {
            readyTimeLeft = readyDuration;
            autoWaveTriggered = false;
        }
    }

    private void UpdateDisplay()
    {
        GameState state = GameManager.Instance.CurrentState;

        switch (state)
        {
            case GameState.Ready:
                // mm:ss 형태로 출력
                int minutes = (int)(readyTimeLeft / 60);
                int seconds = (int)(readyTimeLeft % 60);
                timerText.text = $"{minutes}:{seconds:00}";
                break;

            case GameState.Wave:
                // 웨이브 상태에서는 남은 몬스터 수 표시
                int remain = GameManager.Instance.AliveMonsterCount;
                timerText.text = $"{remain}";
                break;

            case GameState.Result:
                // 결과 상태에서는 그냥 비우거나, 원하면 "결과" 같은 텍스트로
                timerText.text = "";
                break;

            default:
                timerText.text = "";
                break;
        }
    }
}
