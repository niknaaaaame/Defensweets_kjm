using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.Analytics;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Stage Data")]
    public StageSO stage;         // 이 씬에서 사용할 스테이지 데이터

    public GameState CurrentState { get; private set; }
    private int currentWaveIndex = -1;   // 아직 시작 전

    private int gateHp;           // 성문 체력(또는 침투 허용치)
    private int aliveMonsters = 0; // 필드에 살아있는 몬스터 수

    [SerializeField] private TMPro.TextMeshProUGUI gateHpText;

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
        currentWaveIndex = -1;
        UpdateGateHpUI();
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

        // 1) 경로 계산 & 잠금
        bool ok = PathSystem.Instance.ComputeAndLockPath(spawnTransform.position, goalTransform.position);
        if (!ok)
        {
            Debug.LogWarning("[GameManager] 경로가 없어 웨이브를 시작할 수 없습니다.");
            return;
        }

        // 2) 웨이브 진행
        currentWaveIndex++;
        SetState(GameState.Wave);

        StartCoroutine(WaveSystem.Instance.RunWave(
            stage.waves[currentWaveIndex],
            onSpawned: () => { aliveMonsters++; },
            onOneRemoved: () => { aliveMonsters = Mathf.Max(0, aliveMonsters - 1); CheckWaveEnd(); }
        ));
    }

    private void CheckWaveEnd()
    {
        if (aliveMonsters <= 0 && WaveSystem.Instance.IsSpawnFinished)
        {
            // 보상은 WaveSystem 쪽에서
            EventBus.Publish(Events.OnWaveCleared, currentWaveIndex);
            SetState(GameState.Ready);
            PathSystem.Instance.Unlock();
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
        var reward = stage.waves[currentWaveIndex].reward;
        if (reward.sugar > 0) ResourceSystem.Instance.AddSugar(reward.sugar);
    }
    
    private void OnWaveCleared(int waveIndex)
    {
        // UI 연출/사운드 등
    }

    private void OnAllWavesCleared()
    {
        SetState(GameState.Result);
        // 결과 UI(클리어) 호출
    }

    private void OnFailed()
    {
        Debug.Log("Gate HP 0 → Game Over");
        SetState(GameState.Result);
        // 결과 UI(실패) 호출
    }

    private void SetState(GameState next)
    {
        if (CurrentState == next) return; // 중복 전이 방지
        CurrentState = next;
        EventBus.Publish(Events.OnStateChanged, next);
    }
}
