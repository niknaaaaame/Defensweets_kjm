using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class WaveSystem : MonoBehaviour
{
    public static WaveSystem Instance { get; private set; }

    private bool isSpawning = false;
    public bool IsSpawnFinished => !isSpawning;

    private List<GameObject> activeMonsters = new List<GameObject>(); // 필드에 존재하는 몬스터 관리
    private Coroutine waveRoutine;

    [Header("Spawn Point")]
    public Transform spawnPoint; // 인스펙터에 설정 (시작 위치)

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    
    /// 웨이브 실행 (GameManager에서 호출)
    
    public IEnumerator RunWave(WaveSO waveData, System.Action onSpawned, System.Action onOneDied)
    {
        Debug.Log($"[WaveSystem] 웨이브 시작: {waveData.name}");

        isSpawning = true;

        // 웨이브 준비 시간
        yield return new WaitForSeconds(waveData.prepareTime);

        // 스폰 시작
        foreach (var group in waveData.spawns)
        {
            yield return new WaitForSeconds(group.startDelay);
            StartCoroutine(SpawnGroup(group, onSpawned));
        }

        // 모든 그룹이 끝날 때까지 대기
        while (isSpawning)
        {
            if (activeMonsters.Count == 0) break;
            yield return null;
        }

        // 웨이브 종료 보상 처리
        var reward = waveData.reward;
        if (reward.sugar > 0) ResourceSystem.Instance.AddSugar(reward.sugar);
        if (reward.crystalBonus > 0) ResourceSystem.Instance.TryUseCrystal(-reward.crystalBonus); // 음수 → 추가 지급

        Debug.Log($"[WaveSystem] 웨이브 종료: {waveData.name}");
    }

    
    /// 하나의 몬스터 그룹 스폰
    
    private IEnumerator SpawnGroup(SpawnGroup group, System.Action onSpawned)
    {
        for (int i = 0; i < group.count; i++)
        {
            SpawnMonster(group.monster);
            onSpawned?.Invoke();
            yield return new WaitForSeconds(group.interval);
        }

        // 모든 스폰 완료 후 대기 (혹시 남은 그룹이 있으면 아직 isSpawning 유지)
        yield return new WaitForSeconds(0.5f);
        isSpawning = false;
    }

    
    /// 몬스터 인스턴스 생성
    
    private void SpawnMonster(MonsterSO monsterData)
    {
        if (spawnPoint == null)
        {
            Debug.LogWarning("[WaveSystem] Spawn Point가 지정되지 않았습니다.");
            return;
        }

        GameObject monster = Instantiate(monsterData.prefab, spawnPoint.position, Quaternion.identity);
        /// Monster monsterComp = monster.GetComponent<Monster>();
        /// monsterComp.Initialize(monsterData, this);

        activeMonsters.Add(monster);
    }

    
    /// 몬스터가 사망하거나 필드에서 제거될 때 호출
    
    public void OnMonsterRemoved(GameObject monster)
    {
        if (activeMonsters.Contains(monster))
            activeMonsters.Remove(monster);
    }
}
