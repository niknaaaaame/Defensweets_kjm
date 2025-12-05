using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class WaveSystem : MonoBehaviour
{
    public static WaveSystem Instance { get; private set; }

    [Header("Spawn Settings")]
    [Tooltip("몬스터 소환 위치(필수)")]
    public Transform spawnPoint;

    private readonly HashSet<GameObject> activeMonsterSet = new HashSet<GameObject>();
    private readonly List<GameObject> activeMonsters = new List<GameObject>();

    public bool IsWaveActive { get; private set; } = false;
    public bool IsSpawnFinished => spawningGroups <= 0;

    private int spawningGroups = 0;
    private Coroutine waveRoutine;

    private Action onSpawnedCb;     // 몬스터 1마리 스폰
    private Action onOneRemovedCb;  // 몬스터 1마리 제거

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

    }

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

        if (waveData.prepareTime > 0f)
            yield return new WaitForSeconds(waveData.prepareTime);

        spawningGroups = 0;
        foreach (var group in waveData.spawns)
        {
            if (group.count <= 0) continue;
            if (group.monster == null) continue;

            spawningGroups++;
            StartCoroutine(SpawnGroup(group));
        }

        while (true)
        {
            if (IsSpawnFinished && activeMonsters.Count == 0) break;
            yield return null;
        }

        var rw = waveData.reward;
        if (rw.sugar > 0)
            ResourceSystem.Instance.AddSugar(rw.sugar);

        if (rw.crystalBonus > 0)
            ResourceSystem.Instance.AddCrystal(rw.crystalBonus);

        IsWaveActive = false;
        onSpawnedCb = null;
        onOneRemovedCb = null;

        // EventBus.Publish(Events.OnWaveCleared, null);
    }

    
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
                yield return null;
        }

        spawningGroups--;
        if (spawningGroups < 0) spawningGroups = 0;
    }

    private void SpawnOne(MonsterSO_YYJ monsterData)
    {
        var prefab = monsterData.prefab;
        if (prefab == null)
        {
            Debug.LogWarning("[WaveSystem] SpawnOne: Monster prefab is null");
            return;
        }

        var go = GameObject.Instantiate(prefab, spawnPoint.position, Quaternion.identity);

        var agent = go.GetComponent<WaveAgent>();
        if (agent == null) agent = go.AddComponent<WaveAgent>();
        agent.Setup(this);

        RegisterMonster(go);
    }

    private void RegisterMonster(GameObject go)
    {
        if (go == null) return;
        if (activeMonsterSet.Add(go))
            activeMonsters.Add(go);
    }

    private void UnregisterMonster(GameObject go)
    {
        if (go == null) return;

        if (activeMonsterSet.Remove(go))
        {
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

    internal void OnMonsterDestroyed(GameObject go)
    {
        UnregisterMonster(go);
    }

    [DisallowMultipleComponent]
    private sealed class WaveAgent : MonoBehaviour
    {
        private WaveSystem owner;
        private bool reported = false;

        public void Setup(WaveSystem sys) => owner = sys;

        private void OnDestroy()
        {
            if (reported || owner == null) return;
            reported = true;
            owner.OnMonsterDestroyed(gameObject);
        }
    }
}