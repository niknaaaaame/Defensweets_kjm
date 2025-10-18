using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Monster : MonoBehaviour
{
    public MonsterSO monsterData;

    private int currentHp;
    private List<Vector3> path;
    private int currentWaypointIndex = 0;
    private Transform goal;     // 도착지점

    // Start is called before the first frame update
    void Start()
    {
        currentHp = monsterData.hp;

        goal = GameObject.FindWithTag("Goal").transform; // 도착지점 Goal 태그 가진 오브젝트 태그는 임시
        path = Pathfinding.FindPath(transform.position, goal.position);

        if (path == null)
        {
            Debug.LogError("경로가 없습니다.", gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void HandleMovement()
    {
        if (path == null || currentWaypointIndex >= path.Count)
        {
            return;     // 도착 또는 길 없음
        }

        Vector3 targetPosition = path[currentWaypointIndex];
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, monsterData.speed * Time.deltaTime);

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
        Destroy(gameObject);
    }
}
