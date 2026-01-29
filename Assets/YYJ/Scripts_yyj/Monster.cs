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

    public bool IsStopped { get; set; } = false;

    private SpriteRenderer spriteRenderer;

    // Start is called before the first frame update
    void Start()
    {
        currentHp = monsterData.hp;
        currentSpeed = monsterData.speed;

        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogError("SpriteRenderer가 없습니다", gameObject);
        }

        //goal = GameObject.FindWithTag("Goal").transform;
        //GameObject goalObject = GameObject.Find("Start");   
        GameObject goalObject = GameObject.FindWithTag("Goal"); //윗줄 주석하고 이 줄 추가했어요. -여영부-
        if ( goalObject == null )
        {
            Debug.LogError("Goal이 존재하지 않습니다.");
            Destroy(gameObject);
            return;
        }
        goal = goalObject.transform;

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
        CheckTileEffect();

        HandleMovement();
    }

    private void CheckTileEffect()
    {
        if (TilemapReader_YYJ.Instance == null) return;
        
        // 위치의 타일 효과 확인
        TileEffectType effect = TilemapReader_YYJ.Instance.GetEffectAtWorldPos(transform.position);

        switch (effect)
        {
            case TileEffectType.Sticky:
                currentSpeed = monsterData.speed * 0.5f;
                break;

            case TileEffectType.Explosive:
                // 나중에 추가
                break;

            case TileEffectType.None:
            default:
                currentSpeed = monsterData.speed;
                break;
        }
    }

    private void HandleMovement()
    {
        if (path == null || currentWaypointIndex >= path.Count || isStunned || IsStopped)
        {
            return;     // 도착 또는 길 없음
        }

        Vector3 targetPosition = path[currentWaypointIndex];

        if (spriteRenderer != null) // 좌우 반전
        {
            float directionX = targetPosition.x - transform.position.x;

            if (directionX > 0.01f)
            {
                spriteRenderer.flipX = false;
            }
            else if (directionX < -0.01f)
            {
                spriteRenderer.flipX = true;
            }
            
        }
    
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
        GameManager.Instance.OnMonsterReachGoal();

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

        EventBus.Publish(Events.OnMonsterKilled, monsterData.rewardSugar);

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
        {
            return;
        }

        if (WaveSystem.Instance == null)
        {
            Debug.LogError("WaveSystem을 찾을 수 없습니다.");
            return;
        }

        for (int i = 0; i < monsterData.splitCount; i++)
        {
            Vector3 spawnPos = transform.position + new Vector3(Random.Range(-0.2f, 0.2f), Random.Range(-0.2f, 0.2f), 0);
        }
    }
}
