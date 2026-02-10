using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Stage Data")]
    public StageSO stage;

    public GameState CurrentState { get; private set; }
    private int currentWaveIndex = 0;

    private int gateHp;
    private int aliveMonsters = 0;
    public int AliveMonsterCount => aliveMonsters;

    [SerializeField] private TMPro.TextMeshProUGUI gateHpText;

    [Header("Result UI")]
    [SerializeField] private GameObject successPanel;
    [SerializeField] private GameObject failPanel;

    [Header("Tower Unlock")]
    [SerializeField] private int churrosUnlockStage = 3;
    [SerializeField] private string churrosTowerUnlockKey = "churros";

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void Start()
    {
        InitStage();
        ResourceSystem.Instance.Setup(stage.initialSugar, stage.initialCrystal);

        EventBus.Subscribe(Events.OnMonsterKilled, (obj) => OnMonsterKilled());
        EventBus.Subscribe(Events.OnWaveCleared, (obj) => OnWaveCleared((int)obj));

        SetState(GameState.Ready);
    }
    public Transform spawnTransform;
    public Transform goalTransform;   

    void InitStage()
    {
        gateHp = stage.gateHp;
        currentWaveIndex = 0;
        aliveMonsters = 0;
        UpdateGateHpUI();
        if (HeartHPUI.Instance != null)
        {
            HeartHPUI.Instance.Init(gateHp);
        }

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
        // 버튼에서 호출하는 기본 버전: 경로 없으면 실패 안 뜸
        StartNextWave(false);
    }

    public void StartNextWave(bool failIfNoPath)
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
        bool ok = PathSystem.Instance.ComputeAndLockPath(spawnTransform.position, goalTransform.position);
        if (!ok)
        {
            Debug.LogWarning("[GameManager] 경로가 없어 웨이브를 시작할 수 없습니다.");
            if (failIfNoPath)
            {
                OnFailed();
            }
            return;
        }

        SetState(GameState.Wave);

        StartCoroutine(WaveSystem.Instance.RunWave(
            stage.waves[currentWaveIndex],
            onSpawned: () => { aliveMonsters++; },
            onOneRemoved: () => { aliveMonsters = Mathf.Max(0, aliveMonsters - 1); CheckWaveEnd(); }
        ));
    }

    private void CheckWaveEnd()
    {
        if (aliveMonsters > 0 || !WaveSystem.Instance.IsSpawnFinished)
            return;

        Debug.Log($"[GM] Wave {currentWaveIndex} 종료");

        EventBus.Publish(Events.OnWaveCleared, currentWaveIndex);

        currentWaveIndex++;

        PathSystem.Instance.Unlock();

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
        PathSystem.Instance.Unlock();
        SetState(GameState.Ready);

        OnWaveCleared(currentWaveIndex);
    }

    public void OnMonsterReachGoal()
    {
        gateHp--;
        if (gateHp < 0) gateHp = 0;
        UpdateGateHpUI();
        if (HeartHPUI.Instance != null)
        {
            HeartHPUI.Instance.SetHP(gateHp);
        }

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
    }

    private void OnAllWavesCleared()
    {
        SaveStageClearProgress();
        SetState(GameState.Result);
        Debug.Log("[GM] 모든 웨이브 클리어 → SUCCESS");
        ShowSuccess();
    }

    private void SaveStageClearProgress()
    {
        if (stage == null || string.IsNullOrWhiteSpace(stage.stageId))
            return;

        if (!int.TryParse(stage.stageId, out int clearedStageNumber))
        {
            Debug.LogWarning($"[GM] stageId({stage.stageId})스테이지아이디");
            return;
        }

        ProgressionSave.MarkStageCleared(clearedStageNumber);

        if (clearedStageNumber >= churrosUnlockStage)
        {
            ProgressionSave.UnlockTower(churrosTowerUnlockKey);
            EventBus.Publish(Events.OnTowerUnlocked, churrosTowerUnlockKey);
            Debug.Log($"[GM] ({churrosTowerUnlockKey}) . (Stage {clearedStageNumber})타워 클리어스테이지번호");
        }
    }

    private void OnFailed()
    {
        Debug.Log("Gate HP 0 → Game Over");
        SetState(GameState.Result);
        ShowFail();
    }

    private void SetState(GameState next)
    {
        if (CurrentState == next) return;
        CurrentState = next;
        EventBus.Publish(Events.OnStateChanged, next);
    }

    private void ShowSuccess()
    {
        SetState(GameState.Result);
        if (successPanel != null) successPanel.SetActive(true);

        if (PauseManager.Instance != null)
            PauseManager.Instance.Pause();
    }

    private void ShowFail()
    {
        SetState(GameState.Result);
        if (failPanel != null) failPanel.SetActive(true);

        if (PauseManager.Instance != null)
            PauseManager.Instance.Pause();
    }
}
