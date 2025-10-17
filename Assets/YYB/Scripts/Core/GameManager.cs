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

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void Start()
    {
        // 스테이지 초기 세팅
        gateHp = stage.gateHp;
        ResourceSystem.Instance.Setup(stage.initialSugar, stage.initialCrystal);

        // 이벤트 연결
        EventBus.Subscribe(Events.OnMonsterKilled, (obj) => OnMonsterKilled());
        EventBus.Subscribe(Events.OnWaveCleared, (obj) => OnWaveCleared((int)obj));

        SetState(GameState.Ready);
        // 준비가 끝나면 StartNextWave()를 버튼/타이머로 호출
    }
    /*
    public Transform spawnTransform;  // 스폰 지점(월드 좌표)
    public Transform goalTransform;   // 목표 지점(월드 좌표)
    public TileSystem tileSystem;

    public void StartNextWave()
    {
        if (CurrentState != GameState.Ready) return;

        // 1) 현재 연결된 타일 상태로 최단경로 계산 + 잠금
        var startCell = PathSystem.Instance.WorldToCell(spawnTransform.position);
        var goalCell = PathSystem.Instance.WorldToCell(goalTransform.position);
        var ok = PathSystem.Instance.ComputeAndLockPath(
            tileSystem.GetWalkableCells(), startCell, goalCell);

        if (!ok)
        {
            // 경로가 없으면 웨이브 시작 불가(경고/가이드)
            Debug.LogWarning("경로가 없어 웨이브를 시작할 수 없습니다.");
            return;
        }

        // 2) 웨이브 진행
        SetState(GameState.Wave);
        StartCoroutine(WaveSystem.Instance.RunWave(
            stage.waves[++currentWaveIndex],
            onSpawned: () => aliveMonsters++,
            onOneDied: () => { aliveMonsters--; CheckWaveEnd(); }));
    }
    */
    private void CheckWaveEnd()
    {
        // 스폰 완료 & 필드 몬스터 0 → 웨이브 종료
        if (aliveMonsters <= 0 && WaveSystem.Instance.IsSpawnFinished)
        {
            // 보상 지급
            var reward = stage.waves[currentWaveIndex].reward;
            if (reward.sugar > 0) ResourceSystem.Instance.AddSugar(reward.sugar);
            // (옵션) 크리스탈 보너스도 여기서

            EventBus.Publish(Events.OnWaveCleared, currentWaveIndex);
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
        if (gateHp <= 0) OnFailed();
    }

    private void OnMonsterKilled()
    {
        // 필요 시 글로벌 카운트/콤보 등 처리
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
        SetState(GameState.Result);
        // 결과 UI(실패) 호출
    }

    private void SetState(GameState next)
    {
        CurrentState = next;
        EventBus.Publish(Events.OnStateChanged, next);
        // Ready: 길/타워 조작 허용, Wave: 일부 제한, Result: UI 표시
    }
}
