using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.Analytics;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Stage Data")]
    public StageSO stage;         // 이 씬에서 사용할 스테이지 데이터

    public GameState CurrentState { get; private set; }
    private int currentWaveIndex = 0;   // 아직 시작 전

    private int gateHp;           // 성문 체력(또는 침투 허용치)
    private int aliveMonsters = 0; // 필드에 살아있는 몬스터 수

    [SerializeField] private TMPro.TextMeshProUGUI gateHpText;

    [Header("Result UI")]
    [SerializeField] private GameObject successPanel;   // 클리어 패널
    [SerializeField] private GameObject failPanel;      // 실패 패널

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void Start()
    {
        InitStage();
        // 스테이지 초기 세팅
        ResourceSystem.Instance.Setup(stage.initialSugar, stage.initialCrystal);

        // 이벤트 연결
        EventBus.Subscribe(Events.OnMonsterKilled, (obj) => OnMonsterKilled());
        EventBus.Subscribe(Events.OnWaveCleared, (obj) => OnWaveCleared((int)obj));

        SetState(GameState.Ready);
        // 준비가 끝나면 StartNextWave()를 버튼/타이머로 호출
    }
    public Transform spawnTransform;  // 스폰 지점(월드)
    public Transform goalTransform;   // 목표 지점(월드)

    void InitStage()
    {
        gateHp = stage.gateHp;
        currentWaveIndex = 0;
        aliveMonsters = 0;
        UpdateGateHpUI();

        if (successPanel != null) successPanel.SetActive(false);
        if (failPanel != null) failPanel.SetActive(false);

        SetState(GameState.Ready);
    }
    void UpdateGateHpUI()
    {
        if (gateHpText != null)
            gateHpText.text = $"Gate : {gateHp}/{stage.gateHp}";
    }

    public void StartNextWave()
    {
        Debug.Log($"[GM] StartNextWave 호출, 현재 상태 = {CurrentState}");

        if (CurrentState != GameState.Ready)
        {
            Debug.LogWarning("[GM] Ready 상태가 아니라 웨이브 시작 불가");
            return;
        }

        if (CurrentState != GameState.Ready) return;
        if (currentWaveIndex >= stage.waves.Length)
        {
            Debug.Log("[GM] 모든 웨이브를 이미 클리어했습니다.");
            return;
        }
        // 1) 경로 계산 & 잠금
        bool ok = PathSystem.Instance.ComputeAndLockPath(spawnTransform.position, goalTransform.position);
        if (!ok)
        {
            Debug.LogWarning("[GameManager] 경로가 없어 웨이브를 시작할 수 없습니다.");
            return;
        }

        // 2) 웨이브 진행
        SetState(GameState.Wave);

        StartCoroutine(WaveSystem.Instance.RunWave(
            stage.waves[currentWaveIndex],
            onSpawned: () => { aliveMonsters++; },
            onOneRemoved: () => { aliveMonsters = Mathf.Max(0, aliveMonsters - 1); CheckWaveEnd(); }
        ));
    }

    private void CheckWaveEnd()
    {
        // 스폰이 끝났고 필드에 몬스터가 하나도 없을 때 → 웨이브 종료
        if (aliveMonsters > 0 || !WaveSystem.Instance.IsSpawnFinished)
            return;

        Debug.Log($"[GM] Wave {currentWaveIndex} 종료");

        // 웨이브 클리어 이벤트
        EventBus.Publish(Events.OnWaveCleared, currentWaveIndex);

        // 다음 웨이브로 인덱스 이동
        currentWaveIndex++;

        // 경로 잠금 해제 (다음 Ready에서 다시 길 개척 가능)
        PathSystem.Instance.Unlock();

        // 모든 웨이브를 끝냈으면 → 성공 처리
        if (currentWaveIndex >= stage.waves.Length)
        {
            OnAllWavesCleared();
        }
        else
        {
            SetState(GameState.Ready);
        }
    }


    private void OnWaveEnded()
    {
        // 내부 전환 + 경로 해제
        PathSystem.Instance.Unlock();
        SetState(GameState.Ready);

        // UI/사운드용 이벤트 브로드캐스트
        OnWaveCleared(currentWaveIndex);
    }

    public void OnMonsterReachGoal()
    {
        gateHp--;
        if (gateHp < 0) gateHp = 0;
        UpdateGateHpUI();
        if (gateHp <= 0) OnFailed();
    }
    
    private void OnMonsterKilled()
    {
        /*
        var reward = stage.waves[currentWaveIndex].reward;
        if (reward.sugar > 0) ResourceSystem.Instance.AddSugar(reward.sugar); */
    }
    
    private void OnWaveCleared(int waveIndex)
    {
        // UI 연출/사운드 등
    }

    private void OnAllWavesCleared()
    {
        SetState(GameState.Result);
        Debug.Log("[GM] 모든 웨이브 클리어 → SUCCESS");
        ShowSuccess();
        // 결과 UI(클리어) 호출
    }

    private void OnFailed()
    {
        Debug.Log("Gate HP 0 → Game Over");
        SetState(GameState.Result);
        ShowFail();
        // 결과 UI(실패) 호출
    }

    private void SetState(GameState next)
    {
        if (CurrentState == next) return; // 중복 전이 방지
        CurrentState = next;
        EventBus.Publish(Events.OnStateChanged, next);
    }

    private void ShowSuccess()
    {
        SetState(GameState.Result);
        if (successPanel != null) successPanel.SetActive(true);
    }

    private void ShowFail()
    {
        SetState(GameState.Result);
        if (failPanel != null) failPanel.SetActive(true);
    }
}
