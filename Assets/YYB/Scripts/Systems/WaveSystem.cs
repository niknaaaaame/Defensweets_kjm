using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/// <summary>
/// 웨이브 하나의 라이프사이클을 책임지는 시스템.
/// - 준비시간 대기 → 그룹별 스폰 → 스폰 종료 감시 → 필드 클리어 감시 → 보상 지급
/// - 스폰 직후 프리팹에 WaveAgent를 붙여서
///   파괴(사망/목표 도달) 시점에 자동으로 WaveSystem에 보고되게 한다.
/// </summary>
public class WaveSystem : MonoBehaviour
{
    public static WaveSystem Instance { get; private set; }

    [Header("Spawn Settings")]
    [Tooltip("몬스터 소환 위치(필수)")]
    public Transform spawnPoint;

    // 필드에 살아있는 몬스터 트래킹(중복 방지용 HashSet + 순회용 List)
    private readonly HashSet<GameObject> activeMonsterSet = new HashSet<GameObject>();
    private readonly List<GameObject> activeMonsters = new List<GameObject>();

    // 외부에서 참조하는 상태
    public bool IsWaveActive { get; private set; } = false;
    public bool IsSpawnFinished => spawningGroups <= 0;

    // 내부 스폰 상태
    private int spawningGroups = 0;            // 동시에 돌고 있는 그룹 코루틴 수
    private Coroutine waveRoutine;

    // 콜백(옵션): GameManager에서 넘겨줄 수 있음
    private Action onSpawnedCb;     // 몬스터 1마리 스폰될 때마다
    private Action onOneRemovedCb;  // 몬스터 1마리 제거(사망/도달)될 때마다

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        // (선택) 이벤트 구독: 팀의 Monster가 이벤트를 발행한다면 여기에 연결 가능
        // EventBus.Subscribe(Events.OnMonsterKilled, _ => {/*자원 처리 등*/});
        // EventBus.Subscribe(Events.OnMonsterReachGoal, _ => {/*게이트 처리 등*/});
    }

    /// <summary>
    /// GameManager에서 호출. 웨이브 한 번 실행.
    /// </summary>
    public IEnumerator RunWave(WaveSO waveData, Action onSpawned = null, Action onOneRemoved = null)
    {
        if (waveData == null)
        {
            Debug.LogError("[WaveSystem] RunWave: waveData is null");
            yield break;
        }
        if (spawnPoint == null)
        {
            Debug.LogError("[WaveSystem] RunWave: spawnPoint not assigned");
            yield break;
        }

        IsWaveActive = true;
        onSpawnedCb = onSpawned;
        onOneRemovedCb = onOneRemoved;

        // 준비 시간
        if (waveData.prepareTime > 0f)
            yield return new WaitForSeconds(waveData.prepareTime);

        // 그룹별 스폰 시작 (병렬 가능)
        spawningGroups = 0;
        foreach (var group in waveData.spawns)
        {
            // struct라 null 비교 불가 → 필드로 유효성 판단
            if (group.count <= 0) continue;
            if (group.monster == null) continue;

            spawningGroups++;
            StartCoroutine(SpawnGroup(group));
        }

        // 스폰이 모두 끝나고, 필드에 몬스터가 0마리 될 때까지 대기
        while (true)
        {
            if (IsSpawnFinished && activeMonsters.Count == 0) break;
            yield return null;
        }

        // 웨이브 종료 보상
        var rw = waveData.reward;
        if (rw.sugar > 0)
            ResourceSystem.Instance.AddSugar(rw.sugar);

        if (rw.crystalBonus > 0)
            ResourceSystem.Instance.TryUseCrystal(-rw.crystalBonus);

        // 상태 정리
        IsWaveActive = false;
        onSpawnedCb = null;
        onOneRemovedCb = null;

        // (선택) 웨이브 종료 이벤트 방송
        // EventBus.Publish(Events.OnWaveCleared, null);
    }

    /// <summary>
    /// 한 그룹 스폰(시작 지연 → n회 소환 → 간격)
    /// </summary>
    private IEnumerator SpawnGroup(SpawnGroup group)
    {
        if (group.startDelay > 0f)
            yield return new WaitForSeconds(group.startDelay);

        for (int i = 0; i < group.count; i++)
        {
            SpawnOne(group.monster);
            onSpawnedCb?.Invoke();

            if (group.interval > 0f)
                yield return new WaitForSeconds(group.interval);
            else
                yield return null; // 같은 프레임 과도 생성 방지
        }

        spawningGroups--;
        if (spawningGroups < 0) spawningGroups = 0; // 안전장치
    }

    /// <summary>
    /// 몬스터 1기 소환 + WaveAgent 부착해서 생애주기 관리
    /// </summary>
    private void SpawnOne(MonsterSO monsterData)
    {
        var prefab = monsterData.prefab;
        if (prefab == null)
        {
            Debug.LogWarning("[WaveSystem] SpawnOne: Monster prefab is null");
            return;
        }

        var go = GameObject.Instantiate(prefab, spawnPoint.position, Quaternion.identity);

        // WaveAgent 부착(중복 방지)
        var agent = go.GetComponent<WaveAgent>();
        if (agent == null) agent = go.AddComponent<WaveAgent>();
        agent.Setup(this);

        RegisterMonster(go);

        // (팀원이 Monster.cs에서 Initialize를 제공하면, 여기서 주입)
        // var monster = go.GetComponent<Monster>();
        // if (monster != null) monster.Initialize(monsterData /*, PathSystem 등 필요시*/);
    }

    /// <summary>필드 트래킹: 등록</summary>
    private void RegisterMonster(GameObject go)
    {
        if (go == null) return;
        if (activeMonsterSet.Add(go))
            activeMonsters.Add(go);
    }

    /// <summary>필드 트래킹: 제거(사망/목표도달/파괴)</summary>
    private void UnregisterMonster(GameObject go)
    {
        if (go == null) return;

        if (activeMonsterSet.Remove(go))
        {
            // List에서 빠르게 제거(뒤에서 스왑팝)
            int idx = activeMonsters.IndexOf(go);
            if (idx >= 0)
            {
                int last = activeMonsters.Count - 1;
                activeMonsters[idx] = activeMonsters[last];
                activeMonsters.RemoveAt(last);
            }

            onOneRemovedCb?.Invoke();
        }
    }

    /// <summary>
    /// 몬스터가 파괴될 때 WaveAgent가 호출
    /// </summary>
    internal void OnMonsterDestroyed(GameObject go)
    {
        UnregisterMonster(go);
    }

    // ──────────────────────────────────────────────────────────
    // 내부 헬퍼: 스폰된 몬스터에 붙여져 수명 종료시 보고하는 컴포넌트
    [DisallowMultipleComponent]
    private sealed class WaveAgent : MonoBehaviour
    {
        private WaveSystem owner;
        private bool reported = false;

        public void Setup(WaveSystem sys) => owner = sys;

        // 어떤 이유로든 파괴될 때(사망/골도달/씬종료) 자동 보고
        private void OnDestroy()
        {
            // 씬 종료 시점에 중복 호출 방지
            if (reported || owner == null) return;
            reported = true;
            owner.OnMonsterDestroyed(gameObject);
        }
    }
}