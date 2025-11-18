using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterAttack : MonoBehaviour
{
    public float attackRange = 4.0f;
    public float attackDuration = 0.5f;
    public float attackCooldown = 2.0f;
    public int damage = 10;

    public string targetTag = "Tower";

    private Monster monsterMovement;
    private float lastAttackTime;
    private bool isAttacking = false;

    // Start is called before the first frame update
    void Start()
    {
        monsterMovement = GetComponent<Monster>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isAttacking) return;    // 공격 중일 시 취소
        
        if (Time.time < lastAttackTime + attackCooldown) return;    // 쿨타임 중일 시 취소

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, attackRange);    // 사거리 내 타워 감지

        foreach (var hit in hits)
        {
            if (hit.CompareTag(targetTag))
            {
                StartCoroutine(AttackRoutine(hit.gameObject));
                break;
            }
        }
    }

    private IEnumerator AttackRoutine(GameObject target)
    {
        isAttacking = true;

        if (monsterMovement != null) monsterMovement.IsStopped = true;

        // 나중에 데미지 처리 넣기

        yield return new WaitForSeconds(attackDuration);

        if (monsterMovement != null) monsterMovement.IsStopped = false;

        isAttacking = false;
        lastAttackTime = Time.time;
    }
}
