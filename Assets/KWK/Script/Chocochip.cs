using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chocochip : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float lifeTime;

    private Transform target;
    private Rigidbody2D rb;
    private Coroutine moveCoroutine;
    private int damage;

    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Setting(Transform newTarget, int newDamage)
    {
        target = newTarget;
        damage = newDamage;

        rb = GetComponent<Rigidbody2D>();

        moveCoroutine = StartCoroutine(move());
    }

    private void OnTriggerEnter2D(Collider2D other)     //몬스터테스트인거 일반 몬스터로 바꿀게 -여영부-
    {
        if (other.CompareTag("Monster"))
        {
            Destroy(gameObject);
        }
    }

    /*private void OnTriggerEnter2D(Collider2D other)     //몬스터테스트인거 일반 몬스터로 바꿀게 -여영부-
    {
        if (!other.CompareTag("Monster"))
            return;

        // 여기 타입만 Monster 로 변경
        Monster monster = other.GetComponent<Monster>();
        if (monster != null)
        {
            monster.TakeDamage(damage);   // Monster 쪽 데미지 함수 이름에 맞춰서 호출

            if (moveCoroutine != null)
                StopCoroutine(moveCoroutine);

            Destroy(gameObject);
        }
    }*/

    private IEnumerator move()
    {
        Vector2 direction = (target.position - transform.position).normalized;
        rb.velocity = direction * speed;

        yield return new WaitForSeconds(0.1f);
    }
}
