using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Monster : MonoBehaviour, IDamageable
{
    public MonsterSO_YYJ monsterData;

    private int currentHp;
    private List<Vector3> path;
    private int currentWaypointIndex = 0;
    private Transform goal;     // 도착지점
    private bool isDying = false;

    // 상태 이상 관리 변수
    private float currentSpeed;
    private bool isStunned = false;
    private Coroutine slowCoroutine;
    private Coroutine stunCoroutine;

    // Start is called before the first frame update
    void Start()
    {
        currentHp = monsterData.hp;
        currentSpeed = monsterData.speed;

        //goal = GameObject.FindWithTag("Goal").transform;
        goal = GameObject.Find("Goal").transform;
        if ( goal == null )
        {
            Debug.LogError("Goal이 존재하지 않습니다.");
            Destroy(gameObject);
            return;
        }

        path = BFS.FindPath(transform.position, goal.position);

        if (path == null || path.Count == 0)
        {
            Debug.LogError("경로가 없습니다.", gameObject);
            Destroy(gameObject);
            return;
        }
    }

    // Update is called once per frame
    void Update()
    {
        HandleMovement();
    }

    private void HandleMovement()
    {
        if (path == null || currentWaypointIndex >= path.Count || isStunned)
        {
            return;     // 도착 또는 길 없음
        }

        Vector3 targetPosition = path[currentWaypointIndex];
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, currentSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            currentWaypointIndex++;

            if (currentWaypointIndex >= path.Count)     // 최종 목적지 도달 시
            {
                OnReachGoal();
            }
        }
    }

    private void OnReachGoal()      // 도착 시 몬스터에게 발생할 사항 이후 추가
    {
        // GameManager.Instance.OnMonsterReachGoal();

        Debug.Log("Goal!");

        Destroy(gameObject);        
    }

    public void TakeDamage(int amount)
    {
        currentHp -= amount;
        if (currentHp <= 0)
        {
            Die();
        }
    }

    private void Die()      // 사망 시 몬스터에게 발생할 사항 이후 추가
    {
        if (isDying) return;
        isDying = true;

        //EventBus.Publish(Events.OnMonsterKilled, monsterData.rewardSugar);

        HandleSplit();

        Destroy(gameObject);
    }

    public void ApplySlow(float slowPercentage, float duration)
    {
        float effectiveSlow = slowPercentage * (1.0f - monsterData.slowResist);

        if (effectiveSlow <= 0) return;

        if (slowCoroutine != null)
        {
            StopCoroutine(slowCoroutine);
        }

        float slowedSpeed = monsterData.speed * (1.0f - effectiveSlow);
        slowCoroutine = StartCoroutine(SlowRoutine(slowedSpeed, duration));
    }

    private IEnumerator SlowRoutine(float slowedSpeed, float duration)
    {
        currentSpeed = slowedSpeed;

        yield return new WaitForSeconds(duration);

        currentSpeed = monsterData.speed;
        slowCoroutine = null;
    }

    public void ApplyStun(float duration)
    {
        if (stunCoroutine != null)
        {
            StopCoroutine(stunCoroutine);
        }

        stunCoroutine = StartCoroutine(StunRoutine(duration));
    }

    private IEnumerator StunRoutine(float duration)
    {
        isStunned = true;

        yield return new WaitForSeconds(duration);

        isStunned = false;
        stunCoroutine = null;
    }

    private void HandleSplit()
    {
        if (!monsterData.splitsOnDeath || monsterData.splitMonsterSO == null || monsterData.splitCount <= 0)
            { return; }

        for (int i = 0; i < monsterData.splitCount; i++)    // 분열 수 만큼 몬스터 생성
        {
            Vector3 spawnPos = transform.position + new Vector3(Random.Range(-0.2f, 0.2f), Random.Range(-0.2f, 0.2f), 0); // 위치 랜덤 조정

            Instantiate(monsterData.splitMonsterSO.prefab, spawnPos, Quaternion.identity);
        }
    }
}
