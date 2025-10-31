using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Monster : MonoBehaviour, IDamageable
{
    public MonsterSO monsterData;

    private int currentHp;
    private IReadOnlyList<Vector3> path;
    private int currentWaypointIndex = 0;
    private Transform goal;     // ��������
    private bool isDying = false;

    // ���� �̻� ���� ����
    private float currentSpeed;
    private bool isStunned = false;
    private Coroutine slowCoroutine;
    private Coroutine stunCoroutine;

    // Start is called before the first frame update
    void Start()
    {
        currentHp = monsterData.hp;
        currentSpeed = monsterData.speed;;

        goal = GameObject.FindWithTag("Goal").transform;

        path = BFS.FindPath(transform.position, goal.position);

        if (path == null || path.Count == 0)
        {
            Debug.LogError("��ΰ� �����ϴ�.", gameObject);
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
            return;     // ���� �Ǵ� �� ����
        }

        Vector3 targetPosition = path[currentWaypointIndex];
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, currentSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            currentWaypointIndex++;

            if (currentWaypointIndex >= path.Count)     // ���� ������ ���� ��
            {
                OnReachGoal();
            }
        }
    }

    private void OnReachGoal()      // ���� �� ���Ϳ��� �߻��� ���� ���� �߰�
    {
        GameManager.Instance.OnMonsterReachGoal();

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

    private void Die()      // ��� �� ���Ϳ��� �߻��� ���� ���� �߰�
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
        // ���� ���Ͱ� �п����� �ʴ� ���Ͷ�� return

        // for (int i = 0; i < �п� ��; i++)
        //  �п� ���� ������ ����
    }
}
